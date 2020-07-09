using UnityEngine;
using HutongGames.PlayMaker;
using NodeCanvas.Framework;
using RPGStatSystem;


namespace Enemy {
	
	public class Action_DashTowardTarget : FsmStateAction {

		public bool TowardTarget = true;
		public int FaceDirection = 1;	// 1-面朝目标，-1-背对目标，0-无操作
		public string Key_DASH_SPEED = "DashSpeed";
		public float dashDurationMin = 1f;
		public float dashDurationMax = 1f;
		public FsmEvent finishEvent;

		private RPGStatCollection statCollection;
		private FaceDirectionComponent faceDirectionComponent;
		private Monster platformController;
		private Blackboard blackboard;
		private Transform target;
		private float _dashDuration;
		private float _dashDirection;
		private float _timer;

		public override void Awake () {
			statCollection = Owner.GetComponent< RPGStatCollection > ();
			faceDirectionComponent = Owner.GetComponent< FaceDirectionComponent > ();
			platformController = Owner.GetComponent< Monster > ();
			blackboard = Owner.GetComponent< Blackboard > ();
		}

		public override void OnEnter () {
			
			target = blackboard.GetValue< Transform > ( "Target" );
			
			_dashDuration = Random.Range ( dashDurationMin, dashDurationMax );
			_timer        = 0f;
			
			var dir = AIUtils.GetTargetDirectionRelative ( target.position, Owner.transform.position );

			if ( TowardTarget ) {
				_dashDirection = dir;
			}
			else {
				_dashDirection = -dir;
			}
			
			if ( FaceDirection == 1 ) {
				faceDirectionComponent.SetFaceDirection ( dir );
			}
			if ( FaceDirection == -1 ) {
				faceDirectionComponent.SetFaceDirection ( -dir );
			}

			platformController.ClearVelocity ();
			platformController.ActiveGravity = false;
		}

		public override void OnExit () {
			platformController.ActiveGravity = true;
			platformController.velocity.x = 0f;
		}

		public override void OnUpdate () {

			_timer += CupheadTime.Delta[CupheadTime.Layer.Enemy];

			if ( _timer > _dashDuration ) {
				Fsm.Event ( finishEvent );
				return;
			}
			
			var movementX = _dashDirection * statCollection.GetStatValue ( Key_DASH_SPEED ) * CupheadTime.Delta[CupheadTime.Layer.Enemy];
			platformController.Move ( new Vector2 ( movementX, 0f ) );
		}
		
#if UNITY_EDITOR
		public override void OnGUI () {
			GUI.Label ( new Rect ( 0, 280, 400, 100 ),
				$"Action_DashTowardTarget：{Owner.transform.position} {_dashDirection}" );
			GUI.Label ( new Rect ( 0, 300, 400, 100 ),
				$"Action_DashTowardTarget durition：{_dashDuration}" );
		}
#endif
	}
}
