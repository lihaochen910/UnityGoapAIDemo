using UnityEngine;
using Prime31;

public class Boss08_Atk01_Arrow : Nez.AI.FSM.SimpleStateMachine<Boss08_Atk01_Arrow.State> {

	public enum State {
		Fly,
		Boom
	}

	public float angle;
	public Vector2 Dir;
	public float FlySpeed;

	public string DamageSource = "Boss08_a";
	public string DamageKey = "Atk_1";
	public float FlyDestroyTime = 3f;
	public bool BoomOnDamage = true;
	public State StartState = State.Fly;

	private Animator _animator;
	private DamageDealer _damageDealer;
	private AttackEventListener _attackEventListener;
	private CharacterController2D _characterController2D;
	private void Awake () {
		_animator = transform.Find ( "renderer" ).GetComponent<Animator> ();
		_attackEventListener = transform.Find ( "renderer" ).GetComponent<AttackEventListener> ();
		_characterController2D = GetComponent<CharacterController2D> ();
	}

	private void Start () {

		_damageDealer = DamageDealer.NewEnemy ();
		_damageDealer.SetDamageFlags ( true, false, true );
		_damageDealer.SetDirection ( DamageDirection.Neutral, transform );
		_damageDealer.attackType = AttackType.Hit;
		// _damageDealer.damage = LevelMonsterStrengthManager.Instance.GetDamage ( DamageSource, DamageKey );

		initialState = StartState;
	}

	void OnAttackHit (GameObject hitObject, Vector3 hitPoint) {

		if ( !hitObject.CompareTag ( "PlayerBody" ) ) {
			return;
		}

		if ( _damageDealer.DealDamage ( hitObject, hitPoint ) ) {
			if ( BoomOnDamage ) {
				currentState = State.Boom;
			}
		}
	}
	
	void Fly_Enter () {

		_animator.Play ( "Atk_Weapon" );

		// if ( DamageKey == "Atk_3" ) {
		// 	transform.GetChild ( 1 )?.SetLocalPosition ( 0f, -0.545f );
		// }

		_attackEventListener.OnHitPlayerEnterEvent += OnAttackHit;

		CupheadTime.WaitForSeconds ( this, FlyDestroyTime, CupheadTime.Layer.Enemy, () => currentState = State.Boom );
	}

	void Fly_Tick () {

		_characterController2D.Move ( FlySpeed * CupheadTime.Delta[CupheadTime.Layer.Enemy] * (Vector3)Dir );

		if ( _characterController2D.collisionState.hasCollision () ) {
			currentState = State.Boom;
		}
	}

	void Boom_Enter () {

		// EffectManager.ApplyParticleEffectAt ( "pse_effect_boss08_xuecisuipian", transform.position );

		Object.Destroy ( gameObject );
	}
}
