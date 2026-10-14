#if UNITY_EDITOR
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerPrefsManager : EditorWindow
{
    [MenuItem("Tools/PlayerPrefs/清除所有PlayerPrefs")]
    public static void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("所有PlayerPrefs已清除！");
    }

    [MenuItem("Tools/PlayerPrefs/打开PlayerPrefs窗口")]
    public static void ShowWindow()
    {
        GetWindow<PlayerPrefsManager>("PlayerPrefs Manager");
    }

    private string keyToDelete = "";
    private string keyToAdd = "";
    private string valueToAdd = "";

    // 查看所有Key相关变量
    private Vector2 scrollPosition;
    private Dictionary<string, string> allPrefs = new Dictionary<string, string>();
    private bool isLoaded = false;

    void OnGUI()
    {
        GUILayout.Label("PlayerPrefs 管理器", EditorStyles.boldLabel);

        GUILayout.Space(10);

        // 清除所有
        if (GUILayout.Button("清除所有PlayerPrefs", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("确认", "确定要清除所有PlayerPrefs数据吗？", "确定", "取消"))
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
                allPrefs.Clear();
                Debug.Log("所有PlayerPrefs已清除！");
            }
        }

        GUILayout.Space(10);

        // 删除指定Key
        GUILayout.Label("删除指定Key:");
        keyToDelete = EditorGUILayout.TextField("Key名称:", keyToDelete);
        if (GUILayout.Button("删除该Key"))
        {
            if (!string.IsNullOrEmpty(keyToDelete))
            {
                PlayerPrefs.DeleteKey(keyToDelete);
                PlayerPrefs.Save();
                Debug.Log($"已删除Key: {keyToDelete}");
                keyToDelete = "";
                if (isLoaded) LoadAllPrefs(); // 刷新列表
            }
        }

        GUILayout.Space(10);

        // 添加新的PlayerPrefs
        GUILayout.Label("添加新的PlayerPrefs:");
        keyToAdd = EditorGUILayout.TextField("Key:", keyToAdd);
        valueToAdd = EditorGUILayout.TextField("Value:", valueToAdd);
        if (GUILayout.Button("添加"))
        {
            if (!string.IsNullOrEmpty(keyToAdd))
            {
                PlayerPrefs.SetString(keyToAdd, valueToAdd);
                PlayerPrefs.Save();
                Debug.Log($"已添加 {keyToAdd}: {valueToAdd}");
                keyToAdd = "";
                valueToAdd = "";
                if (isLoaded) LoadAllPrefs(); // 刷新列表
            }
        }

        GUILayout.Space(20);

        // 分隔线
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

        // 查看所有值接口
        DrawAllPrefsList();
    }

    private void DrawAllPrefsList()
    {
        GUILayout.Space(10);
        GUILayout.Label("查看所有 PlayerPrefs (部分平台支持)", EditorStyles.boldLabel);

        if (GUILayout.Button("刷新/加载列表"))
        {
            LoadAllPrefs();
        }

        if (isLoaded)
        {
            if (allPrefs.Count == 0)
            {
                GUILayout.Label("未找到数据 或 当前平台不支持读取。");
            }
            else
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, "box", GUILayout.Height(300));

                foreach (var kvp in allPrefs)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(kvp.Key, GUILayout.Width(150));
                    GUILayout.TextField(kvp.Value); // 使用TextField方便复制

                    if (GUILayout.Button("X", GUILayout.Width(25)))
                    {
                        if (EditorUtility.DisplayDialog("删除", $"确定要删除Key: {kvp.Key} 吗?", "是", "否"))
                        {
                            PlayerPrefs.DeleteKey(kvp.Key);
                            PlayerPrefs.Save();
                            // 稍后刷新，避免在遍历中修改集合
                            EditorApplication.delayCall += LoadAllPrefs;
                        }
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
            }
        }
    }

    private void LoadAllPrefs()
    {
        allPrefs.Clear();

#if UNITY_EDITOR_OSX
        LoadAllPrefsMac();
#elif UNITY_EDITOR_WIN
        LoadAllPrefsWin();
#else
        Debug.LogWarning("此工具的'查看所有'功能暂不支持该平台。");
#endif
        isLoaded = true;
    }

#if UNITY_EDITOR_OSX
    private void LoadAllPrefsMac()
    {
        try
        {
            string bundleId = PlayerSettings.applicationIdentifier;
            if (string.IsNullOrEmpty(bundleId))
            {
                Debug.LogError("Bundle Identifier 未设置，无法读取 PlayerPrefs。");
                return;
            }

            // 使用 defaults export 命令导出 xml 格式的 plist
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "defaults",
                    Arguments = $"export {bundleId} -",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(output))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(output);

                XmlNode dictNode = doc.SelectSingleNode("plist/dict");
                if (dictNode != null && dictNode.HasChildNodes)
                {
                    string currentKey = null;
                    foreach (XmlNode node in dictNode.ChildNodes)
                    {
                        if (node.Name == "key")
                        {
                            currentKey = node.InnerText;
                        }
                        else if (currentKey != null)
                        {
                            allPrefs[currentKey] = node.InnerText;
                            currentKey = null;
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"读取 PlayerPrefs 失败: {e.Message}");
        }
    }
#endif

#if UNITY_EDITOR_WIN
    private void LoadAllPrefsWin()
    {
        try
        {
            // Windows 下 Unity 通常存储在 HKCU\Software\[CompanyName]\[ProductName]
            // 注意：如果 CompanyName 或 ProductName 包含特殊字符，注册表路径可能不同
            string keyPath = $"Software\\{PlayerSettings.companyName}\\{PlayerSettings.productName}";
            
            using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(keyPath))
            {
                if (key != null)
                {
                    string[] valueNames = key.GetValueNames();
                    foreach (var vName in valueNames)
                    {
                        object val = key.GetValue(vName);
                        allPrefs[vName] = val != null ? val.ToString() : "null";
                    }
                }
                else
                {
                    Debug.LogWarning($"注册表路径未找到: {keyPath}");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"读取注册表失败: {e.Message}");
        }
    }
#endif
}
#endif