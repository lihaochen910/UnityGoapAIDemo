using UnityEngine;
using HutongGames.PlayMaker;

public class Action_Boss08_Atk6 : FsmStateAction {

	public float UpDuration  = 1f;
	public float Duration    = 2.5f;
	public float Interval    = 0.1f;
	public float FollowSpeed = 10f;
	public float FlySpeed    = 10f;
	public string DamageKey = "Atk_6";

	private MonsterAnimationEventHandler eventHandler;
	private FaceDirectionComponent faceDirectionComponent;
	
	public override void Awake () {
		eventHandler = Owner.transform.Find ( "ActionAnimator" ).GetComponent<MonsterAnimationEventHandler> ();
		faceDirectionComponent = Owner.GetComponent<FaceDirectionComponent> ();
	}

	public override void OnEnter () {
		eventHandler.OnCustomAnimationEvent += OnCustomEvent;
	}

	public override void OnExit () {
		eventHandler.OnCustomAnimationEvent -= OnCustomEvent;
	}

	private void OnCustomEvent ( string evt ) {

		var target = Owner.GetComponent< NodeCanvas.Framework.Blackboard > ().GetValue< Transform > ( "Target" );

		if ( target == null ) {
			return;
		}

		var obj = Object.Instantiate ( Resources.Load<GameObject> ( "Prefabs/Object/Boss08_Atk05_Arrow" ) );
		obj.transform.position = Owner.transform.position + new Vector3 ( faceDirectionComponent.FaceDirection == 1 ? -0.47f : 0.47f, 0.6f );

		Object.Destroy ( obj.GetComponent< Boss08_Atk05_Arrow > () );
		
		var comp = obj.AddComponent<Boss08_Atk06_Tracker> ();
		comp.DamageSource = Owner.name;
		comp.DamageKey    = DamageKey;
		comp.Duration     = Duration;
		comp.Interval     = Interval;
		comp.FollowSpeed  = FollowSpeed;
		comp.FlySpeed     = FlySpeed;
		comp.Target       = target;
		comp.UpDuration   = UpDuration;
	}

}
