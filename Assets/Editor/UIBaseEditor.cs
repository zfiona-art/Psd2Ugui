using FrameWork.UI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ViewBase), true)]
public class UIBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("ReGenerate"))
        {
            var viewBase = target as ViewBase;
            if (viewBase != null)
            {
                UIBindCodeGenerator.ReGenerate(viewBase.gameObject, viewBase);
            }
        }
    }
}
