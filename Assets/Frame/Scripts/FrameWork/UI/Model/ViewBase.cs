using System;
using System.Collections.Generic;
using DG.Tweening;
using FrameWork.Message;
using UnityEngine;
using UnityEngine.Serialization;

namespace FrameWork.UI
{
    public abstract class ViewBase : MonoBehaviour
    {
        private Transform _content;
        [SerializeField] protected AnimType animType = AnimType.None;
        #region 抽象方法以及虚方法
        protected string viewId;
        protected virtual void BindComponents() { }
        protected virtual void OnViewAwake() { }
        protected virtual void OnStartAnimComplete() { }
        protected virtual void OnViewStart() { }
        protected virtual void OnShowView(ViewContextData data) { }
        protected virtual void OnViewDestroy() { }
        protected virtual void OnViewUpdate() { }
        protected virtual void OnViewFixedUpdate() { }
        protected virtual List<string> AddMessage() { return messageList; }
        protected virtual void HandleMessage(MessageBody message) { }
        #endregion

        private bool isInitDone = false;
        void Awake()
        {
            _content = transform.Find("content");
            if (_content == null) _content = transform;
            //BindComponents();
            OnViewAwake();
        }

        void Start()
        {
            OnViewStart();
        }

        public virtual void ShowView(string view, ViewContextData data)
        {
            viewId = view;
            StartAnim();
            OnShowView(data);
            RegisterMessage();
            isInitDone = true;
        }

        void Update()
        {
            if (isInitDone && gameObject.activeSelf)
            {
                OnViewUpdate();
            }
        }

        void FixedUpdate()
        {
            if (isInitDone && gameObject.activeSelf)
            {
                OnViewFixedUpdate();
            }
        }

        protected virtual void CloseView()
        {
            UIManager.Instance.CloseView(viewId);
        }

        protected virtual void OnClickCloseBtn()
        {
            CloseView();
        }


        void OnDestroy()
        {
            RemoveMessage();
            OnViewDestroy();
            StopAllCoroutines();
        }

        protected List<string> messageList = new List<string>();
        private void RegisterMessage()
        {
            AddMessage();
            foreach (var key in messageList)
            {
                MessageManager.Instance.RegisterMessage(key, HandleMessage);
            }
        }

        private void RemoveMessage()
        {
            foreach (var key in messageList)
            {
                MessageManager.Instance.RemoveMessageListener(key, HandleMessage);
            }
        }

        private void StartAnim()
        {
            if (animType != AnimType.None && _content != null)
            {
                if (animType == AnimType.Scale)
                {
                    _content.transform.localScale = Vector3.zero;
                    var seq = DOTween.Sequence();
                    seq.Append(_content.transform.DOScale(1.2f, 0.2f).SetEase(Ease.InOutSine));
                    seq.Append(_content.transform.DOScale(1, 0.1f));
                    seq.OnComplete(OnStartAnimComplete);
                    seq.Play();

                }
                else
                {
                    _content.transform.localPosition = new Vector3(0, -transform.parent.GetComponent<RectTransform>().sizeDelta.y / 2, 0);
                    var seq = DOTween.Sequence();
                    seq.Append(_content.transform.DOLocalMoveY(100f, 0.2f).SetEase(Ease.InOutSine));
                    seq.Append(_content.transform.DOLocalMoveY(0, 0.1f));
                    seq.OnComplete(OnStartAnimComplete);
                    seq.Play();
                }

            }
            else
            {
                OnStartAnimComplete();
            }
        }

        public void CloseAnim(Action onComplete)
        {
            if (animType != AnimType.None && _content != null)
            {
                if (animType == AnimType.Scale)
                {

                    var seq = DOTween.Sequence();
                    seq.Append(_content.transform.DOScale(1.2f, 0.1f).SetEase(Ease.InOutSine));
                    seq.Append(_content.transform.DOScale(0f, 0.2f));
                    seq.OnComplete(() =>
                    {
                        onComplete?.Invoke();
                    });
                    seq.Play();
                }
                else
                {
                    // _content.transform.localPosition = new Vector3(0, -1624, 0);
                    var seq = DOTween.Sequence();
                    seq.Append(_content.transform.DOLocalMoveY(100f, 0.1f).SetEase(Ease.InOutSine));
                    seq.Append(_content.transform.DOLocalMoveY(-transform.parent.GetComponent<RectTransform>().sizeDelta.y / 2, 0.2f).SetEase(Ease.InOutSine));
                    seq.OnComplete(() =>
                   {
                       onComplete?.Invoke();
                   });
                    // seq.Append(_content.transform.DOScale(1, 0.1f));
                    seq.Play();
                }
            }
            else
            {
                onComplete?.Invoke();
            }

        }
    }
}
