using UnityEngine;
using HutongGames.PlayMaker;

public class Action_Boss08_Atk5 : FsmStateAction {

	public float FlySpeed         = 8f;
	public float Angle            = -120f;
	public float AngleAdd         = 30f;
	public float FlyTrackWaitTime = 0.5f;
	public float FlyTrackWaitTimeRandomRange = 0.2f;
	public string DamageKey = "Atk_5";

	private MonsterAnimationEventHandler eventHandler;
	private FaceDirectionComponent       faceDirectionComponent;
	private float                         currentAngle;
	private Transform                    target;
	
	public override void Awake () {
		eventHandler           = Owner.transform.Find ( "ActionAnimator" ).GetComponent<MonsterAnimationEventHandler> ();
		faceDirectionComponent = Owner.GetComponent<FaceDirectionComponent> ();
	}

	public override void OnEnter () {
		eventHandler.OnCustomAnimationEvent += OnCustomEvent;
		currentAngle = Angle;
		target = Owner.GetComponent< NodeCanvas.Framework.Blackboard > ().GetValue< Transform > ( "Target" );
	}

	public override void OnExit () {
		eventHandler.OnCustomAnimationEvent -= OnCustomEvent;
	}

	private void OnCustomEvent ( string evt ) {
		
		if ( target == null ) {
			return;
		}

		var obj = Object.Instantiate ( Resources.Load<GameObject> ( "Prefabs/Object/Boss08_Atk05_Arrow" ) );
		obj.transform.position = Owner.transform.position;
		
		var comp = obj.GetOrAddComponent<Boss08_Atk05_Arrow> ();
		comp.DamageSource     = Owner.name;
		comp.DamageKey        = DamageKey;
		comp.FlySpeed         = FlySpeed;
		comp.Dir              = MathUtils.AngleToDirection ( currentAngle );
		comp.FlyTrackWaitTime = FlyTrackWaitTime;
		comp.FlyTrackWaitTimeRandomRange = FlyTrackWaitTimeRandomRange;
		comp.Target           = target;

		currentAngle += AngleAdd * faceDirectionComponent.FaceDirection;
	}

}
