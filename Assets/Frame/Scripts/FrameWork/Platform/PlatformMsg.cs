using System;
using System.Collections.Generic;
using FrameWork.UI;
using LitJson;
using UnityEngine;

public class PlatformMsg : MonoBehaviour
{
    private Dictionary<string, Action<AdInfo>> adCallBackDic = new();
    private static PlatformMsg instance;
    public static PlatformMsg Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        instance = this;
    }

    private void MsgFromNative(string msg)
    {
        Debug.Log("MsgFromNative: " + msg);
        Dictionary<string, string> dic = JsonMapper.ToObject<Dictionary<string, string>>(msg);
        if (dic.ContainsKey("name"))
        {
            var name = dic["name"];
            var data = dic["data"];
            switch (name)
            {
                case "gaid":
                    PlatformHelper.Platform.SetGaid(data);
                    break;
                case "ad_action":
                    var adInfo = JsonMapper.ToObject<AdInfo>(data);
                    onAdAction(adInfo);
                    break;
            }
        }
    }

    private void onAdAction(AdInfo adInfo)
    {
        switch (adInfo.adFormat)
        {
            case AdFormat.REWARD:
                {
                    switch (adInfo.status)
                    {
                        case AdStatus.Close:
                            if (adCallBackDic.ContainsKey(adInfo.adFormat))
                            {
                                adInfo.status = AdStatus.RewardUser;
                                adCallBackDic[adInfo.adFormat]?.Invoke(adInfo);
                                adCallBackDic.Remove(adInfo.adFormat);
                            }
                            break;
                        case AdStatus.ShowFailed:
                            if (adCallBackDic.ContainsKey(adInfo.adFormat))
                            {
                                adCallBackDic[adInfo.adFormat]?.Invoke(adInfo);
                                adCallBackDic.Remove(adInfo.adFormat);
                            }
                            UIManager.Instance.Tip(LocalizationUtil.GetLocalValue("ToastWatchADFailed"));
                            break;
                    }

                }
                break;
            case AdFormat.INTERSTITIAL:
                {
                    switch (adInfo.status)
                    {
                        case AdStatus.Close:
                            if (adCallBackDic.ContainsKey(adInfo.adFormat))
                            {
                                adInfo.status = AdStatus.RewardUser;
                                adCallBackDic[adInfo.adFormat]?.Invoke(adInfo);
                                adCallBackDic.Remove(adInfo.adFormat);
                            }
                            break;
                        case AdStatus.ShowFailed:
                            if (adCallBackDic.ContainsKey(adInfo.adFormat))
                            {
                                adCallBackDic[adInfo.adFormat]?.Invoke(adInfo);
                                adCallBackDic.Remove(adInfo.adFormat);
                            }
                            break;
                    }
                }
                break;
        }
    }

    public void AddAdEvent(string adFormat, Action<AdInfo> action = null)
    {
        if (action == null) return;
        adCallBackDic[adFormat] = action;
    }
}