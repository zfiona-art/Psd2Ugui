using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public sealed class YooAssetHandleOwner : MonoBehaviour
{
    private readonly Dictionary<string, AssetHandle> _handles = new();

    public static YooAssetHandleOwner GetOrAdd(GameObject go)
    {
        if (go == null) return null;
        var owner = go.GetComponent<YooAssetHandleOwner>();
        if (owner == null)
            owner = go.AddComponent<YooAssetHandleOwner>();
        return owner;
    }

    public void SetHandle(string key, AssetHandle handle)
    {
        ReleaseHandle(key);
        if (handle != null)
            _handles[key] = handle;
    }

    public bool IsCurrent(string key, AssetHandle handle)
    {
        return _handles.TryGetValue(key, out var current) && ReferenceEquals(current, handle);
    }

    public void ReleaseHandle(string key)
    {
        if (!_handles.Remove(key, out var handle)) return;
        handle?.Release();
    }

    public void ReleaseAll()
    {
        foreach (var handle in _handles.Values)
            handle?.Release();
        _handles.Clear();
    }

    private void OnDestroy()
    {
        ReleaseAll();
    }
}
