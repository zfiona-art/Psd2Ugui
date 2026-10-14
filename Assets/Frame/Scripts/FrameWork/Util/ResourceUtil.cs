using UnityEngine;
using LitJson;
using System.Collections.Generic;

public class ResourceUtil
{
    public static Sprite LoadSprite(string path)
    {
        return Resources.Load<Sprite>(path);
    }

    public static GameObject LoadPrefab(string path)
    {
        return Resources.Load<GameObject>(path);
    }

    public static Material LoadMaterial(string path)
    {
        return Resources.Load<Material>(path);
    }

    public static string GetConfig(string configName)
    {
        var asset = Resources.Load<TextAsset>("Config/" + configName);
        if (asset != null)
            return asset.text;
        return "";
    }

    private static Dictionary<string, AudioClip> audioClipDic = new Dictionary<string, AudioClip>();
    ///<summary>
    /// 加载Audio
    ///</summary>
    ///<param name = "clipName">音效名称</param>
    public static AudioClip GetAudioClip(string clipName)
    {
        AudioClip clip = null;
        string clipStr = clipName;
        if (audioClipDic.ContainsKey(clipStr))
        {
            return audioClipDic[clipStr];
        }
        if (clip == null)
        {
            clip = Resources.Load<AudioClip>("Sound/" + clipName);
        }
        if (clip != null)
        {
            audioClipDic[clipStr] = clip;
            return clip;
        }
        Debug.LogError(clipName + " this audio clip doesn't exit");
        return null;
    }
}