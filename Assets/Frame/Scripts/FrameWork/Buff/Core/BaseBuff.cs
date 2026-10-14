using System;
using UnityEngine;

public abstract class BaseBuff : IBuff
{
    public string Id { get; protected set; }
    public BuffType Type { get; protected set; }
    public long DurationMs { get; set; }
    public long ElapsedMs { get; set; } = 0;
    public bool Stackable { get; protected set; } = false;
    public int Priority { get; protected set; } = 0;

    public BaseBuff(string id, BuffType type, long durationMs, bool stackable = false, int priority = 0)
    {
        this.Id = id;
        this.Type = type;
        this.DurationMs = Math.Max(0, durationMs);
        this.Stackable = stackable;
        this.Priority = priority;
    }

    public virtual void OnAdd(BuffContext ctx)
    {
    }

    public abstract void Exec(BuffContext ctx, int dtMs);

    public virtual void OnRemove(BuffContext ctx, BuffRemoveReason reason)
    {
    }

    public bool IsExpired()
    {
        return this.DurationMs >= 0 && this.ElapsedMs >= this.DurationMs;
    }

    public void Tick(BuffContext ctx, int dtMs)
    {
        // 先执行，再计时，可以处理 duration=0 的瞬时效果
        this.Exec(ctx, dtMs);
        if (this.DurationMs > 0)
        {
            this.ElapsedMs += dtMs;
        }
    }

    public virtual void OnAddTime(BuffContext ctx, long dtMs)
    {
        // 默认实现为空
    }
}