using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

public class StringUtil
{
    public static bool IsValidCPF(string v)
    {
        if (string.IsNullOrEmpty(v))
            return false;

        return Regex.IsMatch(v, @"^\d{11}$") || Regex.IsMatch(v, @"^\d{3}\.\d{3}\.\d{3}-\d{2}$");
    }

    public static bool IsValidCNPJ(string v)
    {
        if (string.IsNullOrEmpty(v))
            return false;

        return Regex.IsMatch(v, @"^\d{14}$") || Regex.IsMatch(v, @"^\d{2}\.\d{3}\.\d{3}\/\d{4}\-\d{2}$");
    }

    public static string GenerateSecureHexString()
    {
        byte[] bytes = new byte[32];
        RandomNumberGenerator.Create().GetBytes(bytes);
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

    public static string FormatMoney(long num, int fixedNum = 1)
    {
        if (num < 1000)
        {
            return num.ToString();
        }
        else
        {
            string str = "";
            string format = "F" + fixedNum; // 构建格式化字符串，例如 "F2"

            if (num >= 1000 && num < 1000000)
            {
                str = (num / 1000f).ToString(format) + "K";
            }
            else if (num >= 1000000 && num < 1000000000) // 注意这里原逻辑可能是 1000000
            {
                str = (num / 1000000f).ToString(format) + "M";
            }
            else
            {
                str = (num / 1000000000f).ToString(format) + "B";
            }

            return str;
        }
    }
}