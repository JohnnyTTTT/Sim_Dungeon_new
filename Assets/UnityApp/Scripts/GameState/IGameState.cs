using Johnny.SimDungeon;

public interface IGameState
{
    GameState StateID { get; }

    // ״̬������ʱ���ã����ڳ�ʼ��״̬�߼�
    void Enter();

    // ״̬�˳�ʱ���ã�����������Դ�򱣴�����
    void Exit();

    // ״̬�����ڼ�ĸ����߼�����Ӧ Unity �е� Update �� FixedUpdate��
    void Update();
}