using System;
using System.Collections.Generic;
using GameConfig.Network;
using UnityEngine;
using LitJson;

public class EditorPlatform : IPlatform
{
    private DeviceInfo deviceInfo = new DeviceInfo();

    Dictionary<string, string> paramDic = new Dictionary<string, string>(){
        {"mod","NOH-AN00"},
        {"sys_vc","31"},
        {"isp","46000"},
        {"has_sim_card","1"},
        {"pkg","com.arrow.adventure"},
        {"sid","d710fb7e-3dd2-457a-bf7c-862761e443e6"},
        {"uid","7803bc138ed0b1c1"},
        {"brd","HUAWEI"},
        {"verc","3"},
        {"net","wifi"},
        {"anm","page"},
        {"log_id","e3f37a89-a11e-46c4-b5d5-c3d4c6ec2e37"},
        {"ver","1.0.1"},
        {"os","31"},
        {"net_mcc_mnc","46000"},
        {"sim_mcc_mnc","46007"},
        {"bucket","51"},
        {"slan","zh_CN_#Hans"},
        {"pf","android"},
        {"sys_vn","12"},
        {"subanm","sealife"},
        {"aid","7803bc138ed0b1c13"},
        {"logtime","1709540773764"},
        {"did",""},
        {"lan","en"},//en,pt(葡萄牙语),id(印尼语)
        {"cou","US"},//US,BR(巴西),ID(印度尼西亚)
    };

    public EditorPlatform()
    {
        deviceInfo = JsonMapper.ToObject<DeviceInfo>(JsonMapper.ToJson(paramDic));
        Debug.Log(JsonMapper.ToJson(deviceInfo));
    }

    public DeviceInfo DeviceInfo()
    {
        return deviceInfo;
    }


    public string Did()
    {
        if (!string.IsNullOrEmpty(deviceInfo.gaid)) return deviceInfo.gaid;
        return deviceInfo.aid;
    }

    public string Uid()
    {
        return deviceInfo.aid;
    }

    public bool IsProd()
    {
        return false;
    }

    public void SetGaid(string gaid)
    {
        deviceInfo.gaid = gaid;
    }

    public string Platform()
    {
        return "editor";
    }

    public void Vibrate(int duration, int amplitude)
    {

    }


    public Dictionary<string, string> GetCommonLogParamDic()
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic["pkg"] = deviceInfo.pkg;
        dic["verc"] = deviceInfo.verc;
        dic["ver"] = deviceInfo.ver;
        dic["pf"] = "android";
        dic["did"] = Did();
        dic["aid"] = deviceInfo.aid;
        dic["brd"] = deviceInfo.brd;
        dic["mod"] = deviceInfo.mod;
        dic["cou"] = deviceInfo.cou;
        dic["slan"] = deviceInfo.slan;
        dic["has_sim_card"] = deviceInfo.has_sim_card;
        dic["sim_mcc_mnc"] = deviceInfo.sim_mcc_mnc;
        dic["net_mcc_mnc"] = deviceInfo.net_mcc_mnc;
        dic["isp"] = deviceInfo.isp;
        dic["net"] = deviceInfo.net;
        dic["sys_vc"] = deviceInfo.sys_vc;
        dic["sys_vn"] = deviceInfo.sys_vn;
        dic["sid"] = deviceInfo.sid;
        dic["bucket"] = deviceInfo.bucket;
        dic["os"] = deviceInfo.os;
        dic["abslot"] = deviceInfo.bucket;
        dic["idfa"] = "1";
        dic["idfv"] = "0";
        dic["gaid"] = deviceInfo.gaid;
        dic["logtime"] = TimeUtil.GetCurrentUTCMilliSeconds().ToString();
        dic["log_id"] = System.Guid.NewGuid().ToString();
        return dic;
    }

    public void SendAdjustEvent(string eventName, string jsonStr)
    {

    }
    public void SendFirebaseEvent(string eventName, string jsonStr)
    {

    }
    public void SendOursEvent(string eventName, string jsonStr)
    {

    }
    public void LoadAd(string adFormat)
    {

    }
    public bool IsAdReady(string adFormat, string placementName)
    {
        return true;
    }

    public void PlayAd(string adFormat, string placementName, string ext, Action<AdInfo> act = null)
    {
        var adInfo = new AdInfo()
        {
            adFormat = adFormat,
            placementName = placementName,
            status = AdStatus.RewardUser,
        };
        act?.Invoke(adInfo);
    }
    public void CopyStr(string str)
    {
        GUIUtility.systemCopyBuffer = str;
    }
    public void RateUs()
    {

    }

    public void SetCustomData(string json)
    {

    }

    public bool IsNotificationEnabled()
    {
        return true;
    }

    public void OpenNotificationSettings()
    {
    }
    

    public void Purchase(string productId, Action<bool> onComplete = null)
    {
        onComplete?.Invoke(true);
    }

    public string Token()
    {
        var str = deviceInfo.uid + ";" + deviceInfo.cou + ";" + deviceInfo.pkg + ";" + deviceInfo.ver;
        return AESUtils.AESEncrtyptToStr(str, NetworkConfigLoader.Instance.GetEnBytes(),
            NetworkConfigLoader.Instance.GetEnBytes());
    }
}
