using UnityEngine;
using HutongGames.PlayMaker;

public class Action_Boss08_Atk2 : FsmStateAction {

	public Vector2 Offset           = new Vector2 ( 0.256f, 0.45f );
	public float   FlySpeed         = 15f;
	public float   Angle            = 45f;
	public Vector2 JumpBackVelocity = new Vector2 ();
	public float   JumpBackFlySpeed = 3f;
	public string  DamageKey        = "Atk_2";

	private MonsterAnimationEventHandler eventHandler;
	private FaceDirectionComponent       faceDirectionComponent;
	private Enemy.Monster controller;
	private bool isJumpBack;
	
	public override void Awake () {
		eventHandler           = Owner.transform.Find ( "ActionAnimator" ).GetComponent<MonsterAnimationEventHandler> ();
		faceDirectionComponent = Owner.GetComponent<FaceDirectionComponent> ();
		controller = Owner.GetComponent<Enemy.Monster> ();
	}

	public override void OnEnter () {
		eventHandler.OnCustomAnimationEvent += OnCustomEvent;
		controller.ClearVelocity ();
		controller.ActiveGravity = false;
		isJumpBack = false;
	}

	public override void OnExit () {
		eventHandler.OnCustomAnimationEvent -= OnCustomEvent;
		controller.ActiveGravity = true;
		controller.velocity.x = 0f;
	}

	public override void OnUpdate () {
		if ( !isJumpBack ) {
			return;
		}
		
		controller.Move ( new Vector2 ( JumpBackFlySpeed * -faceDirectionComponent.FaceDirection * CupheadTime.Delta[ CupheadTime.Layer.Enemy ], -0.01f ) );
	}

	private void OnCustomEvent ( string evt ) {

		var force = JumpBackVelocity;
		if ( faceDirectionComponent.GetrRelativeHorizontalFaceDirection () == -1 ) {
			force.x = -force.x;
		}
		controller.AddForce ( force, ForceMode2D.Force );
		controller.ActiveGravity = true;
		
		isJumpBack = true;

		var obj = Object.Instantiate ( Resources.Load<GameObject> ( "Prefabs/Object/Boss08_Atk02_Arrow" ) );
		obj.transform.position = Owner.transform.position + new Vector3 ( faceDirectionComponent.GetrRelativeHorizontalFaceDirection () == 1 ? Offset.x : -Offset.x, Offset.y );
		
		var comp = obj.GetComponent<Boss08_Atk02_Arrow> ();
		comp.DamageSource = Owner.name;
		comp.DamageKey    = DamageKey;
		comp.FlySpeed     = FlySpeed;

		if ( faceDirectionComponent.GetrRelativeHorizontalFaceDirection () == 1 ) {
			comp.angle = Angle;
		}
		else {
			comp.angle = Angle + 180f - Angle * 2f;
		}
		comp.Dir = MathUtils.AngleToDirection ( comp.angle );

		obj.transform.Find ( "renderer" ).SetLocalEulerAngles ( null, null, comp.angle - 180f );
		obj.transform.Find ( "renderer/tail" ).SetLocalPosition ( 0f, 1f );
	}

}
