
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;

public class AESUtils
{

    #region  加密
    /// <summary>
    /// 将明文进行对称加密成字符串
    /// </summary>
    /// <param name="plainText">需要加密的明文</param>
    /// <param name="keyStr">用于对称算法的机密密钥。 密钥大小必须为 128、192 或 256 位</param>
    /// <param name="IVStr">将用于对称算法的 IV</param>
    /// <returns></returns>
    public static string AESEncrtyptToStr(string plainText, string keyStr, string IVStr)
    {
        byte[] rgbKey = Encoding.UTF8.GetBytes(keyStr);
        byte[] rgbIV = Convert.FromBase64String(IVStr);
        return EncrtyptToStr(plainText, rgbKey, rgbIV);
    }


    /// <summary>
    /// 将明文进行对称加密成字符串
    /// </summary>
    /// <param name="plainText">需要加密的明文</param>
    /// <param name="keyStr">用于对称算法的机密密钥。 密钥大小必须为 128、192 或 256 位</param>
    /// <param name="IVStr">将用于对称算法的 IV</param>
    /// <returns></returns>
    public static string AESEncrtyptToStr(string plainText, byte[] key, byte[] iv)
    {
        return EncrtyptToStr(plainText, key, iv);
    }
    /// <summary>
    /// 将明文进行对称加密成字符串
    /// </summary>
    /// <param name="plainText">需要加密的明文</param>
    /// <param name="keyStr">用于对称算法的机密密钥。 密钥大小必须为 128、192 或 256 位</param>
    /// <param name="IVStr">将用于对称算法的 IV</param>
    /// <returns></returns>
    private static string EncrtyptToStr(string plainText, byte[] key, byte[] iv)
    {
        using (RijndaelManaged rijAlg = new RijndaelManaged
        {
            Padding = PaddingMode.PKCS7,
            Mode = CipherMode.CBC,
            KeySize = 128,
            BlockSize = 128
        })
        {
            rijAlg.Key = key;
            rijAlg.IV = iv;
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
            byte[] cipherBytes = encryptor.TransformFinalBlock(plainTextBytes, 0, plainText.Length);  
            return Convert.ToBase64String(cipherBytes);  
        }
    }
    #endregion

    #region 解密
    /// <summary>
    /// 将加密文件解密成字符串
    /// </summary>
    /// <param name="cipherText">加密后的密文</param>
    /// <param name="keyStr">用于对称算法的机密密钥。 密钥大小必须为 128、192 或 256 位</param>
    /// <param name="IVStr">将用于对称算法的 IV</param>
    /// <returns></returns>
    public static string AESDecryptToStr(string cipherText, string keyStr, string IVStr)
    {
        byte[] key = Encoding.UTF8.GetBytes(keyStr);
        byte[] iv = Convert.FromBase64String(IVStr);
        return DecryptToStr(cipherText, key, iv);
    }

    /// <summary>
    /// 将加密文件解密成字符串
    /// </summary>
    /// <param name="cipherText">加密后的密文</param>
    /// <param name="key">用于对称算法的机密密钥。 密钥大小必须为 128、192 或 256 位</param>
    /// <param name="iv">将用于对称算法的 IV</param>
    /// <returns></returns>
    public static string AESDecryptToStr(string cipherText, byte[] key, byte[] iv)
    {
        return DecryptToStr(cipherText, key, iv);
    }

    private static string DecryptToStr(string cipherText, byte[] key, byte[] iv)
    {
        var DecryptedString = "";
        try
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return "";
            }
            using (RijndaelManaged rijAlg = new RijndaelManaged
            {
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.CBC,
                KeySize = 128,
                BlockSize = 128
            })
            {
                rijAlg.Key = key;
                rijAlg.IV = iv;


                var encryptData = Convert.FromBase64String(cipherText);
                ICryptoTransform ct = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                var decryptedData = ct.TransformFinalBlock(encryptData, 0, encryptData.Length);
                DecryptedString = Encoding.UTF8.GetString(decryptedData);
            }
        }
        catch (Exception ex)
        {

        }

        return DecryptedString;
        // byte[] cipherByte = Convert.FromBase64String(cipherText);
        // DecryptionResultEntity decryptionResultEntity = DecryptToStr(cipherByte, key, iv);
        // return decryptionResultEntity.ResultOfString;
    }

    /// <summary>
    /// 将加密文件解密成字符串
    /// </summary>
    /// <param name="cipherText">加密后的密文</param>
    /// <param name="key">用于对称算法的机密密钥。 密钥大小必须为 128、192 或 256 位</param>
    /// <param name="iv">将用于对称算法的 IV</param>
    /// <returns></returns>
    // private static DecryptionResultEntity DecryptToStr(byte[] cipherByte, byte[] key, byte[] iv)
    // {
    //     DecryptionResultEntity decryptionResultEntity = null;
    //     using (RijndaelManaged rijAlg = new RijndaelManaged
    //     {
    //         Padding = PaddingMode.PKCS7,
    //         Mode = CipherMode.CBC,
    //         KeySize = 128,
    //         BlockSize = 128
    //     })
    //     {
    //         rijAlg.Key = key;
    //         rijAlg.IV = iv;

    //         // Create a decryptor to perform the stream transform.
    //         ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);



    //         Byte[] decryptedData = decryptor.TransformFinalBlock(cipherByte, 0, cipherByte.Length);
    //         // DecryptedString = Encoding.UTF8.GetString(decryptedData);

    //         // Create the streams used for decryption.
    //         using (MemoryStream msDecrypt = new MemoryStream(cipherByte))
    //         {
    //             using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
    //             {
    //                 using (StreamReader srDecrypt = new StreamReader(csDecrypt, Encoding.UTF8))
    //                 {
    //                     string plaintext = srDecrypt.ReadToEnd();
    //                     byte[] plaintextByteArray = System.Text.Encoding.UTF8.GetBytes(plaintext);
    //                     plaintext.TrimEnd(new char[1]);
    //                     decryptionResultEntity = new DecryptionResultEntity(plaintext, plaintextByteArray);
    //                 }
    //             }
    //         }
    //     }

    //     //剥离空格
    //     return decryptionResultEntity;
    // }
    #endregion



    #region 解密服务器返回的数据
    /**
     * 解密服务器返回的数据
     */
    // public static string DecryptDataFromNet(string inputBase64, string passphrase = null)
    // {
    //     byte[] key, iv;
    //     byte[] cipherData = Convert.FromBase64String(inputBase64);
    //     byte[] saltData = new byte[8];
    //     System.Buffer.BlockCopy(cipherData, 8, saltData, 0, 8);
    //     DeriveKeyAndIV(System.Text.Encoding.UTF8.GetBytes(passphrase), saltData, 1, out key, out iv);
    //     byte[] tempinput = Convert.FromBase64String(inputBase64);
    //     byte[] inputParams = new byte[tempinput.Length - 16];
    //     System.Buffer.BlockCopy(tempinput, 16, inputParams, 0, tempinput.Length - 16);
    //     DecryptionResultEntity decryptionResultEntity = DecryptToStr(inputParams, key, iv);
    //     return decryptionResultEntity.ResultOfString;
    // }

    //     private static void DeriveKeyAndIV(byte[] data, byte[] salt, int count, out byte[] key, out byte[] iv)
    //     {
    //         List<byte> hashList = new List<byte>();
    //         byte[] currentHash = new byte[0];

    //         int preHashLength = data.Length + ((salt != null) ? salt.Length : 0);
    //         byte[] preHash = new byte[preHashLength];

    //         System.Buffer.BlockCopy(data, 0, preHash, 0, data.Length);
    //         if (salt != null)
    //             System.Buffer.BlockCopy(salt, 0, preHash, data.Length, salt.Length);
    //         MD5 hash = MD5.Create();
    //         currentHash = hash.ComputeHash(preHash);

    //         for (int i = 1; i < count; i++)
    //         {
    //             currentHash = hash.ComputeHash(currentHash);
    //         }

    //         hashList.AddRange(currentHash);

    //         while (hashList.Count < 48) // for 32-byte key and 16-byte iv
    //         {
    //             preHashLength = currentHash.Length + data.Length + ((salt != null) ? salt.Length : 0);
    //             preHash = new byte[preHashLength];

    //             System.Buffer.BlockCopy(currentHash, 0, preHash, 0, currentHash.Length);
    //             System.Buffer.BlockCopy(data, 0, preHash, currentHash.Length, data.Length);
    //             if (salt != null)
    //                 System.Buffer.BlockCopy(salt, 0, preHash, currentHash.Length + data.Length, salt.Length);

    //             currentHash = hash.ComputeHash(preHash);

    //             for (int i = 1; i < count; i++)
    //             {
    //                 currentHash = hash.ComputeHash(currentHash);
    //             }

    //             hashList.AddRange(currentHash);
    //         }
    //         hash.Clear();
    //         key = new byte[32];
    //         iv = new byte[16];
    //         hashList.CopyTo(0, key, 0, 32);
    //         hashList.CopyTo(32, iv, 0, 16);
}
#endregion

// }

public class DecryptionResultEntity
{
    public string ResultOfString;
    public byte[] ResultOfByteArray;

    public DecryptionResultEntity(string ResultOfString, byte[] ResultOfByteArray)
    {
        this.ResultOfString = ResultOfString;
        this.ResultOfByteArray = ResultOfByteArray;
    }
}
