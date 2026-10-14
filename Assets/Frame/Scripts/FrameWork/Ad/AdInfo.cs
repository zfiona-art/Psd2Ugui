public enum AdStatus
{
    LoadFailed, // 加载失败
    LoadSuccess, // 加载成功
    ShowFailed, // 显示失败
    ShowSuccess, // 显示成功
    Click, // 点击广告
    Close, // 关闭广告
    RewardUser, // 奖励用户
    RevenuePaid, // 广告收入回调
    AdEnd,  // 展示结束
}

public class AdInfo
{
    public string adFormat;
    public string placementName;
    public AdStatus status;
}