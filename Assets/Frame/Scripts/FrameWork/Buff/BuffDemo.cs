using UnityEngine;

public class BuffDemo : MonoBehaviour
{
    private BuffManager buffMgr;

    void Start()
    {
        Init();

        // 测试添加
        AddBuff();
    }

    void Update()
    {
        // 驱动 Buff 系统
        if (buffMgr != null)
        {
            buffMgr.Update(Time.deltaTime);
        }
    }

    public void Init()
    {
        // 构造上下文，把游戏核心对象传进去
        var ctx = new BuffContext();
        // ctx.Game = ModelMgr.Instance.Game; 

        this.buffMgr = new BuffManager(ctx);
    }

    public void AddBuff()
    {
        // 10秒的冰冻Buff
        // this.buffMgr?.Add(new FreezeBuff("buff1001", 10000));
    }

    public void ClearAll()
    {
        this.buffMgr?.Clear();
    }
}