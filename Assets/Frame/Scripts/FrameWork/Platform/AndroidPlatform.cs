using UnityEngine;
using LitJson;
using System.Collections.Generic;
using System.Globalization;
using System;
using GameConfig.Network;

#if UNITY_ANDROID
public class AndroidPlatform : IPlatform
{
    private const string PostNotificationsPermission = "android.permission.POST_NOTIFICATIONS";
    private const int AndroidApiNougat = 24;
    private const int AndroidApiTiramisu = 33;

    private AndroidJavaObject androidJavaApi;
    private DeviceInfo deviceInfo = new DeviceInfo();
    private string prodPkg;

    public AndroidPlatform(string prodPkg)
    {
        RegionInfo region = RegionInfo.CurrentRegion;

        var did = SystemInfo.deviceUniqueIdentifier;
        deviceInfo.pkg = Application.identifier;
        deviceInfo.brd = SystemInfo.deviceName;
        deviceInfo.mod = SystemInfo.deviceModel;
        deviceInfo.cou = region.TwoLetterISORegionName;
        deviceInfo.lan = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant(); 
        deviceInfo.ver = Application.version;
        deviceInfo.aid = did.Replace("-", "");
        deviceInfo.gaid = did;
        deviceInfo.uid = deviceInfo.aid;
        deviceInfo.os = SystemInfo.operatingSystem;
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
        return deviceInfo.uid;
    }

    public bool IsProd()
    {
        return deviceInfo.pkg == prodPkg;
    }

    public void SetGaid(string gaid)
    {
        deviceInfo.gaid = gaid;
        CustomPrefesUtil.SaveCommonData("gaid", gaid);
    }

    public string Platform()
    {
        return "android";
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
        dic["logtime"] = TimeUtil.GetCurrentUTCMilliSeconds().ToString(CultureInfo.InvariantCulture);
        dic["log_id"] = System.Guid.NewGuid().ToString();
        return dic;
    }

    public void SendOursEvent(string eventName, string jsonStr)
    {
        
    }

    public void SendAdjustEvent(string eventName, string jsonStr)
    {
        
    }

    public void SendFirebaseEvent(string eventName, string jsonStr)
    {
        
    }
    public void SetCustomData(string json)
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
        
    }

    public void CopyStr(string str)
    {
        try
        {
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var clipboard = activity.Call<AndroidJavaObject>("getSystemService", "clipboard");
            var clipData = new AndroidJavaClass("android.content.ClipData")
                .CallStatic<AndroidJavaObject>("newPlainText", "label", str);
            clipboard.Call("setPrimaryClip", clipData);
        }
        catch
        {
        }
    }

    public void RateUs()
    {
        try
        {
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var uri = new AndroidJavaClass("android.net.Uri")
                .CallStatic<AndroidJavaObject>("parse", "market://details?id=" + Application.identifier);
            var intent = new AndroidJavaObject("android.content.Intent", "android.intent.action.VIEW", uri);
            intent.Call<AndroidJavaObject>("setPackage", "com.android.vending");
            activity.Call("startActivity", intent);
        }
        catch
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
        }
    }
    
    public bool IsNotificationEnabled()
    {
        try
        {
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var version = new AndroidJavaClass("android.os.Build$VERSION");
            var sdkInt = version.GetStatic<int>("SDK_INT");

            // Android 13+ requires POST_NOTIFICATIONS to be granted at runtime.
            if (sdkInt >= AndroidApiTiramisu &&
                !UnityEngine.Android.Permission.HasUserAuthorizedPermission(PostNotificationsPermission))
            {
                return false;
            }

            // This framework API is available from Android 7.0 and also detects
            // notifications disabled from the application's system settings.
            if (sdkInt >= AndroidApiNougat)
            {
                var notificationManager = activity.Call<AndroidJavaObject>("getSystemService", "notification");
                return notificationManager != null && notificationManager.Call<bool>("areNotificationsEnabled");
            }

            // Older Android versions do not expose a public app-level check.
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Failed to check Android notification permission: {exception}");
            return false;
        }
    }

    public void OpenNotificationSettings()
    {
        try
        {
            var version = new AndroidJavaClass("android.os.Build$VERSION");
            var sdkInt = version.GetStatic<int>("SDK_INT");

            // Request the runtime permission first. After it has been granted,
            // subsequent clicks open the app's notification settings.
            if (sdkInt >= AndroidApiTiramisu &&
                !UnityEngine.Android.Permission.HasUserAuthorizedPermission(PostNotificationsPermission))
            {
                var callbacks = new UnityEngine.Android.PermissionCallbacks();
                callbacks.PermissionDeniedAndDontAskAgain += _ => OpenSystemNotificationSettings();
                UnityEngine.Android.Permission.RequestUserPermission(PostNotificationsPermission, callbacks);
                return;
            }

            OpenSystemNotificationSettings();
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Failed to open Android notification settings: {exception}");
            Application.OpenURL("package:" + Application.identifier);
        }
    }

    private void OpenSystemNotificationSettings()
    {
        try
        {
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var context = activity.Call<AndroidJavaObject>("getApplicationContext");
            var pkgName = context.Call<string>("getPackageName");
            var intent = new AndroidJavaObject("android.content.Intent", "android.settings.APP_NOTIFICATION_SETTINGS");
            intent.Call<AndroidJavaObject>("putExtra", "android.provider.extra.APP_PACKAGE", pkgName);
            intent.Call<AndroidJavaObject>("putExtra", "app_package", pkgName);
            activity.Call("startActivity", intent);
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Failed to open Android notification settings: {exception}");
            Application.OpenURL("package:" + Application.identifier);
        }
    }


    public void Purchase(string productId, Action<bool> onComplete = null)
    {
        
    }

    public string Token()
    {
        var str = deviceInfo.uid + ";" + deviceInfo.cou + ";" + deviceInfo.pkg + ";" + deviceInfo.ver;
        return AESUtils.AESEncrtyptToStr(str, NetworkConfigLoader.Instance.GetEnBytes(),
            NetworkConfigLoader.Instance.GetEnBytes());
    }
}
#endif
