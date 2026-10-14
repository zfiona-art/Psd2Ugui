using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System;

namespace FrameWork.Log
{
    public class Debugger
    {
        private static Dictionary<string, string> colorDic = new Dictionary<string, string>();

        // [Conditional("DEBUG")]
        public static void Log(object message, string logType = "default")
        {
#if UNITY_EDITOR || DEBUG
            Debug.Log(message);
#else
#endif
        }

        // [Conditional("DEBUG")]
        public static void TypeLog(string logType, object message)
        {
            //Editor平台用颜色区分
            if (Application.platform == RuntimePlatform.OSXEditor ||
                Application.platform == RuntimePlatform.WindowsEditor)
            {
                string colorStr = string.Empty;
                string logTypeStr = logType.ToString();
                if (colorDic.ContainsKey(logTypeStr))
                {
                    colorStr = colorDic[logTypeStr];
                }
                else
                {
                    byte[] colorByte = System.Text.Encoding.ASCII.GetBytes(logTypeStr);
                    var length = colorByte.Length;
                    for (int i = 0; i < 3 && i < length; i++)
                    {
                        colorStr += System.Convert.ToString(colorByte[i], 16);
                    }

                    colorDic[logTypeStr] = colorStr;
                }

                Debug.Log(string.Format("<color=#{0}>【{1}】{2}</color>", colorStr, logTypeStr, message));
            }
            else
            {
                string logTypeStr = logType.ToString();
                Debug.Log(string.Format("【{0}】{1}", logTypeStr, message));
            }
        }

        public static void LogError(object message)
        {
#if UNITY_EDITOR || DEBUG
            Debug.LogError(message);
#else
#endif
        }
    }
}