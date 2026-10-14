using System.Collections.Generic;

// 对应 BuffID.ts
public enum BuffType
{
    None = 0,
    
}

// 对应 IBuff.ts 中的 BuffRemoveReason
public enum BuffRemoveReason
{
    Expire = 1, // 到时移除
    Dispel, // 被净化/手动移除
    Override, // 被同类高优先级/新实例覆盖
    External // 其他外部原因
}

// 对应 IBuff.ts 中的 BuffContext
public class BuffContext
{
    // 放需要用到的上下文
    // 你可以根据实际项目需求修改这里的类型，例如 public GameModel Game;
    public object Game;
    public object View;

    // 也可以放一些通用工具引用
}