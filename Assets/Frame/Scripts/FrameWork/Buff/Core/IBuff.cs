using System.Collections.Generic;

public interface IBuff
{
    string Id { get; } // 唯一id
    BuffType Type { get; } // Buff种类
    long DurationMs { get; set; } // 持续时长（毫秒），=0 视为瞬时/仅执行一次
    long ElapsedMs { get; set; } // 已经执行时长
    bool Stackable { get; } // 是否可叠加
    int Priority { get; } // 覆盖优先级（越大越优先）

    // 生命周期
    void OnAdd(BuffContext ctx);
    void Exec(BuffContext ctx, int dtMs); // 执行具体逻辑（每帧/每Tick）
    void OnRemove(BuffContext ctx, BuffRemoveReason reason);

    // 工具
    bool IsExpired();
    void Tick(BuffContext ctx, int dtMs);
    void OnAddTime(BuffContext ctx, long dtMs);
}