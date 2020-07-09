using UnityEngine;
using Prime31;

public class Boss08_Atk05_Arrow : Nez.AI.FSM.SimpleStateMachine<Boss08_Atk05_Arrow.State> {

	public enum State {
		Fly,
		FlyTrack,
		Boom
	}

	public Vector2 Dir;
	public float FlySpeed;
	public float FlyTrackWaitTime;
	public float FlyTrackWaitTimeRandomRange;
	public Transform Target;

	public string DamageSource = "Boss08_a";
	public string DamageKey = "Atk_5";
	public float FlyDestroyTime = 3f;
	public State StartState = State.Fly;

	private DamageDealer _damageDealer;
	private CharacterController2D _characterController2D;
	private float _timer;
	private bool _flyTrackEnable;
	private float _flyTrackWaitTimeRandomRange;
	private void Awake () {
		_characterController2D = GetComponent<CharacterController2D> ();
	}

	private void Start () {

		_damageDealer = DamageDealer.NewEnemy ();
		_damageDealer.SetDamageFlags ( true, false, true );
		_damageDealer.SetDirection ( DamageDirection.Neutral, transform );
		_damageDealer.ignoreReceiverOnDealDamage = true;
		_damageDealer.attackType = AttackType.Hit;
		// _damageDealer.damage = LevelMonsterStrengthManager.Instance.GetDamage ( DamageSource, DamageKey );

		initialState = StartState;

		_flyTrackEnable = DamageSource != "Boss08_a";
		_flyTrackEnable = true;
		_flyTrackWaitTimeRandomRange = Random.Range ( FlyTrackWaitTime - FlyTrackWaitTimeRandomRange,
			FlyTrackWaitTime + FlyTrackWaitTimeRandomRange );
	}
	
	void Fly_Enter () {
		_timer = 0f;
	}

	void Fly_Tick () {
		
		if ( Target == null ) {
			currentState = State.Boom;
			return;
		}

		_characterController2D.Move ( FlySpeed * CupheadTime.Delta[ CupheadTime.Layer.Enemy ] * ( Vector3 )Dir );
		if ( _characterController2D.collisionState.hasCollision () ) {
			currentState = State.Boom;
		}
		
		Collider2D hitCollider2D = Physics2D.OverlapCircle ( transform.position, 0.23f, LayerMask.GetMask ( "PlayerBody" ) );
		if ( hitCollider2D != null ) {
			_damageDealer.DealDamage ( hitCollider2D.gameObject, hitCollider2D.transform.position );
		}

		_timer += CupheadTime.Delta[ CupheadTime.Layer.Enemy ];
		if ( _flyTrackEnable && _timer >= _flyTrackWaitTimeRandomRange ) {
			Dir = ( Target.position - transform.position ).normalized;
			_timer = 0f;
			_flyTrackEnable = false;
		}
		if ( _timer >= FlyDestroyTime || Target == null ) {
			currentState = State.Boom;
		}
	}

	void Boom_Enter () {

		// EffectManager.ApplyParticleEffectAt ( "pse_effect_boss08_xuecisuipian", transform.position );

		Object.Destroy ( gameObject );
	}
}
