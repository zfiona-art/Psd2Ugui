using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;
using DG.Tweening;
using FrameWork.Sound;
using UnityEngine.UI;

namespace FrameWork.UI
{
    public class UIManager : Singleton<UIManager>
    {
        private Dictionary<ViewType, Transform> viewParentDic = new();
        private Dictionary<string, ViewConf> viewPathDic = new();
        private List<ViewInfo> showingList = new();

        public void Init(Dictionary<string, ViewConf> dic)
        {
            viewPathDic = dic;
        }

        public void AddViewConf(string viewId, ViewConf conf)
        {
            if (viewPathDic.ContainsKey(viewId))
                viewPathDic[viewId] = conf;
            else
                viewPathDic.Add(viewId, conf);
        }

        public async UniTaskVoid LoadLayer(string viewId, ViewContextData contextData = null)
        {
            if (IsShowing(viewId))
            {
                var viewItem = GetViewComp(viewId);
                viewItem.ShowView(viewId, contextData);
                viewItem.transform.SetAsLastSibling();
                Debug.LogWarning("is aleady showing layer: reopen" + viewId);
                return;
            }

            await OpenUI(viewId, ViewType.Layer, contextData);
        }

        public async UniTask LoadPanel(string viewId, ViewContextData contextData = null)
        {
            if (IsShowing(viewId))
            {
                return;
            }

            await OpenUI(viewId, ViewType.Panel, contextData);
        }

        public async UniTask LoadPopUp(string viewId, ViewContextData contextData = null)
        {
            await OpenUI(viewId, ViewType.PopUp, contextData);
        }

        public void CloseView(string viewId)
        {
            ViewInfo info = GetViewInfo(viewId);
            if (info == null)
            {
                Debug.LogWarning($"view:{viewId} is not opening!");
                return;
            }

            showingList.Remove(info);
            info.isClose = true;
            if (!info.ViewBase)
            {
                return;
            }

            removeGlobalButtonClick(info.ViewBase.gameObject);

            info.ViewBase.CloseAnim(() => { GameObject.Destroy(info.ViewBase.gameObject); });
        }

        public async UniTask OpenUI(string viewId, ViewType viewType, ViewContextData contextData)
        {
            if (!viewPathDic.TryGetValue(viewId, out ViewConf conf)) return;
            ViewInfo viewInfo = new ViewInfo();
            viewInfo.ViewId = viewId;
            viewInfo.ContextData = contextData;
            viewInfo.ViewType = viewType;
            showingList.Add(viewInfo);

            GameObject prefab;
            AssetHandle prefabHandle = default;
            if (conf.Package == "Resources")
            {
                prefab = Resources.Load<GameObject>(conf.Path);
            }
            else
            {
                prefabHandle = string.IsNullOrEmpty(conf.Package)
                    ? YooAssets.LoadAssetAsync<GameObject>(conf.Path)
                    : YooAssets.GetPackage(conf.Package).LoadAssetAsync<GameObject>(conf.Path);
                await prefabHandle.ToUniTask();
                prefab = prefabHandle.AssetObject as GameObject;
            }

            if (viewInfo.isClose) return;
            var g = Object.Instantiate(prefab,GetViewParent(viewType));
            if (prefabHandle is { IsValid: true })
                YooAssetHandleOwner.GetOrAdd(g).SetHandle("prefab", prefabHandle);
            var layer = LayerMask.NameToLayer("UI");
            SetLayerRecursively(g, layer);
            var rectTrans = g.GetComponent<RectTransform>();
            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchorMax = Vector2.one;
            rectTrans.localPosition = Vector2.zero;
            rectTrans.localScale = Vector3.one;
            rectTrans.sizeDelta = Vector3.zero;
            var viewBase = g.GetComponent<ViewBase>();
            viewBase.ShowView(viewId, contextData);
            viewInfo.ViewBase = viewBase;
            addGlobalButtonClick(g);
        }

        public void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }

        public bool IsShowing(string viewId)
        {
            return GetViewInfo(viewId) != null;
        }

        public ViewBase GetViewComp(string viewId)
        {
            var info = GetViewInfo(viewId);
            return info?.ViewBase;
        }

        public ViewInfo GetViewInfo(string viewId)
        {
            for (var i = 0; i < showingList.Count; i++)
            {
                var info = showingList[i];
                if (info.ViewId == viewId) return info;
            }

            return null;
        }


        private Transform GetViewParent(ViewType viewType)
        {
            if (viewParentDic.ContainsKey(viewType))
            {
                return viewParentDic[viewType];
            }

            switch (viewType)
            {
                case ViewType.Panel:
                    var panel = GameObject.Find("UI/Panel").transform;
                    viewParentDic.Add(viewType, panel);
                    return panel;

                case ViewType.PopUp:
                    var popUp = GameObject.Find("UI/PopUp").transform;
                    viewParentDic.Add(viewType, popUp);
                    return popUp;

                default:
                    var layer = GameObject.Find("UI/Layer").transform;
                    viewParentDic.Add(viewType, layer);
                    return layer;
            }
        }

        private void addGlobalButtonClick(GameObject obj)
        {
            // 为所有按钮添加全局点击事件
            Button[] allButtons = obj.transform.GetComponentsInChildren<Button>(true);
            foreach (Button btn in allButtons)
            {
                btn.onClick.AddListener(() => OnGlobalButtonClick(btn.gameObject));
            }
        }

        private void removeGlobalButtonClick(GameObject obj)
        {
            // 为所有按钮添加全局点击事件
            Button[] allButtons = obj.transform.GetComponentsInChildren<Button>(true);
            foreach (Button btn in allButtons)
            {
                btn.onClick.RemoveAllListeners();
            }
        }

        private void OnGlobalButtonClick(GameObject button)
        {
            SoundManager.Instance.PlaySound("Sounds_click");
            SoundManager.Instance.Vibrate(20, 50);
        }

        private GameObject _tipPrefab;
        public void Tip(string msg)
        {
            if (!viewPathDic.TryGetValue("MsgTip", out ViewConf conf)) return;
            if(_tipPrefab == null)
                _tipPrefab = Resources.Load<GameObject>(conf.Path);
            if (_tipPrefab == null) return;
            
            var g = Object.Instantiate(_tipPrefab,GetViewParent(ViewType.PopUp));
            var rectTrans = g.GetComponent<RectTransform>();
            rectTrans.anchorMin = Vector2.one * 0.5f;
            rectTrans.anchorMax = Vector2.one * 0.5f;
            rectTrans.localPosition = Vector2.zero;
            rectTrans.localScale = Vector3.one;
            Sequence sequence = DOTween.Sequence();
            sequence.SetLink(g, LinkBehaviour.KillOnDestroy);
            g.transform.Find("Text").GetComponent<Text>().text = msg;
            sequence.Append(g.transform.DOLocalMoveY(100, 1f));
            var canvasGroup = g.GetComponent<CanvasGroup>();
            var t = DOTween.To(() => canvasGroup.alpha, x => canvasGroup.alpha = x, 0, 0.2f);
            t.SetTarget(canvasGroup);
            sequence.Append(t);
            sequence.OnComplete(() =>
            {
                Object.Destroy(g);
            });
        }
    }
}