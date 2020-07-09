using UnityEngine;

public class Boss08_Atk03_Skill : Nez.AI.FSM.SimpleStateMachine<Boss08_Atk03_Skill.State> {

	public enum State {
		Skill,
		Fade
	}

	public float FlySpeed = 15f;
	public float Angle    = -90f;

	public string DamageSource = "Boss08_a";
	public string DamageKey = "Atk_3";

	private Animator _animator;
	private DamageDealer _damageDealer;
	private SimpleAnimationEventHandler _eventHandler;
	private void Awake () {
		_animator = transform.Find ( "renderer" ).GetComponent<Animator> ();
		_eventHandler = transform.Find ( "renderer" ).GetComponent<SimpleAnimationEventHandler> ();
	}

	private void Start () {

		_damageDealer = DamageDealer.NewEnemy ();
		_damageDealer.SetDamageFlags ( true, false, true );
		_damageDealer.SetDirection ( DamageDirection.Neutral, transform );
		_damageDealer.attackType = AttackType.Hit;
		// _damageDealer.damage = LevelMonsterStrengthManager.Instance.GetDamage ( DamageSource, DamageKey );

		initialState = State.Skill;
		_eventHandler.OnStringEvent += OnCustomEvent;
	}
	
	private void OnCustomEvent ( string evt ) {

		if ( !evt.Equals ( "Fire" ) ) {
			return;
		}
		
		var obj = Object.Instantiate ( Resources.Load<GameObject> ( "Prefabs/Object/Boss08_Atk01_Arrow" ) );
		// obj.transform.position = transform.position + new Vector3 ( faceDirectionComponent.FaceDirection == 1 ? -0.111f : 0.19f, 0.68f );
		obj.transform.position = transform.position + new Vector3 ( -0.111f, 0.68f );
		
		var comp = obj.AddComponent<Boss08_Atk01_Arrow> ();
		comp.DamageSource = DamageSource;
		comp.DamageKey    = DamageKey;
		comp.FlySpeed     = FlySpeed;
		comp.angle        = Angle;
		comp.Dir = MathUtils.AngleToDirection ( comp.angle );

		obj.transform.Find ( "renderer" ).SetLocalEulerAngles ( null, null, comp.angle - 180f );
		obj.transform.Find ( "renderer/tail" ).SetLocalPosition ( 0f, 1f );
	}
	
	void Skill_Enter () {
		_animator.Play ( "Boss_08_Atk03_mid" );
	}

	void Skill_Tick () {
		if ( _animator.GetCurrentAnimationProgress () >= 1f ) {
			currentState = State.Fade;
		}
	}

	void Fade_Enter () {
		Object.Destroy ( gameObject );
	}
}
