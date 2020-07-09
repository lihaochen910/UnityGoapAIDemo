using UnityEngine;
using HutongGames.PlayMaker;

public class Action_Boss08_Atk4 : FsmStateAction {

	public float flySpeed = 5f;
	public float flyTime  = 0.5f;
	public float flyTrackWaitTime = 0.5f;
	public float flyTrackWaitTimeRandomRange = 0.2f;
	public float angle   = 40f;
	public int count = 3;
	public float range = 8f;
	public string damageKey = "Atk_4";

	private MonsterAnimationEventHandler eventHandler;
	private FaceDirectionComponent       faceDirectionComponent;
	
	public override void Awake () {
		eventHandler           = Owner.transform.Find ( "ActionAnimator" ).GetComponent<MonsterAnimationEventHandler> ();
		faceDirectionComponent = Owner.GetComponent<FaceDirectionComponent> ();
	}

	public override void OnEnter () {
		eventHandler.OnCustomAnimationEvent += OnCustomEvent;
	}

	public override void OnExit () {
		eventHandler.OnCustomAnimationEvent -= OnCustomEvent;
	}

	private void OnCustomEvent ( string evt ) {

		var obj = Object.Instantiate ( Resources.Load<GameObject> ( "Prefabs/Object/Boss08_Atk01_Arrow" ) );
		obj.transform.position = Owner.transform.position + new Vector3 ( faceDirectionComponent.FaceDirection == 1 ? -0.334f : 0.29f, 0.52f );
		
		var comp = obj.AddComponent<Boss08_Atk04_StartArrow> ();
		comp.DamageSource     = Owner.name;
		comp.DamageKey        = damageKey;
		comp.FlySpeed         = flySpeed;
		comp.FlyTime          = flyTime;
		comp.SkillCount       = count;
		comp.SkillRange       = range;
		comp.FlyTrackWaitTime = flyTrackWaitTime;
		comp.FlyTrackWaitTimeRandomRange = flyTrackWaitTimeRandomRange;
		comp.target = Owner.GetComponent< NodeCanvas.Framework.Blackboard > ().GetValue< Transform > ( "Target" );

		if ( faceDirectionComponent.FaceDirection == 1 ) {
			comp.angle = angle;
		}
		else {
			comp.angle = angle + 180f - angle * 2f;
		}
		comp.Dir = MathUtils.AngleToDirection ( comp.angle );

		obj.transform.Find ( "renderer" ).SetLocalEulerAngles ( null, null, comp.angle - 180f );
		obj.transform.Find ( "renderer/tail" ).SetLocalPosition ( 0f, 1f );
	}

}