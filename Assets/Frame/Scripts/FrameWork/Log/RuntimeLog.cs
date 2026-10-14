using UnityEngine;

public static class RuntimeLog
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public const bool Enabled = true;
#else
    public const bool Enabled = false;
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void ConfigureUnityLogger()
    {
        Debug.Log("LogMode:" + Enabled);
        Debug.unityLogger.logEnabled = Enabled;
    }
}
