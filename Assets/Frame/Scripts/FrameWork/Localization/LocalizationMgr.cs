using Cysharp.Threading.Tasks;
using FrameWork.Message;
public class LocalizationMgr : Singleton<LocalizationMgr>
{
    const string LANGUAGE_SAVE_KEY = "language";
    private string lan;
    public LanguagePathConfig Config;

    // 初始化多语言 从resource中读取
    public void Init(string systemLan, LanguagePathConfig configItem)
    {
        Config = configItem;
        if (systemLan == "in") // 安卓系统获取到的印尼语代码是in 兼容一下
            lan = "id";

        lan = CustomPrefesUtil.GetCommonData(LANGUAGE_SAVE_KEY, "");
        if (string.IsNullOrEmpty(lan))
        {
            lan = configItem.Supports.Contains(systemLan) ? systemLan : configItem.Default;
            CustomPrefesUtil.SaveCommonData(LANGUAGE_SAVE_KEY, lan);
        }
        Localization.Init(lan, configItem);
    }
    public async UniTask Supplement()  // 补充多语言
    {
        await Localization.Supplement(Config);
    }

    public void ChangeLan(string lan)
    {
        if (!Config.Supports.Contains(lan)) return;

        this.lan = lan;
        Localization.Language = lan;
        CustomPrefesUtil.SaveCommonData(LANGUAGE_SAVE_KEY, lan);

        MessageManager.Instance.SendMessage("MessageName.LanguageChange");
    }

    public string Lan
    {
        get { return lan; }
    }
}