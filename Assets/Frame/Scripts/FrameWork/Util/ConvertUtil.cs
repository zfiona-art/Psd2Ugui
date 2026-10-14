
using System.Globalization;

using System;
public class ConvertUtil
{
    public static long Str2Long(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return 0;
        }
        return Convert.ToInt64(str, CultureInfo.InvariantCulture);
    }

    public static int Str2Int(string str)
    {
        return Convert.ToInt32(str, CultureInfo.InvariantCulture);
    }

    public static double Str2Double(string str)
    {
        return Convert.ToDouble(str, CultureInfo.InvariantCulture);
    }
    public static float Str2Float(string str)
    {
        return (float)Convert.ToDouble(str, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// string to enum
    /// </summary>
    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public static bool Str2Bool(string str)
    {
        if (string.IsNullOrEmpty(str)) return false;
        return Convert.ToBoolean(str, CultureInfo.InvariantCulture);
    }


    /// <summary>
    /// 敏感字符检测
    /// </summary>
    /// <param name="content">欲过滤的内容</param>
    /// <param name="sensitive_word">敏感词组</param>
    /// <param name="result_str">执行过滤之后的内容</param>
    /// <param name="filter_deep">检测深度，即sensitive_word数组中的每个词中的插入几个字以内会被过滤掉</param>
    /// <param name="replace_str">将检测到的敏感字替换成的字符</param>
    /// <returns></returns>
    static public bool Filter(string content, string[] sensitive_word, out string result_str, int filter_deep = 1, string replace_str = "*")
    {
        string result = content;
        result_str = result;

        if (sensitive_word == null) return false;

        bool isSensitive = false;
        foreach (string str in sensitive_word)
        {
            string s = str.Replace(replace_str, "");
            if (s.Length == 0) continue;
            bool bFiltered = true;
            while (bFiltered)
            {
                int result_index_start = -1;
                int result_index_end = -1;
                int idx = 0;
                while (idx < s.Length)
                {
                    string one_s = s.Substring(idx, 1);
                    if (one_s == replace_str)
                    {
                        continue;
                    }
                    if (result_index_end + 1 >= result.Length)
                    {
                        bFiltered = false;
                        break;
                    }
                    int new_index = result.IndexOf(one_s, result_index_end + 1, StringComparison.OrdinalIgnoreCase);
                    if (new_index == -1)
                    {
                        bFiltered = false;
                        break;
                    }

                    if (isCHS(one_s.ToCharArray()[0])) filter_deep = 1;
                    else filter_deep = 0;

                    if (idx > 0 && new_index - result_index_end > filter_deep + 1)
                    {
                        bFiltered = false;
                        break;
                    }
                    result_index_end = new_index;

                    if (result_index_start == -1)
                    {
                        result_index_start = new_index;
                    }
                    idx++;
                }

                if (bFiltered)
                {
                    isSensitive = true;
                    string result_left = result.Substring(0, result_index_start);
                    for (int i = result_index_start; i <= result_index_end; i++)
                    {
                        result_left += replace_str;
                    }
                    string result_right = result.Substring(result_index_end + 1);
                    result = result_left + result_right;
                }
            }
        }
        result_str = result;
        return isSensitive;
    }

    static private bool isCHS(char character)
    {
        // 中文表意字符的范围 4E00-9FA5
        int charVal = (int)character;
        return (charVal >= 0x4e00 && charVal <= 0x9fa5);
    }
}