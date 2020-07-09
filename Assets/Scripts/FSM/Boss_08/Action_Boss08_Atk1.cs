using UnityEngine;
using HutongGames.PlayMaker;

public class Action_Boss08_Atk1 : FsmStateAction {

	public Vector2 Offset   = new Vector2 ( -0.88f, 0.465f );
	public float   FlySpeed  = 5f;
	public float   AngleMin  = -120f;
	public float   AngleMax  = -155f;
	public string  DamageKey = "Atk_1";

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

		var obj = Object.Instantiate ( Resources.Load<GameObject> ( "Prefabs/Object/Boss08_Atk01_Arrow" ) );
		obj.transform.position = Owner.transform.position + new Vector3 ( faceDirectionComponent.GetrRelativeHorizontalFaceDirection () == 1 ? Offset.x : -Offset.x, Offset.y );
		
		var comp = obj.AddComponent<Boss08_Atk01_Arrow> ();
		comp.DamageSource = Owner.name;
		comp.DamageKey    = DamageKey;
		comp.FlySpeed     = FlySpeed;

		if ( faceDirectionComponent.GetrRelativeHorizontalFaceDirection () == 1 ) {
			comp.angle = Random.Range ( AngleMin, AngleMax );
		}
		else {
			comp.angle = Random.Range ( AngleMin, AngleMax ) + 180f - AngleMax;
		}
		comp.Dir = MathUtils.AngleToDirection ( comp.angle );

		obj.transform.Find ( "renderer" ).SetLocalEulerAngles ( null, null, comp.angle - 180f );
		obj.transform.Find ( "renderer/tail" ).SetLocalPosition ( -0.15f, 1f );
	}

}
