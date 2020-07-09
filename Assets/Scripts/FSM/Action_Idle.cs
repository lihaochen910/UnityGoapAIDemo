using Enemy;
using HutongGames.PlayMaker;

/// <summary>
/// 怪通用的用于待机时的Action
/// 下方没有平台时将会发出事件:OnFalling
/// </summary>
public class Action_Idle : MonsterFsmStateAction
{
    private bool ignoreFalling = false;

    public override void Awake()
    {
        base.Awake();
    }

    public override void OnEnter()
    {
        ignoreFalling = false;
        //m_Monster.SetDirectionalInput(Vector2.zero);
        m_Monster.velocity.x = 0;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (ignoreFalling)
            return;

        if (!m_Monster.isGrounded)
            Fsm.Event("OnFalling");
    }

    public override bool Event(FsmEvent fsmEvent)
    {
        if (fsmEvent.Name.Equals("JumpBack"))
            ignoreFalling = true;
        return base.Event(fsmEvent);
    }
}
