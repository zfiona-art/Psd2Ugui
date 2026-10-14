using System.Collections.Generic;
using FrameWork.Log;
using LitJson;

public class LogMgr : Singleton<LogMgr>
{

    public LogMgr()
    {

    }

    public void AddLog(string action, Dictionary<string, string> dic = null)
    {
        dic ??= new Dictionary<string, string>();
        var str = JsonMapper.ToJson(dic);
        if (!dic.ContainsKey("ver"))
        {
            dic.Add("ver", PlatformHelper.Platform.DeviceInfo().ver);
        }
        Debugger.TypeLog(DebuggerType.Log, "action: " + action + " dic: " + JsonMapper.ToJson(dic));
        PlatformHelper.Platform.SendOursEvent(action, str);
    }
}