using System.Collections.Generic;

public class LanguageConfigItem
{
    public string LanguageKey;
    public string Chinese;
    public string English;
    public string Hindi;
    public string Indonesian;
    public string Portuguese;
    public string Spanish;
    public string French;
    public string German;
    public string Japanese;
    public string Korean;
    public string Malay;
    public string Thai;
    public string Vietnamese;
}

public class LanguagePathConfig
{
    public List<string> Supports = new List<string>()
        { "en", "hi", "id", "pt", "th", "vi", "es", "ms", "ko", "ja", "fr", "de" };

    public string Default = "en";
    public string ResourePath = "Config/LanguageConfig";
    public string Package = "Resources";
    public string PackagePath = "Config_LanguageConfig";
}