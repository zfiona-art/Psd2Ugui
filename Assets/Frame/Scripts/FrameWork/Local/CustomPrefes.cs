using System;
using UnityEngine;
using LitJson;

public class CustomPrefes
{
    private static readonly string sKEY = "swg2AWSaFKO3PF20KSL7GXblQYZsNzH1";
    private static readonly string sIV = "6xkYwEFaw/PWeO76qY3osT==";

    public void ReadyToSave<T>(string key, T value)
    {
        if (value == null)
        {
            return;
        }

        object currentVal = value;
        string plainText;
        if (currentVal is bool)
        {
            plainText = (Convert.ToInt32(currentVal).ToString());
        }
        else if (IsComplexObject(value))
        {
            plainText = JsonMapper.ToJson(value);
        }
        else
        {
            plainText = Convert.ToString(currentVal);
        }

        Save(key, plainText);
    }

    public T ReadyToGetValue<T>(string key, T defaultValue)
    {
        object value = defaultValue;
        string plainText;
        if (value is bool)
        {
            plainText = GetValue(key, Convert.ToInt32(value).ToString());
        }
        else
        {
            plainText = GetValue(key, Convert.ToString(value));
        }

        if (defaultValue == null || !IsComplexObject(defaultValue) || value is bool)
        {
            object valueObj = null;
            if (value is double)
            {
                valueObj = double.Parse(plainText);
            }
            else if (value is long)
            {
                valueObj = long.Parse(plainText);
            }
            else if (value is int)
            {
                valueObj = int.Parse(plainText);
            }
            else if (value is bool)
            {
                valueObj = int.Parse(plainText) == 1;
            }
            else if (value is float)
            {
                valueObj = float.Parse(plainText);
            }
            else if (value is string)
            {
                valueObj = plainText;
            }

            return (T)valueObj;
        }
        else
        {
            try
            {
                return JsonMapper.ToObject<T>(plainText);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }

    private bool IsComplexObject<T>(T value)
    {
        bool result = true;
        if (value is int || value is float || value is string || value is double || value is long)
        {
            result = false;
        }

        return result;
    }

    private string MD5TheKey(string key)
    {
        return MD5Utils.Md5Encript(key);
    }

    /// <summary>
    /// 存储方法,key采用MD5加密，value采用AES加密
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    private void Save(string key, string value)
    {
        PlayerPrefs.SetString(MD5TheKey(key), AESUtils.AESEncrtyptToStr(value, sKEY, sIV));
        SaveToDisk();
    }

    /// <summary>
    /// 获取方法，value采用AES解密
    /// </summary>
    /// <param name="key"></param>
    /// <param name="strDefaultValue"></param>
    /// <returns>解密后的原文</returns>
    private string GetValue(string key, string strDefaultValue)
    {
        var value = PlayerPrefs.GetString(MD5TheKey(key), strDefaultValue);
        if (value == strDefaultValue) return strDefaultValue; //返回默认值
        var plainText =
            AESUtils.AESDecryptToStr(value, sKEY,
                sIV); //解密后的Value                                                           
        return plainText;
    }

    private float lastSaveTime;

    /// <summary>
    /// 将数据存储到磁盘，防止数据丢失
    /// </summary>
    /// <param name="forceSave"> 强制存储 </param>
    public void SaveToDisk(bool forceSave = false)
    {
        if (forceSave || Time.time - lastSaveTime > 60f)
        {
            lastSaveTime = Time.time;
            PlayerPrefs.Save();
        }
    }

    public void NormalSave(string key, string value)
    {
        PlayerPrefs.SetString(MD5TheKey(key), value);
        SaveToDisk();
    }

    public string NoramlGet(string key, string defaultValue)
    {
        var v = PlayerPrefs.GetString(MD5TheKey(key));
        if (string.IsNullOrEmpty(v)) return defaultValue;
        return v;
    }

    /// <summary>
    /// 二进制存储接口：将对象直接以二进制形式写入磁盘文件，适合大数据量优化。
    /// 注意：被存储的类（及其成员类）必须标记为 [System.Serializable]
    /// </summary>
    public void SaveBinary<T>(string key, T value)
    {
        if (value == null) return;
        try
        {
            string filePath = GetBinaryFilePath(key);
            using (var fs = System.IO.File.Open(filePath, System.IO.FileMode.Create))
            {
                var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bf.Serialize(fs, value);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"SaveBinary 失败 (Key: {key}): " + e.Message);
        }
    }

    /// <summary>
    /// 二进制读取接口
    /// </summary>
    public T GetBinary<T>(string key, T defaultValue)
    {
        string filePath = GetBinaryFilePath(key);
        if (!System.IO.File.Exists(filePath)) return defaultValue;
        try
        {
            using (var fs = System.IO.File.Open(filePath, System.IO.FileMode.Open))
            {
                var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)bf.Deserialize(fs);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"GetBinary 失败 (Key: {key}): " + e.Message);
            return defaultValue;
        }
    }

    /// <summary>
    /// 删除二进制存储文件
    /// </summary>
    public void DeleteBinary(string key)
    {
        string filePath = GetBinaryFilePath(key);
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
    }

    private string GetBinaryFilePath(string key)
    {
        // 存储在设备的 persistentDataPath 目录下，文件名使用 MD5 加密
        return System.IO.Path.Combine(Application.persistentDataPath, MD5TheKey(key) + ".bin");
    }
}