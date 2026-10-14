using System.Collections.Generic;
using FrameWork.Message;
using FrameWork.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UI
{
    public abstract class ItemBase : MonoBehaviour
    {
        #region 抽象方法以及虚方法
        protected virtual void OnItemInit() { }
        protected virtual void OnItemDestroy() { }
        protected virtual void OnItemUpdate() { }
        protected virtual void OnItemFixedUpdate() { }
        protected virtual void OnItemAwake() { }

        protected virtual List<string> AddMessage() { return messageList; }
        protected virtual void HandleMessage(MessageBody message) { }
        #endregion

        private bool isInitDone = false;
        void Awake()
        {
            OnItemAwake();
        }

        void Start()
        {
            OnItemInit();
            RegisterMessage();
            isInitDone = true;
            AddButtonsClick();
        }

        void Update()
        {
            if (gameObject.activeSelf && isInitDone)
            {
                OnItemUpdate();
            }
        }

        void OnDestroy()
        {
            RemoveMessage();
            OnItemDestroy();
            StopAllCoroutines();
            RemoveButtonsClick();
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

        private void AddButtonsClick()
        {
            // 为所有按钮添加全局点击事件
            Button[] allButtons = transform.GetComponentsInChildren<Button>();
            foreach (Button btn in allButtons)
            {
                btn.onClick.AddListener(OnGlobalButtonClick);
            }
        }

        private void RemoveButtonsClick()
        {
            // 为所有按钮添加全局点击事件
            Button[] allButtons = transform.GetComponentsInChildren<Button>();
            foreach (Button btn in allButtons)
            {
                btn.onClick.RemoveAllListeners();
            }
        }

        private void OnGlobalButtonClick()
        {
            _ = SoundManager.Instance.PlaySound("Sounds_click");
        }
    }
}
