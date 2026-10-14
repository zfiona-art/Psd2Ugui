using System.Collections.Generic;
using UnityEngine;

public class BuffManager
{
    private Dictionary<string, IBuff> buffs = new Dictionary<string, IBuff>(); // 按实例id
    private Dictionary<BuffType, List<IBuff>> byType = new Dictionary<BuffType, List<IBuff>>(); // 按类型分组
    private BuffContext ctx;

    // 【优化】增加一个预分配的列表，用于每帧更新，避免重复 new
    private List<IBuff> _updateList = new List<IBuff>();

    public BuffManager(BuffContext ctx = null)
    {
        this.ctx = ctx ?? new BuffContext();
    }

    // 在 Unity Update 中调用，dt 为秒
    public void Update(float dt)
    {
        // 转换毫秒
        int dtMs = Mathf.FloorToInt(dt * 1000);
        if (dtMs <= 0) return;

        _updateList.Clear();
        foreach (var buff in this.buffs.Values)
        {
            _updateList.Add(buff);
        }

        for (int i = 0; i < _updateList.Count; i++)
        {
            var buff = _updateList[i];
            if (!this.buffs.ContainsKey(buff.Id)) continue;
            buff.Tick(this.ctx, dtMs);

            if (buff.IsExpired())
            {
                this.Remove(buff.Id, BuffRemoveReason.Expire);
            }
        }
    }

    public void Add(IBuff buff)
    {
        // 类型分组
        if (!this.byType.ContainsKey(buff.Type))
        {
            this.byType[buff.Type] = new List<IBuff>();
        }

        var group = this.byType[buff.Type];

        // 叠加策略：仅延长持续时间，不新增/移除
        if (buff.Stackable)
        {
            if (group.Count > 0)
            {
                var target = group[0]; // 选择第一个同类Buff延长时长
                if (buff.DurationMs > 0)
                {
                    target.DurationMs += buff.DurationMs;
                    target.OnAddTime(this.ctx, buff.DurationMs);
                }
            }
            else
            {
                this._Attach(buff);
            }

            return;
        }

        // 不可叠加：按优先级覆盖
        if (group.Count > 0)
        {
            /*
            // 找到优先级最高的
            group.Sort((a, b) => b.Priority.CompareTo(a.Priority));
            var winner = group[0];
            if (buff.Priority >= winner.Priority)
            {
                // 移除旧的（或者移除所有旧的）
                var losers = new List<IBuff>(group);
                foreach(var loser in losers)
                {
                    Remove(loser.Id, BuffRemoveReason.Override);
                }
                _Attach(buff);
            }
            */

            // 目前逻辑：直接添加，允许多个同类不可叠加Buff共存（除非业务层自己控制）
            this._Attach(buff);
        }
        else
        {
            this._Attach(buff);
        }
    }

    public void Remove(string id, BuffRemoveReason reason = BuffRemoveReason.Dispel)
    {
        if (!this.buffs.TryGetValue(id, out var buff)) return;

        // 从分组移除
        if (this.byType.TryGetValue(buff.Type, out var group))
        {
            var idx = group.FindIndex(b => b.Id == id);
            if (idx >= 0) group.RemoveAt(idx);
            if (group.Count == 0) this.byType.Remove(buff.Type);
        }

        // 回调
        buff.OnRemove(this.ctx, reason);

        // 从集合移除
        this.buffs.Remove(id);
    }

    public void RemoveByType(BuffType type, BuffRemoveReason reason = BuffRemoveReason.Dispel)
    {
        if (!this.byType.TryGetValue(type, out var group)) return;

        // 复制一份列表进行遍历移除
        var listToRemove = new List<IBuff>(group);
        foreach (var b in listToRemove)
        {
            this.Remove(b.Id, reason);
        }
    }

    public void Clear(BuffRemoveReason reason = BuffRemoveReason.Dispel)
    {
        var keys = new List<string>(this.buffs.Keys);
        foreach (var id in keys)
        {
            this.Remove(id, reason);
        }
    }

    public bool HasType(BuffType type)
    {
        return this.byType.TryGetValue(type, out var group) && group.Count > 0;
    }

    private void _Attach(IBuff buff)
    {
        if (this.buffs.ContainsKey(buff.Id))
        {
            Debug.LogWarning($"Buff with id {buff.Id} already exists!");
            return;
        }

        this.buffs.Add(buff.Id, buff);

        if (!this.byType.ContainsKey(buff.Type))
        {
            this.byType[buff.Type] = new List<IBuff>();
        }

        this.byType[buff.Type].Add(buff);

        buff.OnAdd(this.ctx);
    }
}