using UnityEngine;
using Prime31;

public class Boss08_Atk04_StartArrow : Nez.AI.FSM.SimpleStateMachine<Boss08_Atk04_StartArrow.State> {

	public enum State {
		Fly,
		Boom
	}

	public float angle;
	public Vector2 Dir;
	public float FlySpeed;
	public float FlyTrackWaitTime;
	public float FlyTrackWaitTimeRandomRange;
	public Transform target;

	public string DamageSource = "Boss08_a";
	public string DamageKey = "Atk_1";
	public float FlyTime = 3f;
	public float SkillCount;
	public float SkillRange;
	public State StartState = State.Fly;

	private Animator _animator;
	private DamageDealer _damageDealer;
	private AttackEventListener _attackEventListener;
	private CharacterController2D _characterController2D;
	private float _timer;
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

		_damageDealer.DealDamage ( hitObject, hitPoint );
	}
	
	void Fly_Enter () {

		_animator.Play ( "Atk_Weapon" );
		
		// Debug.Log ( $"angle: {angle}" );

		// if ( angle < 100 ) {
		// 	transform.GetChild ( 1 )?.SetLocalPosition ( 0.394f, 0.317f );
		// }
		// else {
		// 	transform.GetChild ( 1 )?.SetLocalPosition ( -0.386f, 0.317f );
		// }

		_attackEventListener.OnHitPlayerEnterEvent += OnAttackHit;
	}

	void Fly_Tick () {

		_characterController2D.Move ( FlySpeed * CupheadTime.Delta[CupheadTime.Layer.Enemy] * (Vector3)Dir );

		if ( _characterController2D.collisionState.hasCollision () ) {
			currentState = State.Boom;
		}

		_timer += CupheadTime.Delta[ CupheadTime.Layer.Enemy ];

		if ( _timer >= FlyTime ) {
			currentState = State.Boom;
		}
	}

	void Boom_Enter () {

		// EffectManager.ApplyParticleEffectAt ( "pse_effect_boss08_xuecisuipian", transform.position );

		Object.Destroy ( gameObject );
		
		var hit = Physics2D.Raycast ( transform.position, Vector2.down, float.MaxValue, LayerMask.GetMask ( "Platform", "OneWayPlatform", "Wall" ) );
		float rangeLeft = 0f, rangeRight = 0f;

		if ( hit.collider != null ) {
			rangeLeft  = transform.position.x - SkillRange * 0.5f;
			rangeRight = transform.position.x + SkillRange * 0.5f;
			AIUtils.ClampPlatformEdgePoint ( hit.collider, ref rangeLeft, ref rangeRight );
		}
		else {
			Debug.LogError ( $"Action_Boss08_Atk4_Teleport没找到平台" );
		}

		for ( var i = 0; i < SkillCount; ++i ) {
			
			var obj = Object.Instantiate ( Resources.Load<GameObject> ( "Prefabs/Object/Boss08_Atk04_Arrow" ), transform.position, Quaternion.identity );

			var arrow = obj.GetComponent<Boss08_Atk04_EndArrow> ();
			arrow.velocity         = MathUtils.AngleToDirection ( Random.Range ( -180f, 0f ) );
			arrow.targetPosition   = new Vector3 ( Random.Range ( rangeLeft, rangeRight ), hit.collider.bounds.max.y );
			arrow.DamageSource     = DamageSource;
			arrow.FlyTrackWaitTime = FlyTrackWaitTime;
			arrow.FlyTrackWaitTimeRandomRange = FlyTrackWaitTimeRandomRange;
			arrow.target           = target;
		}
	}
}
