using UnityEngine;
using HutongGames.PlayMaker;

public class Action_Boss08_Atk3_Teleport : FsmStateAction {

	public int maxCount = 5;
	public float range = 8f;
	public float height = 5f;	// 距离地面的高度
	public FsmEvent finishEvent;

	private MonsterAnimationEventHandler eventHandler;
	private FaceDirectionComponent       faceDirectionComponent;
	private Enemy.Monster                controller;
	private Transform target;
	private float rangeLeft, rangeRight;
	private float heightY;
	private int currentCount;
	
	public override void Awake () {
		eventHandler           = Owner.transform.Find ( "ActionAnimator" ).GetComponent<MonsterAnimationEventHandler> ();
		faceDirectionComponent = Owner.GetComponent<FaceDirectionComponent> ();
		controller             = Owner.GetComponent<Enemy.Monster> ();
	}

	public override void OnEnter () {
		eventHandler.OnCustomAnimationEvent += OnCustomEvent;
		controller.ClearVelocity ();
		controller.ActiveGravity = false;

		target = Owner.GetComponent< NodeCanvas.Framework.Blackboard > ().GetValue< Transform > ( "Target" );

		currentCount = 0;
		
		var hit = Physics2D.Raycast ( target.position, Vector2.down, float.MaxValue, LayerMask.GetMask ( "Platform", "OneWayPlatform", "Wall" ) );

		if ( hit.collider != null ) {
			heightY = hit.collider.bounds.max.y + height;
			rangeLeft = target.position.x - range * 0.5f;
			rangeRight = target.position.x + range * 0.5f;
			AIUtils.ClampPlatformEdgePoint ( hit.collider, ref rangeLeft, ref rangeRight );
		}
		else {
			Debug.LogError ( $"Action_Boss08_Atk3_Teleport没找到平台" );
		}
		
		// 状态起始先传送一下
		Owner.transform.SetPosition ( Random.Range ( rangeLeft, rangeRight ), heightY );
	}

	public override void OnExit () {
		eventHandler.OnCustomAnimationEvent -= OnCustomEvent;
		controller.ActiveGravity            =  true;
		controller.velocity.x               =  0f;
	}
	
	private void OnCustomEvent ( string evt ) {

		if ( !evt.Equals ( "Teleport" ) ) {
			return;
		}

		if ( ++currentCount >= maxCount ) {
			Fsm.Event ( finishEvent );
			return;
		}
		
		Owner.transform.SetPosition ( Random.Range ( rangeLeft, rangeRight ), heightY );
	}

}
