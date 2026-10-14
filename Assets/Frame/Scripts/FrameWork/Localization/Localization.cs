using UnityEngine;
using System.Collections.Generic;
using LitJson;
using Cysharp.Threading.Tasks;
using YooAsset;

static public class Localization
{
	public delegate void OnLocalizeNotification();
	static public OnLocalizeNotification onLocalize;
	static public bool localizationHasBeenSet = false;
	static List<string> mLanguages = new List<string>() { "zh", "en", "in", "id", "pt" };
	static Dictionary<string, LanguageConfigItem> languageDic = new();
	static string mLanguage;
	public static Dictionary<string, LanguageConfigItem> LanguageDic
	{
		get
		{
			return languageDic;
		}
	}

	public static List<string> KnownLanguages
	{
		get
		{
			return mLanguages;
		}
	}

	public static string Language
	{
		get
		{
			return mLanguage;
		}
		set
		{
			if (mLanguage != value)
			{
				mLanguage = value;
				LoadAndSelect(value);
			}
		}
	}

	// 从resources 中初始化
	public static void Init(string lan, LanguagePathConfig conf)
	{
		mLanguages = conf.Supports;
		TextAsset asset = Resources.Load<TextAsset>(conf.ResourePath);
		if (asset != null)
		{
			var list = JsonMapper.ToObject<List<LanguageConfigItem>>(asset.text);
			foreach (var item in list)
			{
				languageDic.TryAdd(item.LanguageKey, item);
			}
		}
		Language = lan;
		localizationHasBeenSet = true;
	}

	// 增补多语言
	public static async UniTask Supplement(LanguagePathConfig conf)
	{
		TextAsset asset;
		if (conf.Package == "Resources")
		{
			asset = Resources.Load<TextAsset>(conf.PackagePath);
		}
		else
		{
			var handle = YooAssets.LoadAssetAsync<TextAsset>(conf.PackagePath);
			// Debug.Log("conf.PackagePath: " + conf.PackagePath);
			// var handle = string.IsNullOrEmpty(conf.Package) ?  : YooAssets.GetPackage(conf.Package).LoadAssetAsync<TextAsset>(conf.PackagePath);
			await handle.ToUniTask();
			asset = handle.AssetObject as TextAsset;
			handle.Release();
		}
		if (asset != null)
		{
			var list = JsonMapper.ToObject<List<LanguageConfigItem>>(asset.text);
			foreach (var item in list)
			{
				languageDic[item.LanguageKey] = item;
			}
		}
	}


	static bool LoadAndSelect(string language)
	{
		if (!string.IsNullOrEmpty(language))
		{
			if (languageDic.Count == 0) return false;
			if (SelectLanguage(language)) return true;
		}
		return false;
	}


	static bool HasLanguage(string languageName)
	{
		return mLanguages.Contains(languageName);
	}

	static bool SelectLanguage(string language)
	{
		if (mLanguages.Count == 0) return false;
		if (HasLanguage(language))
		{
			mLanguage = language;
			onLocalize?.Invoke();
			return true;
		}
		return false;
	}

	/// <summary>
	/// Localize the specified value.
	/// </summary>

	static public string Get(string key)
	{
		if (string.IsNullOrEmpty(key)) return "";
		if (!languageDic.ContainsKey(key)) return "";

		var item = languageDic[key];
        string v = mLanguage switch
        {
            "hi" => item.Hindi,
            "id" => item.Indonesian,
            "pt" => item.Portuguese,
            "th" => item.Thai,
            "vi" => item.Vietnamese,
            "es" => item.Spanish,
            "ms" => item.Malay,
            "ko" => item.Korean,
            "ja" => item.Japanese,
            "fr" => item.French,
            "de" => item.German,
            _ => item.English,
        };
        return v;
	}

	/// <summary>
	/// Localize the specified value and format it.
	/// </summary>

	static public string Format(string key, params object[] parameters) { return string.Format(Get(key), parameters); }


	static public string Localize(string key) { return Get(key); }

	/// <summary>
	/// Returns whether the specified key is present in the localization dictionary.
	/// </summary>

	static public bool Exists(string key)
	{
		return languageDic.ContainsKey(key);
	}

}
