using Assets.Scripts.AI.BehaviorTree;
using UnityEngine;
using HutongGames.PlayMaker;
using NodeCanvas.Framework;
using RPGStatSystem;


namespace Enemy {
	
	public class Action_MoveToRange : FsmStateAction {

		public string Key_MOVE_SPEED = "RunSpeed";
		public string KEY_RANGE_X;
		public float okDistance = 0.25f;
		public bool dynamicMove;
		public bool faceToTarget = true;
		public bool dynamicFaceTo = true;
		public FsmEvent finishEvent;

		private RPGStatCollection statCollection;
		private FaceDirectionComponent faceDirectionComponent;
		private Monster platformController;
		private Blackboard blackboard;
		private Transform target;
		private float moveTargetX;

		public override void Awake () {
			statCollection = Owner.GetComponent< RPGStatCollection > ();
			faceDirectionComponent = Owner.GetComponent< FaceDirectionComponent > ();
			platformController = Owner.GetComponent< Monster > ();
			blackboard = Owner.GetComponent< Blackboard > ();
		}

		public override void OnEnter () {
			
			target = blackboard.GetValue< Transform > ( "Target" );
			
			if ( !dynamicMove ) {
				var relDir = Util.GetPlayerDirectionRelative ( target.position, platformController.transform.position );
				moveTargetX = target.position.x - relDir * statCollection.GetStatValue ( KEY_RANGE_X );
			}

			if ( !dynamicFaceTo ) {
				faceDirectionComponent.SetFaceDirection ( faceToTarget
					? Util.GetPlayerDirectionRelative ( target.position, platformController.transform.position )
					: -Util.GetPlayerDirectionRelative ( target.position, platformController.transform.position ) );
			}
		}

		public override void OnUpdate () {

			if ( dynamicMove ) {
				var relDir = Util.GetPlayerDirectionRelative ( target.position, platformController.transform.position );
				moveTargetX = target.position.x - relDir * statCollection.GetStatValue ( KEY_RANGE_X );
			}
			
			if ( dynamicFaceTo ) {
				faceDirectionComponent.SetFaceDirection ( faceToTarget
					? Util.GetPlayerDirectionRelative ( target.position, platformController.transform.position )
					: -Util.GetPlayerDirectionRelative ( target.position, platformController.transform.position ) );
			}
			
			var distance = moveTargetX - platformController.transform.position.x;

			if ( Mathf.Abs ( distance ) <= okDistance ) {
				// Debug.Log ( $"Action_MoveToRange: 已移动到指定范围 distance = {distance} moveTargetX = {moveTargetX} self = {platformController.transform.position.x}" );
				Fsm.Event ( finishEvent );
				return;
			}

			if ( distance > 0f && platformController.controller.collisionState.right
			     || distance < 0f && platformController.controller.collisionState.left ) {
				// Debug.Log ( $"Action_MoveToRange: 移动遇到障碍物" );
				Fsm.Event ( finishEvent );
				return;
			}

			if ( distance < 0f ) {
				platformController.Move ( new Vector2 ( -statCollection.GetStatValue ( Key_MOVE_SPEED ) * CupheadTime.Delta[ CupheadTime.Layer.Enemy ], -0.01f ) );
			}
			else {
				platformController.Move ( new Vector2 ( statCollection.GetStatValue ( Key_MOVE_SPEED ) * CupheadTime.Delta[ CupheadTime.Layer.Enemy ], -0.01f ) );
			}
		}
		
#if UNITY_EDITOR
		public override void OnGUI () {
			GUI.Label ( new Rect ( 0, 280, 400, 100 ),
				$"Action_MoveToRange：{Owner.transform.position} {moveTargetX}" );
			GUI.Label ( new Rect ( 0, 300, 400, 100 ),
				$"Action_MoveToRange distance：{Util.DistanceX2D ( target, platformController.transform )}" );
		}
#endif
	}
}
