using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace Assets.Scripts.AI.BehaviorTree
{
    public static class Util
    {
        /// <summary>
        /// 获取玩家相对于怪物当前位置的方向
        /// </summary>
        /// <returns>1/-1/0</returns>
        public static int GetPlayerDirectionRelative(Vector3 playerPosition, Vector3 monsterPosition)
        {
            if (playerPosition.x > monsterPosition.x)
                return 1;
            if (playerPosition.x < monsterPosition.x)
                return -1;
            return 0;
        }
        /// <summary>
        /// 获取点a相对于点b的方向
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int GetPointDirectionRelative(float a, float b)
        {
            if (a > b)
                return 1;
            if (a < b)
                return -1;
            return 0;
        }
        public static float Distance2D(Transform A, Transform B)
        {
            return Vector2.Distance(
                new Vector2(A.position.x, A.position.y),
                new Vector2(B.position.x, B.position.y));
        }
        public static float Distance2D(Vector3 A, Vector3 B)
        {
            return Vector2.Distance(A, B);
        }
        public static float DistanceX2D(Transform A, Transform B)
        {
            return Mathf.Abs(A.position.x - B.position.x);
        }
        public static float DistanceX2D(Vector3 A, Vector3 B)
        {
            return Mathf.Abs(A.x - B.x);
        }
        public static float DistanceY2D(Transform A, Transform B)
        {
            /// 1.684为精灵图的偏移值
            return Mathf.Abs(A.position.y - B.position.y);
        }
        public static float DistanceY2D(Vector3 A, Vector3 B)
        {
            return Mathf.Abs(A.y - B.y);
        }
        /// <summary>
        /// 获取平台的边界点，使用box2d碰撞盒
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static Vector2[] GetPlatformEdgePoint(Collider2D platform)
        {
            if ((platform as BoxCollider2D) != null)
                return new[]
                {
                    new Vector2(platform.bounds.min.x, platform.bounds.max.y),
                    new Vector2(platform.bounds.max.x, platform.bounds.max.y)
                };
            if ((platform as EdgeCollider2D) != null)
            {
                var eplatform = platform as EdgeCollider2D;
                var point0 = eplatform.transform.TransformPoint(eplatform.points[0].x, eplatform.points[0].y, 0);
                var point1 = eplatform.transform.TransformPoint(eplatform.points[eplatform.pointCount - 1].x, eplatform.points[eplatform.pointCount - 1].y, 0);
                return new[]
                {
                    new Vector2(point0.x, point0.y),
                    new Vector2(point1.x, point1.y)
                };
            }
            Debug.LogWarning("platform == null");
            return new[] { Vector2.zero, Vector2.zero };
        }



        static public bool IsTargetInSomethingRange(Transform target, Vector3 selfPosition, float somethingRangeX, float somethingRangeY = 0)
        {
            if (target == null)
                return false;

            return Mathf.Abs(target.position.x - selfPosition.x) < somethingRangeX
                   && Mathf.Abs(target.position.y - selfPosition.y) < somethingRangeY;
        }

        static public bool IsTargetWithinSight(Transform target, Vector3 selfPosition, int selfFaceDirection)
        {
            if (target == null)
                return false;

            return (target.position.x > selfPosition.x
                   && selfFaceDirection == 1) ||
                   (target.position.x < selfPosition.x
                    && selfFaceDirection == -1);
        }


        /// <summary>
        /// 准备目标的黑板组件
        /// </summary>
        /// <param name="monster"></param>
        static public void PrepareBlackboard ( GameObject monster ) {

            /// 怪的AI黑板相关
            var blackboard = monster.GetComponent < NodeCanvas.Framework.Blackboard >();

            if ( blackboard == null ) {
                blackboard = monster.gameObject.AddComponent<NodeCanvas.Framework.Blackboard> ();
            }

            // 初始化黑板变量
            Assets.Scripts.AI.BehaviorTree.Util.AffirmBlackboardVariable ( blackboard );

            // 设置目标
            if ( !monster.name.Contains ( "Boss" ) ) {

                // 设置怪的初始敌对目标
                Assets.Scripts.AI.BehaviorTree.Util.SetBehaviorTreeTarget ( blackboard, Assets.Scripts.AI.BehaviorTree.Util.FindTargetByDistance ( blackboard, monster.transform ) );

                /// 设置初始巡逻方向
                blackboard.AddVariable ( "InitPatrolDirection", MathUtils.RandomBool () ? -1 : 1 );

                //patrolDir = patrolDir == 1 ? -1 : 1;
            }
            else {
                // blackboard.SetValue ( "Target", GameManager.Player );
                blackboard.SetValue ( "Target", default );
            }

        }

		/// <summary>
		/// Affirm黑板变量
		/// </summary>
		/// <param name="blackboard"></param>
		static public void AffirmBlackboardVariable ( Blackboard blackboard ) {

			if ( blackboard.GetVariable < Transform >( "Target" ) == null ) {
				blackboard.AddVariable ( "Target", typeof ( Transform ) );
			}

            if ( blackboard.GetVariable < bool >( "FriendlyFire" ) == null ) {
                blackboard.AddVariable ( "FriendlyFire", false );
            }
            
            if ( blackboard.GetVariable < bool >( GlobalSymbol.FLAG_TARGET_LOCKED ) == null ) {
	            blackboard.AddVariable ( GlobalSymbol.FLAG_TARGET_LOCKED, false );
            }

			if ( blackboard.GetVariable < List<Transform> >( "Candidate Targets" ) == null ) {

				// (blackboard.AddVariable ( "Candidate Targets", new List<Transform>() )
				// 	as Variable< List<Transform> >).GetValue ().Add ( GameManager.Player );
				(blackboard.AddVariable ( "Candidate Targets", new List<Transform>() )
					as Variable< List<Transform> >).GetValue ().Add ( default );
			}

            if ( blackboard.GetValue < Transform >( "Target" ) == null ) {
                // blackboard.SetValue ( "Target", GameManager.Player );
                blackboard.SetValue ( "Target", default );
            }
		}

		/// <summary>
		/// 设置怪的当前敌对目标
		/// </summary>
		/// <param name="blackboard"></param>
		/// <param name="target"></param>
		static public void SetBehaviorTreeTarget ( Blackboard blackboard, Transform target ) {

			var targetVariable = blackboard.GetVariable < Transform >( "Target" );

			if ( targetVariable == null ) {
				targetVariable = blackboard.AddVariable ( "Target", typeof ( Transform ) ) as Variable< Transform >;
			}

			if ( target != null ) {
				targetVariable.SetValue ( target );
			}
			else {
				// targetVariable.SetValue ( GameManager.Player );
				targetVariable.SetValue ( default );
			}
		}

		/// <summary>
		/// 选择一个目标，距离优先
		/// </summary>
		static public Transform FindTargetByDistance ( Blackboard blackboard, Transform self ) {
			float minDistance = float.MaxValue;

			var candidateTargets = blackboard.GetValue < List<Transform> >( "Candidate Targets" );

			if ( candidateTargets.Count == 0 ) {
				// candidateTargets.Add ( GameManager.Player );
				candidateTargets.Add ( default );
			}

			Transform t = candidateTargets [ 0 ];

			foreach ( var ct in candidateTargets ) {
				if ( ct == null )
					continue;

				var distance = Vector2.Distance(ct.position, self.position);

				if ( distance < minDistance ) {
					t = ct;
					minDistance = distance;
				}
			}

			return t;
		}
	}
}
