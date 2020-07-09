using Enemy;
using HutongGames.PlayMaker;

public class Action_CloseControl : MonsterFsmStateAction
{
    public FsmBool ActiveGravity = true;
    public FsmBool ActiveControl = true;

    public override void OnEnter()
    {
        m_Monster.ActiveGravity = ActiveGravity.Value;
        m_Monster.ActiveControl = ActiveControl.Value;
    }

    public override void OnExit()
    {
        m_Monster.ActiveGravity = true;
        m_Monster.ActiveControl = true;
    }
}
