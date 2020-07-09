using UnityEngine;
using HutongGames.PlayMaker;
using NodeCanvas.Framework;
using RPGStatSystem;
using Enemy;


public class Action_Boss08_Teleport : FsmStateAction {
	
	public const float LINE_LENGTH = 4.5f;
	
	public string KEY_RANGE_X;
	public float   height = 4f;

	private IBlackboard blackboard;
	private BoxCollider2D boxCollider2D;
	private RPGStatCollection statCollection;
	private Monster controller;

	public override void Awake () {
		blackboard = Owner.GetComponent< IBlackboard > ();
		boxCollider2D = Owner.GetComponent< BoxCollider2D > ();
		statCollection = Owner.GetComponent< RPGStatCollection > ();
		controller = Owner.GetComponent< Monster > ();
	}

	public override void OnEnter () {

		controller.ClearVelocity ();
		controller.ActiveGravity = false;
		
		var target = blackboard.GetValue< Transform > ( "Target" );
		var dir = 0;
		var groundY = 0f;
		var startPosition = Owner.transform.position;
		bool leftBlocked = false;
		bool rightBlocked = false;

		var range = statCollection.GetStatValue ( KEY_RANGE_X );
		
		// 传送方向选择
		var hit = Physics2D.Raycast ( target.position, Vector2.left, range, LayerMask.GetMask ( "Platform", "Wall" ) );
		if ( hit.transform != null ) {
			leftBlocked = true;
		}
		hit = Physics2D.Raycast ( target.position, Vector2.right, range, LayerMask.GetMask ( "Platform", "Wall" ) );
		if ( hit.transform != null ) {
			rightBlocked = true;
		}
		
		hit = Physics2D.Raycast ( target.position, Vector2.down, range, LayerMask.GetMask ( "Platform", "Wall" ) );
		if ( hit.collider != null ) {
			groundY = hit.collider.bounds.max.y;
		}

		if ( leftBlocked && !rightBlocked ) {
			dir = 1;
		}
		else if ( !leftBlocked && rightBlocked ) {
			dir = -1;
		}
		if ( dir == 0 ) {
			dir = MathUtils.RandomBool () ? 1 : -1;
		}
		
		// 坐标修正
		var teleportPoint = new Vector2 ( target.position.x + dir * range, target.position.y + height );
		if ( height < 0 ) {
			teleportPoint.y = groundY;
		}
		teleportPoint = TerrainDetectionSystem.TerrainOverlapHorizontalFix ( teleportPoint, boxCollider2D.size,
			boxCollider2D.offset, Owner.transform.localScale );
		
		// 特效
		// EffectManager.ApplySpecificEffectAt ( "Boss_08_yuandian", Owner.transform.position );
		
		Owner.transform.SetPosition ( teleportPoint.x, teleportPoint.y );
		
		// var line = EffectManager.ApplySpecificEffectAt ( "Boss_08_chuansongdianlu", ( startPosition + Owner.transform.position ) / 2f );
		// line.transform.SetEulerAngles ( null, null, MathUtils.DirectionToAngle ( ( Owner.transform.position - startPosition ).normalized ) );
		// line.transform.SetScale ( 1f / LINE_LENGTH * Vector2.Distance ( startPosition, Owner.transform.position ) );
		//
		// // 特效
		// EffectManager.ApplySpecificEffectAt ( "Boss_08_yuandian", Owner.transform.position );
	}

	public override void OnExit () {
		controller.ActiveGravity = true;
	}
}
