using UnityEngine;
using Prime31;

public class Boss08_Atk09_Tentacle : Nez.AI.FSM.SimpleStateMachine<Boss08_Atk09_Tentacle.State> {

	public enum State {
		Start,
		Mid,
		End,
		Fade
	}

	public float  Duration;
	public float  DamageInterval;
	public string DamageSource = "Boss08_a";
	public string DamageKey    = "Atk_9";

	private Animator _animator;
	private Animator _animator2;
	private DamageDealer _damageDealer;
	private float _timer;
	private void Awake () {
		_animator = transform.Find ( "renderer" ).GetComponent<Animator> ();
		_animator2 = transform.Find ( "renderer2" ).GetComponent<Animator> ();
	}

	private void Start () {

		_damageDealer = DamageDealer.NewEnemy ();
		_damageDealer.SetDamageFlags ( true, false, true );
		_damageDealer.SetDirection ( DamageDirection.Neutral, transform );
		_damageDealer.useDamageInterval = true;
		_damageDealer.damageInterval    = DamageInterval;
		_damageDealer.attackType        = AttackType.Hit;
		// _damageDealer.damage            = LevelMonsterStrengthManager.Instance.GetDamage ( DamageSource, DamageKey );
		
		initialState = State.Start;
	}
	
	void Start_Enter () {
		
		_animator.Play ( "Boss_08_Atk09_texiaoq_start" );
		_animator2.Play ( "Boss_08_Atk09_texiaoh_start" );
	}
	
	void Start_Tick () {
		if ( _animator.GetCurrentAnimationProgress () >= 1f ) {
			currentState = State.Mid;
		}
	}
	
	void Mid_Enter () {
		_animator.Play ( "Boss_08_Atk09_texiaoq_mid" );
		_animator2.Play ( "Boss_08_Atk09_texiaoh_mid" );
	}
	
	void Mid_Tick () {

		_timer += CupheadTime.Delta[ CupheadTime.Layer.Enemy ];
		if ( _timer >= Duration ) {
			currentState = State.End;
		}
		
		_damageDealer.Update ();
		
		Collider2D[] hitCollider2Ds = Physics2D.OverlapBoxAll ( transform.position + new Vector3 ( -0.54f, 3.34f ), new Vector2 ( 2.88f, 8.8f ), 0f, LayerMask.GetMask ( "PlayerBody" ) );

		foreach ( var hit in hitCollider2Ds ) {
			_damageDealer.DealDamage ( hit.gameObject, hit.transform.position );
		}
		
		// DebugExtensions.DrawBox ( transform.position + new Vector3 ( -0.54f, 3.34f ), new Vector2 ( 2.88f, 8.8f ), 0f, Color.red );
	}
	
	void End_Enter () {
		_animator.Play ( "Boss_08_Atk09_texiaoq_end" );
		_animator2.Play ( "Boss_08_Atk09_texiaoh_end" );
	}
	
	void End_Tick () {
		if ( _animator.GetCurrentAnimationProgress () >= 1f ) {
			currentState = State.Fade;
		}
	}
	
	void Fade_Enter () {
		Object.Destroy ( gameObject );
	}
}
