using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace FrameWork.Sound
{
    public class SoundManager : Singleton<SoundManager>
    {
        private int musicOpen = 1;
        private int soundOpen = 1;
        private AudioSource audioSource;
        
        private Dictionary<string, AudioSource> loopSources = new Dictionary<string, AudioSource>();

        public void Init()
        {
            var audioGo = GameObject.Find("AudioSource");
            if (!audioGo)
            {
                audioGo = new GameObject("AudioSource");
                audioGo.transform.localPosition = Vector3.zero;
                audioSource = audioGo.AddComponent<AudioSource>();
            }
            else
            {
                audioSource = audioGo.GetComponent<AudioSource>();
            }

            GameObject.DontDestroyOnLoad(audioSource);
            musicOpen = CustomPrefesUtil.GetCommonData("music_open", 1);
            soundOpen = CustomPrefesUtil.GetCommonData("sound_open", 1);
        }

        ///<summary>
        /// 修改sound的状态
        ///<summary>
        public void ChangeSoundState()
        {
            soundOpen = soundOpen > 0 ? 0 : 1;
            CustomPrefesUtil.SaveCommonData("sound_open", soundOpen);
        }

        ///<summary>
        /// 修改music的状态
        ///<summary>
        public void ChangeMusicState()
        {
            musicOpen = musicOpen > 0 ? 0 : 1;
            ChangeAudioSourceState();
            CustomPrefesUtil.SaveCommonData("music_open", musicOpen);
        }

        ///<summary>
        ///更改 audiosource 状态
        ///</summary>
        public void ChangeAudioSourceState()
        {
            if (musicOpen > 0)
            {
                audioSource.Play();
            }
            else
            {
                audioSource.Stop();
            }
        }

        public async UniTask PlayMusic(string musicName)
        {
            await PlayMusic("", musicName);
        }

        public async UniTask PlayMusic(string package, string musicName)
        {
            audioSource.clip = await GetAudioClipByName(package, musicName);
            audioSource.loop = true;
            ChangeAudioSourceState();
        }

        private Dictionary<string, float> lastPlayTimes = new Dictionary<string, float>();

        public async UniTask PlaySound(string clipName, float time = 0)
        {
            if (time > 0)
            {
                if (lastPlayTimes.ContainsKey(clipName))
                {
                    if (UnityEngine.Time.time - lastPlayTimes[clipName] < time)
                    {
                        return;
                    }
                }

                lastPlayTimes[clipName] = UnityEngine.Time.time;
            }

            await PlaySound("", clipName);
        }


        public async UniTask PlaySound(string package, string clipName)
        {
            if (!SoundOpen) return;
            audioSource.PlayOneShot(await GetAudioClipByName(package, clipName));
        }

        // 新增：播放循环音效
        public async UniTask PlaySound(string clipName, bool loop)
        {
            if (!SoundOpen) return;
            // 如果已经在播放，就不重复播放
            if (IsPlaying(clipName)) return;

            AudioClip clip = await GetAudioClipByName("", clipName);
            if (clip == null) return;

            if (loop)
            {
                // 创建一个新的 AudioSource 用于循环播放
                GameObject go = new GameObject("Sound_" + clipName);
                go.transform.SetParent(audioSource.transform);
                AudioSource source = go.AddComponent<AudioSource>();
                source.clip = clip;
                source.loop = true;
                source.Play();

                if (loopSources.ContainsKey(clipName))
                {
                    // 理论上 IsPlaying 已经拦截了，但为了安全起见
                    GameObject.Destroy(loopSources[clipName].gameObject);
                    loopSources[clipName] = source;
                }
                else
                {
                    loopSources.Add(clipName, source);
                }
            }
            else
            {
                audioSource.PlayOneShot(clip);
            }
        }

        // 新增：播放 3D 循环音效
        public async UniTask Play3DSound(string clipName, Transform target, float minDistance = 1f,
            float maxDistance = 10f)
        {
            if (!SoundOpen) return;
            // 如果已经在播放，就不重复播放
            if (IsPlaying(clipName)) return;

            AudioClip clip = await GetAudioClipByName("", clipName);
            if (clip == null) return;

            // 创建一个新的 AudioSource 用于 3D 播放
            GameObject go = new GameObject("Sound3D_" + clipName);
            go.transform.SetParent(target);
            go.transform.localPosition = Vector3.zero;

            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;

            // 设置 3D 音效参数
            source.spatialBlend = 1f; // 1.0 为完全 3D
            source.rolloffMode = AudioRolloffMode.Linear; // 线性衰减
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;

            source.Play();

            if (loopSources.ContainsKey(clipName))
            {
                if (loopSources[clipName] != null)
                {
                    GameObject.Destroy(loopSources[clipName].gameObject);
                }

                loopSources[clipName] = source;
            }
            else
            {
                loopSources.Add(clipName, source);
            }
        }

        // 新增：停止音效
        public void StopSound(string clipName)
        {
            if (loopSources.ContainsKey(clipName))
            {
                AudioSource source = loopSources[clipName];
                if (source != null)
                {
                    source.Stop();
                    GameObject.Destroy(source.gameObject);
                }

                loopSources.Remove(clipName);
            }
        }

        // 新增：检查音效是否在播放
        public bool IsPlaying(string clipName)
        {
            if (loopSources.ContainsKey(clipName))
            {
                var source = loopSources[clipName];
                return source != null && source.isPlaying;
            }

            return false;
        }

        private const int ShortAudioCacheLimit = 10;

        /// <summary>缓存条目：AudioClip + 对应的 YooAsset Handle + 在 LRU 链表中的节点。</summary>
        private sealed class AudioCacheEntry
        {
            public AudioClip Clip;
            public AssetHandle Handle;
            public LinkedListNode<string> ListNode;
        }

        private readonly LinkedList<string> _audioLruList = new();
        private readonly Dictionary<string, AudioCacheEntry> _audioCache = new();

        /// <summary>
        /// 获取短音频的 AudioClip，使用 LRU 缓存（最多缓存10个）。
        /// 每次命中时将条目移到队列最前；缓存满时淘汰队列最后的条目并释放其资源。
        /// </summary>
        private async UniTask<AudioClip> GetAudioClipByName(string package, string clipName)
        {
            string key = package + "_" + clipName;

            // 命中缓存：移到 LRU 队列最前（O(1)）
            if (_audioCache.TryGetValue(key, out var entry) && entry.Clip != null)
            {
                _audioLruList.Remove(entry.ListNode);
                _audioLruList.AddFirst(entry.ListNode);
                return entry.Clip;
            }

            // 未命中或之前缓存了 null：清理残留
            _audioCache.Remove(key);

            // 加载新资源
            AudioClip clip;
            AssetHandle handle = default;
            if (package == "Resources")
            {
                clip = Resources.Load<AudioClip>(clipName);
            }
            else
            {
                handle = string.IsNullOrEmpty(package)
                    ? YooAssets.LoadAssetAsync<AudioClip>(clipName)
                    : YooAssets.GetPackage(package).LoadAssetAsync<AudioClip>(clipName);
                await handle.ToUniTask();
                clip = handle.AssetObject as AudioClip;
            }

            if (clip == null) return null;

            // 淘汰最久未使用的条目
            while (_audioLruList.Count >= ShortAudioCacheLimit)
            {
                var lastKey = _audioLruList.Last.Value;
                _audioLruList.RemoveLast();
                EvictAudioCache(lastKey);
            }

            // 加入缓存并标记为最近使用
            var node = _audioLruList.AddFirst(key);
            _audioCache[key] = new AudioCacheEntry
            {
                Clip = clip,
                Handle = handle,
                ListNode = node,
            };

            return clip;
        }

        private void EvictAudioCache(string key)
        {
            if (_audioCache.TryGetValue(key, out var entry))
            {
                if (entry.Handle.IsValid)
                    entry.Handle.Release();
                _audioCache.Remove(key);
            }
        }

        /// <summary>
        /// 背景音乐状态
        /// </summary>
        public bool MusicOpen
        {
            get { return musicOpen > 0; }
        }

        /// <summary>
        /// 音效状态
        /// </summary>
        public bool SoundOpen
        {
            get { return soundOpen > 0; }
        }

        public void PlayByMiniGame(AudioClip clip)
        {
            if (!SoundOpen) return;
            audioSource.PlayOneShot(clip);
        }

        private float lastVibrateTime;
        private float lastResourceInTime;
        private float lastResourceOutTime;

        public void DrillVibrate()
        {
            if (UnityEngine.Time.time - lastVibrateTime < 0.2f)
                return;
            lastVibrateTime = UnityEngine.Time.time;
            PlatformHelper.Platform.Vibrate(20, 50);
        }

        public void ResourceInVibrate()
        {
            if (UnityEngine.Time.time - lastResourceInTime < 0.1f)
                return;
            lastResourceInTime = UnityEngine.Time.time;
            PlatformHelper.Platform.Vibrate(20, 50);
        }

        public void ResourceOutVibrate()
        {
            if (UnityEngine.Time.time - lastResourceOutTime < 0.1f)
                return;
            lastResourceOutTime = UnityEngine.Time.time;
            PlatformHelper.Platform.Vibrate(20, 50);
        }

        public void Vibrate(int duration, int amplitude)
        {
            PlatformHelper.Platform.Vibrate(duration, amplitude);
        }

        /// <summary>
        /// 清理所有循环音效
        /// </summary>
        public void StopAllLoopSounds()
        {
            foreach (var kv in loopSources)
            {
                if (kv.Value != null)
                {
                    kv.Value.Stop();
                    GameObject.Destroy(kv.Value.gameObject);
                }
            }
            loopSources.Clear();
        }

        /// <summary>
        /// 释放短音频 LRU 缓存，低内存时调用。
        /// </summary>
        public void ReleaseAudioCaches()
        {
            foreach (var entry in _audioCache.Values)
            {
                if (entry.Handle.IsValid)
                    entry.Handle.Release();
            }
            _audioCache.Clear();
            _audioLruList.Clear();
        }
    }
}