using UnityEngine;
using Prime31;

public class Boss08_Atk02_Arrow : Nez.AI.FSM.SimpleStateMachine<Boss08_Atk02_Arrow.State> {

	public enum State {
		Fly,
		Boom,
		Fade
	}

	public float angle;
	public Vector2 Dir;
	public float FlySpeed;

	public string DamageSource = "Boss08_a";
	public string DamageKey = "Atk_2";
	public float FlyDestroyTime = 3f;

	private Animator _animator;
	private DamageDealer _damageDealer;
	private CharacterController2D _characterController2D;
	private void Awake () {
		_animator = transform.Find ( "renderer" ).GetComponent<Animator> ();
		_characterController2D = GetComponent<CharacterController2D> ();
	}

	private void Start () {

		_damageDealer = DamageDealer.NewEnemy ();
		_damageDealer.SetDamageFlags ( true, false, true );
		_damageDealer.SetDirection ( DamageDirection.Neutral, transform );
		_damageDealer.attackType = AttackType.Hit;
		// _damageDealer.damage = LevelMonsterStrengthManager.Instance.GetDamage ( DamageSource, DamageKey );
		
		initialState = State.Fly;
	}
	
	void Fly_Enter () {

		_animator.Play ( "Atk_Weapon" );
		
		CupheadTime.WaitForSeconds ( this, FlyDestroyTime, CupheadTime.Layer.Enemy, () => {
			if ( currentState == State.Fly ) {
				currentState = State.Fade;
			}
		} );

		_damageDealer.ignoreReceiverOnDealDamage = true;
		
		// Debug.Log ( $"angle: {angle}" );
	}

	void Fly_Tick () {

		_characterController2D.Move ( FlySpeed * CupheadTime.Delta[CupheadTime.Layer.Enemy] * (Vector3)Dir );

		if ( _characterController2D.collisionState.hasCollision () ) {
			currentState = State.Boom;
		}
		
		Collider2D hitCollider2D = Physics2D.OverlapBox ( transform.position + new Vector3 ( angle < 0f ? 0.44f : -0.44f, -0.27f ), new Vector2 ( 0.31f, 0.28f ), 0f, LayerMask.GetMask ( "PlayerBody" ) );

		if ( hitCollider2D != null ) {
			_damageDealer.DealDamage ( hitCollider2D.gameObject, hitCollider2D.transform.position );
		}
		
		// DebugExtensions.DrawBox ( transform.position + new Vector3 ( angle < 0f ? 0.44f : -0.44f, -0.27f ), new Vector2 ( 0.31f, 0.28f ), 0f, Color.red );
	}

	void Boom_Enter () {
		
		transform.GetChild ( 0 ).GetChild ( 0 )?.gameObject.SetActive ( false );
		
		_animator.Play ( "Boss_08_Atk02texiao" );
		_animator.transform.localScale = Vector3.one;
		_animator.transform.rotation = Quaternion.identity;
		_animator.transform.SetLocalPosition ( 0.5f, 0.9f );
		
		_damageDealer.ignoreReceiverOnDealDamage = false;
		_damageDealer.useDamageInterval = true;
		_damageDealer.damageInterval    = 0.5f;
		_damageDealer.ClearIgnoredReceiver ();
	}
	
	void Boom_Tick () {

		if ( _animator.GetCurrentAnimationProgress () >= 1f ) {
			currentState = State.Fade;
		}
		
		_damageDealer.Update ();
		
		Collider2D hitCollider2D = Physics2D.OverlapBox ( transform.position + new Vector3 ( 0.5f, 0.9f ) + new Vector3 ( -0.05f, -0.2f ), new Vector2 ( 1.6f, 1.7f ), 0f, LayerMask.GetMask ( "PlayerBody" ) );

		if ( hitCollider2D != null ) {
			_damageDealer.DealDamage ( hitCollider2D.gameObject, hitCollider2D.transform.position );
		}
	}
	
	void Fade_Enter () {
		Object.Destroy ( gameObject );
	}
}
