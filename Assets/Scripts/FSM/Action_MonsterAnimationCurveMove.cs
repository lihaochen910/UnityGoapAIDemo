using HutongGames.PlayMaker;
using UnityEngine;

namespace Enemy {
	public class Action_MonsterAnimationCurveMove : MonsterFsmStateAction {

		public FsmAnimationCurve curveX;
		public float curveXSpeed = 1f;
		public FsmAnimationCurve curveY;
		public float curveYSpeed = 1f;
		public float time = 1f;
		public FsmEvent completedEvent;

		private float _timer;
		private float _totalX;
		private float _totalY;

		public override void Awake () {
			base.Awake ();
			Keyframe keyframeX = curveX.curve.keys[curveX.curve.length - 1];
			_totalX = keyframeX.time; 
			Keyframe keyframeY = curveY.curve.keys[curveY.curve.length - 1];
			_totalY = keyframeY.time;
		}

		public override void OnEnter () {
			_timer = 0f;
		}

		public override void OnUpdate () {
			_timer += CupheadTime.Delta[CupheadTime.Layer.Enemy];

			m_Monster.Move ( new Vector2 (
				curveX.curve.Evaluate ( _timer / time * _totalX ) * curveXSpeed * m_Monster.FaceDirection * CupheadTime.Delta[CupheadTime.Layer.Enemy]
				, curveY.curve.Evaluate ( _timer / time * _totalY ) * curveYSpeed * CupheadTime.Delta[CupheadTime.Layer.Enemy]
			) );

			if ( _timer > time && completedEvent != null ) {
				Fsm.Event ( completedEvent );
			}
		}
	}
}
