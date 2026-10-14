using System;
using System.Collections.Generic;

public interface IPlatform
{
    public DeviceInfo DeviceInfo();
    public string Did();
    public bool IsProd();
    public void SetGaid(string gaid);
    public string Platform();
    public string Uid();
    public void Vibrate(int duration, int amplitude);
    public Dictionary<string, string> GetCommonLogParamDic();
    public void SendAdjustEvent(string eventName, string jsonStr);
    public void SendFirebaseEvent(string eventName, string jsonStr);
    public void SendOursEvent(string eventName, string jsonStr);
    public void LoadAd(string adFormat);
    public bool IsAdReady(string adFormat, string placementName);
    public void PlayAd(string adFormat, string placementName, string ext, Action<AdInfo> act = null);
    public void CopyStr(string str);
    public void RateUs();
    public void SetCustomData(string json);
    public bool IsNotificationEnabled();
    public void OpenNotificationSettings();
    public string Token();
    public void Purchase(string productId, Action<bool> onComplete = null);
}
