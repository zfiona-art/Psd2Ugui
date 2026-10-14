using UnityEngine;
using System;
namespace FrameWork.Message
{
    public class MessageBody
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public GameObject sender;

        /// <summary>
        /// 消息内容,消息允许携带的参数
        /// </summary>
        public object[] param;

        ///<summary>
        /// 消息名
        ///</summary>
        public string name;

        public EventArgs Args
        {
            get
            {
                if (param != null && param.Length > 0)
                {
                    EventArgs args = param[0] as EventArgs;
                    if (args != null)
                    {
                        return args;
                    }

                }
                return EventArgs.Empty;
            }
        }

        /// <summary>
        /// 构造函数 （初始化）
        /// </summary>
        ///<param name="sender">通知发送者
        ///<param name="param">通知内容
        public MessageBody(GameObject sender, object[] param, string name)
        {
            this.sender = sender;
            this.param = param;
            this.name = name;
        }

        public MessageBody()
        {

        }
    }

}