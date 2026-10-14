using System.Security.Cryptography;
using System.Text;
public class MD5Utils
{
    /// <summary>
    /// MD5加密
    /// </summary>
    /// <param name="plainText">原明文文件</param>
    /// <returns></returns>
    public static string Md5Encript(string plainText)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] array = md5.ComputeHash(Encoding.UTF8.GetBytes(plainText));
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < array.Length; i++)
        {
            stringBuilder.Append(array[i].ToString("x2"));
        }
        return stringBuilder.ToString();
    }
}