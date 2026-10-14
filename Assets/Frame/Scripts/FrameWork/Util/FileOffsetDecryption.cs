using UnityEngine;
using YooAsset;


/// <summary>
/// 资源文件偏移解密类
/// </summary>
public class FileOffsetDecryption : IDecryptionServices
{
    /// <summary>
    /// 同步方式获取解密的资源包对象
    /// 注意：加载流对象在资源包对象释放的时候会自动释放
    /// </summary>
    DecryptResult IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo)
    {
        DecryptResult decryptResult = new DecryptResult();
        decryptResult.ManagedStream = null;
        ulong offset = (ulong)fileInfo.BundleName.Length;
        decryptResult.Result = AssetBundle.LoadFromFile(fileInfo.FileLoadPath, fileInfo.FileLoadCRC, offset);
        return decryptResult;
    }

    /// <summary>
    /// 异步方式获取解密的资源包对象
    /// 注意：加载流对象在资源包对象释放的时候会自动释放
    /// </summary>
    DecryptResult IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo)
    {
        DecryptResult decryptResult = new DecryptResult();
        decryptResult.ManagedStream = null;
          ulong offset = (ulong)fileInfo.BundleName.Length;
        decryptResult.CreateRequest = AssetBundle.LoadFromFileAsync(fileInfo.FileLoadPath, fileInfo.FileLoadCRC,offset);
        return decryptResult;
    }

    /// <summary>
    /// 获取解密的字节数据
    /// </summary>
    byte[] IDecryptionServices.ReadFileData(DecryptFileInfo fileInfo)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 获取解密的文本数据
    /// </summary>
    string IDecryptionServices.ReadFileText(DecryptFileInfo fileInfo)
    {
        throw new System.NotImplementedException();
    }
}