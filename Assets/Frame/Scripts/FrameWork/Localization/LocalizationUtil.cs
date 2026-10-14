using System;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationUtil
{
    ///<summary>
    ///将label本地化
    ///</summary>
    public static void SetLocalize(Text _text, string key)
    {
        _text.text = GetLocalValue(key);
    }

    ///<summary>
    ///localize本地化
    ///</summary>
    public static void SetLocalize(UILocalize localize, string key)
    {
        Text uILabel = localize.GetComponent<Text>();
        if (uILabel != null)
        {
            SetLocalize(uILabel, key);
        }
    }

    /// <summary>
    /// 为label 赋值
    /// </summary>
    /// <param name="label"></param>
    /// <param name="value"></param>
    public static void SetLabelValue(Text label, string value)
    {
        label.text = value;
    }

    ///<summary>
    ///传入 key,获得本地化值, 给 label赋值
    ///</summary>
    public static void SetLabel(string key, Text uILabel, string value1, string value2 = null, string value3 = null)
    {
        string localValue = Localization.Get(key);
        if (!string.IsNullOrEmpty(localValue) && localValue.Contains("{"))
        {
            uILabel.text = string.Format(localValue, value1, value2, value3);
        }
        else
        {
            uILabel.text = localValue;
        }
    }

    ///<summary>
    ///获得label的 key,获得本地化数据，赋值
    ///</summary>
    public static void SetLabel(Text uILabel, string value1, string value2 = null, string value3 = null, string value4 = null)
    {
        string localValue = GetValue(uILabel.gameObject);
        if (!string.IsNullOrEmpty(localValue) && localValue.Contains("{"))
        {
            uILabel.text = string.Format(localValue, value1, value2, value3, value4);
        }
        else
        {
            uILabel.text = localValue;
        }
    }

    public static string GetLocalValue(Text uILabel, string value1)
    {
        string localValue = GetValue(uILabel.gameObject);
        if (!string.IsNullOrEmpty(localValue) && localValue.Contains("{"))
        {
            return string.Format(localValue, value1);
        }
        return localValue;
    }

    /// <summary>
    /// 获取本地化数据
    /// </summary>
    /// <param name="key"> local key</param>
    /// <param name="value1">传入值，用作拼接字符串</param>
    public static string GetLocalValue(string key, string value1)
    {
        string localValue = GetLocalValue(key);
        if (!string.IsNullOrEmpty(localValue) && localValue.Contains("{"))
        {
            return string.Format(localValue, value1);
        }
        return localValue;
    }

    public static string GetLocalValue(string key, string value1, string value2)
    {
        string localValue = GetLocalValue(key);
        if (!string.IsNullOrEmpty(localValue) && localValue.Contains("{"))
        {
            return string.Format(localValue, value1, value2);
        }
        return localValue;
    }

    ///<summary>
    ///获取本地化数据
    ///</summary>
    public static string GetLocalValue(string key)
    {
        return Localization.Get(key);
    }

    public static string GetFillValue(string key)
    {
        string v = Localization.Get(key);
        if (!String.IsNullOrEmpty(v))
            return v;
        return key;
    }

    private static string GetValue(GameObject objt)
    {
        UILocalize uILocalize = objt.GetComponent<UILocalize>();
        if (uILocalize != null) return Localization.Get(uILocalize.key);
        return null;
    }
}

