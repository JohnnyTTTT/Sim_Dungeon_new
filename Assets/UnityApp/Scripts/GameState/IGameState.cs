using Johnny.SimDungeon;

public interface IGameState
{
    GameState StateID { get; }

    // 状态被激活时调用，用于初始化状态逻辑
    void Enter();

    // 状态退出时调用，用于清理资源或保存数据
    void Exit();

    // 状态持续期间的更新逻辑（对应 Unity 中的 Update 或 FixedUpdate）
    void Update();
}