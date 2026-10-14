using UnityEngine;

public static class GameObjectUtil
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        var component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static void RemoveChild(this GameObject go)
    {
        for (int i = go.transform.childCount - 1; i >= 0; i--)
        {
            var child = go.transform.GetChild(i);
            Object.Destroy(child.gameObject);
        }
    }
}