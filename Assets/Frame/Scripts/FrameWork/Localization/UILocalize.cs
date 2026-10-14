using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[AddComponentMenu("UI/Localize")]
public class UILocalize : MonoBehaviour
{
    /// <summary>
    /// Localization key.
    /// </summary>
    public string key;

    public string value
    {
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                Text lbl = GetComponent<Text>();
                if (lbl != null)
                {
                    lbl.text = value;
                    action?.Invoke();
                }

                TextMeshPro text_mesh = GetComponent<TextMeshPro>();
                if (text_mesh != null)
                {
                    text_mesh.text = value;
                    action?.Invoke();
                }
            }
        }
    }

    public void SetKey(string key, Action ac = null)
    {
        this.key = key;
        action = ac;
        OnLocalize();
    }

    private Action action;

    bool mStarted = false;

    /// <summary>
    /// Localize the widget on enable, but only if it has been started already.
    /// </summary>
    void OnEnable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        if (mStarted) OnLocalize();
    }

    /// <summary>
    /// Localize the widget on start.
    /// </summary>
    void Start()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        mStarted = true;
        OnLocalize();
        Localization.onLocalize += OnLocalize;
    }

    /// <summary>
    /// This function is called by the Localization manager via a broadcast SendMessage.
    /// </summary>
    public void OnLocalize()
    {
        if (!string.IsNullOrEmpty(key))
        {
            Text lbl = GetComponent<Text>();
            TextMeshPro text_mesh = GetComponent<TextMeshPro>();
            if (Localization.Exists(key))
            {
                if (lbl != null)
                {
                    value = Localization.Get(key);
                }

                if (text_mesh != null)
                {
                    value = Localization.Get(key);
                }
            }
        }
    }

    public void OnChangeFont()
    {
        // UILabel lbl = GetComponent<UILabel>();
        // if (lbl != null) ChangeFont(lbl);
    }

    void OnDestroy()
    {
        Localization.onLocalize -= OnLocalize;
    }

    // private void ChangeFont(UILabel lbl)
    // {
    // 	lbl.trueTypeFont = ResourceUtil.LoadFont(Localization.language);
    // }
}