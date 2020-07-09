using UnityEngine;
using HutongGames.PlayMaker;

public class Action_Boss08_Atk3_LaunchArrow : FsmStateAction {

	public int  maxCount = 5;
	public float range    = 8f;
	public float height   = 5f; // 距离地面的高度
	
	public float FlySpeed = 15f;
	public float Angle    = 90f;
	public float Interval = 0.2f;
	public  string   DamageKey = "Atk_3";
	public  FsmEvent finishEvent;

	private FaceDirectionComponent faceDirectionComponent;
	private Enemy.Monster          controller;
	private float                   intervalTimer;
	
	private Transform target;
	private float      rangeLeft, rangeRight;
	private float      heightY;
	private int       currentCount;

	public override void Awake () {
		faceDirectionComponent = Owner.GetComponent<FaceDirectionComponent> ();
		controller = Owner.GetComponent<Enemy.Monster> ();
	}

	public override void OnEnter () {
		controller.ClearVelocity ();
		controller.ActiveGravity = false;
		intervalTimer = Interval;
		currentCount = 0;
		
		target = Owner.GetComponent< NodeCanvas.Framework.Blackboard > ().GetValue< Transform > ( "Target" );
			
		var hit = Physics2D.Raycast ( target.position, Vector2.down, float.MaxValue, LayerMask.GetMask ( "Platform", "OneWayPlatform", "Wall" ) );

		if ( hit.collider != null ) {
			heightY    = hit.collider.bounds.max.y + height;
			rangeLeft  = target.position.x - range * 0.5f;
			rangeRight = target.position.x + range * 0.5f;
			AIUtils.ClampPlatformEdgePoint ( hit.collider, ref rangeLeft, ref rangeRight );
		}
		else {
			Debug.LogError ( $"Action_Boss08_Atk3_Teleport没找到平台" );
		}
	}

	public override void OnExit () {
		controller.ActiveGravity = true;
		controller.velocity.x = 0f;
	}

	public override void OnUpdate () {
		
		intervalTimer += CupheadTime.Delta[ CupheadTime.Layer.Enemy ];
		if ( intervalTimer >= Interval ) {
			
			var obj = Object.Instantiate ( Resources.Load<GameObject> ( "Prefabs/Object/Boss08_Atk03_Skill" ) );
			obj.transform.SetPosition ( Random.Range ( rangeLeft, rangeRight ), heightY );
		
			var comp = obj.GetOrAddComponent<Boss08_Atk03_Skill> ();
			comp.DamageSource = Owner.name;
			comp.DamageKey    = DamageKey;
			comp.FlySpeed     = FlySpeed;
			comp.Angle        = Angle;
			
			if ( ++currentCount >= maxCount ) {
				Fsm.Event ( finishEvent );
				return;
			}
			
			intervalTimer = 0f;
		}
	}
}
