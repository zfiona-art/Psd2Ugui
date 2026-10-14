using System.Collections.Generic;
using UnityEngine;
using System;

namespace FrameWork.Message
{
    public class MessageManager
    {
        public static MessageManager Instance { get { return Nested.instance; } }
        private class Nested
        {
            static Nested() { }
            internal static readonly MessageManager instance = new();
        }

        public delegate void MessageDelegate(MessageBody messagebody);
        /// 消息存放的容器
        private Dictionary<string, MessageDelegate> messageListeners = new Dictionary<string, MessageDelegate>();

        ///<summary>
        /// 添加注册事件监听，增加特定的委托
        ///</summary>
        ///<param name = "messageKey" >事件名</param>
        ///<param name = "listener" >委托</param>
        public void RegisterMessage(string messageKey, MessageDelegate listener)
        {
            if (!HasMessageListener(messageKey))
            {
                MessageDelegate del = null; //定义方法
                messageListeners[messageKey] = del;// 给委托变量赋值
            }
            messageListeners[messageKey] += listener; //注册接收者的监听
        }

        ///<summary>
        /// 移除注册事件监听,取消特定的委托
        ///</summary>
        ///<param name = "messageKey" >事件名</param>
        ///<param name = "listener" >委托</param>
        public void RemoveMessageListener(string messageKey, MessageDelegate listener)
        {
            if (!HasMessageListener(messageKey))
                return;
            messageListeners[messageKey] -= listener; //析构指定接收者的监听
            //如果该事件下没有委托，则移除该事件
            if (messageListeners[messageKey] == null)
            {
                RemoveMessageListener(messageKey);
            }
        }
        ///<summary>
        /// 移除注册事件监听,移除所有的委托
        ///</summary>
        private void RemoveMessageListener(string messageKey)
        {
            messageListeners.Remove(messageKey);
        }

        /// <summary>
        /// 分发事件，不需要知道接送者
        /// </summary>
        ///<param name="messageKey">事件Key
        ///<param name="param">封装后的通知内容
        public void SendMessage(string messageKey, params object[] param)
        {
            if (!HasMessageListener(messageKey))
                return;
            messageListeners[messageKey](new MessageBody(null, param, messageKey));
        }

        /// <summary>
        /// 分发事件，可拓展消息具体信息
        /// </summary>
        /// <param name="messageKey"></param>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void SendMessage(string messageKey, GameObject sender, EventArgs args)
        {
            if (!HasMessageListener(messageKey))
                return;
            messageListeners[messageKey](new MessageBody(sender, new object[] { args }, messageKey));
        }

        /// <summary>
        /// 是否存在指定事件的监听器
        /// </summary>
        public bool HasMessageListener(string eventKey)
        {
            return messageListeners.ContainsKey(eventKey);
        }
    }
}