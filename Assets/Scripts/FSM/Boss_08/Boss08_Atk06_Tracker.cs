using UnityEngine;

public class Boss08_Atk06_Tracker : Nez.AI.FSM.SimpleStateMachine<Boss08_Atk06_Tracker.State> {

	public enum State {
		Up,
		Track,
		Fade
	}

	public float      UpDuration;
	public Transform Target;
	public float     Duration;
	public float     Interval;
	public float     FollowSpeed;
	public float     FlySpeed;

	public string DamageSource = "Boss08_a";
	public string DamageKey    = "Atk_6";
	
	private FollowCamera _followCamera;
	private float        _timer;
	private Vector2 _startPosition;
	private Vector2 _upPosition;

	private void Start () {
		_followCamera = Camera.main.GetComponent< FollowCamera > ();
		initialState = State.Up;
	}
	
	void Up_Enter () {
		_startPosition = transform.position;
		_upPosition = new Vector2 ( transform.position.x, _followCamera.TopLeft.y + 5f );
	}
	
	void Up_Exit () {
		transform.GetChild ( 0 )?.gameObject.SetActive ( false );
	}
	
	void Up_Tick () {

		_timer += CupheadTime.Delta[ CupheadTime.Layer.Enemy ];
		
		transform.SetPosition (
			EaseUtils.Linear ( _startPosition.x, _upPosition.x, _timer / UpDuration ),
			EaseUtils.Linear ( _startPosition.y, _upPosition.y, _timer / UpDuration )
		);

		if ( _timer >= UpDuration ) {
			currentState = State.Track;
		}
	}

	void Track_Enter () {
		CupheadTime.WaitForSeconds ( this, Duration, CupheadTime.Layer.Enemy, () => currentState = State.Fade );
		_timer = 0f;
		transform.SetPosition ( Target.position.x );
	}

	void Track_Tick () {

		_timer += CupheadTime.Delta[ CupheadTime.Layer.Enemy ];

		if ( _timer > Interval ) {
			
			var hit = Physics2D.Raycast ( new Vector2 ( transform.position.x, Target.position.y ), Vector2.down, float.MaxValue, LayerMask.GetMask ( "Platform", "OneWayPlatform", "Wall" ) );
			// EffectManager.ApplySpecificEffectAt ( "Boss_06_Atk03_warning", hit.point );
			
			var comp = Object.Instantiate ( Resources.Load<GameObject> ( "Prefabs/Object/Boss08_Atk05_Arrow" ) ).GetOrAddComponent<Boss08_Atk05_Arrow>();
			comp.transform.position = transform.position;
			comp.DamageSource = DamageSource;
			comp.DamageKey    = DamageKey;
			comp.FlySpeed = FlySpeed;
			comp.Dir      = MathUtils.AngleToDirection ( -90f );

			_timer = 0f;
		}

		transform.position += new Vector3 ( FollowSpeed * CupheadTime.Delta[CupheadTime.Layer.Enemy] * Mathf.Sign ( Target.position.x - transform.position.x ), 0f );
		transform.SetPosition ( null, _followCamera.TopLeft.y );
	}

	void Fade_Enter () {
		Object.Destroy ( gameObject );
	}
}
