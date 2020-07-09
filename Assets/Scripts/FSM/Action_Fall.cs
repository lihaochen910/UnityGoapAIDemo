using Enemy;
using HutongGames.PlayMaker;

public class Action_Fall : MonsterFsmStateAction
{
    public FsmEvent GroundedEvent = FsmEvent.Finished;
	public float FallSpeedEx = 0f;
	public float FallSpeedExAdd = 0f;

	private float currentFallSpeedEx;
	public override void OnEnter () {
		currentFallSpeedEx = FallSpeedEx;
	}

	public override void OnUpdate()
    {
		if ( FallSpeedEx != 0f && FallSpeedExAdd != 0f ) {
			currentFallSpeedEx += FallSpeedExAdd * CupheadTime.Delta[CupheadTime.Layer.Enemy];
			m_Monster.Move ( new UnityEngine.Vector2 ( 0f, -currentFallSpeedEx * CupheadTime.Delta[CupheadTime.Layer.Enemy] ) );
		}

		if ( m_Monster.isGrounded ) {
			Fsm.Event ( GroundedEvent );
		}
	}
}
