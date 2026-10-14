using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlatformHelper
{
    static IPlatform platform;

    public static IPlatform Platform
    {
        get { return platform; }
    }


    public static void Init(string prodPkg)
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
#if UNITY_ANDROID
                platform = new AndroidPlatform(prodPkg);
#endif
                break;
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.LinuxEditor:
                platform = new EditorPlatform();
                break;
        }
    }

    public static bool IsTestPackage()
    {
        return false;
    }

    public static bool IsEditor()
    {
#if UNITY_EDITOR
        return true;
#else
                return false;
#endif
    }
}