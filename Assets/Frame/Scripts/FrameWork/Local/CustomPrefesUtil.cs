public class CustomPrefesUtil
{
    private static CustomPrefes playPrefes = new CustomPrefes();

    public static int PlayLevel = 1;

    /// <summary>
    /// 存储全场景通用的数据
    /// </summary>
    /// <param name="key">存储key值，一旦存储将无法修改</param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    public static void SaveCommonData<T>(string key, T value)
    {
        SaveValue(key, value);
    }

    /// <summary>
    /// 获取全场景通用的数据
    /// </summary>
    /// <param name="key">存储key值，一旦存储将无法修改</param>
    /// <param name="defaultValue">默认值:如果不传，将指定为该类型的默认值</param>
    /// <typeparam name="T"></typeparam>
    public static T GetCommonData<T>(string key, T defaultValue = default(T))
    {
        return GetValue(key.ToString(), defaultValue);
    }

    /// <summary>
    /// 强制存储数据到磁盘
    /// </summary>
    public static void ForceSave()
    {
        playPrefes?.SaveToDisk(true);
    }

    public static void SaveValue<T>(string key, T value)
    {
        playPrefes.ReadyToSave(key, value);
    }

    public static T GetValue<T>(string key, T defaultValue)
    {
        return playPrefes.ReadyToGetValue(key, defaultValue);
    }

    public static void NormalSave(string key, string value)
    {
        playPrefes.NormalSave(key, value);
    }

    public static string NoramlGet(string key, string defaultValue)
    {
        return playPrefes.NoramlGet(key, defaultValue);
    }

    /// <summary>
    /// 二进制方式存储数据到磁盘文件（适合大数据量）
    /// </summary>
    public static void SaveBinary<T>(string key, T value)
    {
        playPrefes.SaveBinary(key, value);
    }

    /// <summary>
    /// 获取二进制方式存储的数据
    /// </summary>
    public static T GetBinary<T>(string key, T defaultValue = default(T))
    {
        return playPrefes.GetBinary(key, defaultValue);
    }

    /// <summary>
    /// 删除对应的二进制存档文件
    /// </summary>
    public static void DeleteBinary(string key)
    {
        playPrefes.DeleteBinary(key);
    }
}