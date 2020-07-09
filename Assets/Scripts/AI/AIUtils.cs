using UnityEngine;


/// <summary>
/// 与AI相关的有帮助的辅助方法
/// </summary>
public static class AIUtils {
	
	public static float Distance2D ( Vector3 A, Vector3 B ) {
		return Vector2.Distance ( A, B );
	}

	public static float DistanceX2D ( Transform A, Transform B ) {
		return Mathf.Abs ( A.position.x - B.position.x );
	}

	public static float DistanceX2D ( Vector3 A, Vector3 B ) {
		return Mathf.Abs ( A.x - B.x );
	}

	public static float DistanceY2D ( Transform A, Transform B ) {
		/// 1.684为精灵图的偏移值
		return Mathf.Abs ( A.position.y - B.position.y );
	}

	public static float DistanceY2D ( Vector3 A, Vector3 B ) {
		return Mathf.Abs ( A.y - B.y );
	}


	/// <summary>
	/// 获取目标相对于自身当前位置的方向
	/// </summary>
	/// <returns>1/-1/0</returns>
	public static int GetTargetDirectionRelative ( Vector3 targetPosition, Vector3 selfPosition ) {
		if ( targetPosition.x > selfPosition.x )
			return 1;
		if ( targetPosition.x < selfPosition.x )
			return -1;
		return 0;
	}

	/// <summary>
	/// 获取平台的边界点，使用box2d碰撞盒
	/// </summary>
	/// <param name="platform"></param>
	/// <returns></returns>
	public static Vector2[] GetPlatformEdgePoint ( Collider2D platform ) {
		if ( ( platform as BoxCollider2D ) != null )
			return new[] {
				new Vector2 ( platform.bounds.min.x, platform.bounds.max.y ),
				new Vector2 ( platform.bounds.max.x, platform.bounds.max.y )
			};
		if ( ( platform as EdgeCollider2D ) != null ) {
			var eplatform = platform as EdgeCollider2D;
			var point0 =
				eplatform.transform.TransformPoint ( eplatform.points [ 0 ].x, eplatform.points [ 0 ].y, 0 );
			var point1 = eplatform.transform.TransformPoint ( eplatform.points [ eplatform.pointCount - 1 ].x,
				eplatform.points [ eplatform.pointCount - 1 ].y, 0 );
			return new[] {
				new Vector2 ( point0.x, point0.y ),
				new Vector2 ( point1.x, point1.y )
			};
		}

		Debug.LogWarning ( "platform == null" );
		return new[] { Vector2.zero, Vector2.zero };
	}


	/// <summary>
	/// 如果物体与平台相交，则返回修正后位置坐标
	/// </summary>
	/// <param name="targetPosition">Transform.position</param>
	/// <param name="boxSize">碰撞盒大小</param>
	/// <param name="boxOffset">碰撞盒偏移</param>
	public static Vector2 TerrainOverlapFix ( Vector3 targetPosition, Vector2 boxSize, Vector2 boxOffset, Vector2 targetScale ) {
		//Debug.Log($"in_targetPosition:{targetPosition} in_boxSize:{boxSize} in_boxOffset:{boxOffset} in_targetScale:{targetScale}");

		// 从中心位置发射一条射线
		RaycastHit2D hit = Physics2D.Raycast ( ( Vector2 ) targetPosition + boxOffset * targetScale, Vector2.down,
			float.MaxValue, LayerMask.GetMask ( "Platform", "OneWayPlatform", "Wall" ) );
		//DebugExtensions.DrawBox ( ( Vector2 )targetPosition + boxOffset * targetScale, boxSize, 0f, Color.red, 1f );

		if ( hit.collider == null ) {
			Debug.LogError ( "无法确定地面，TreasureBoxManager::TerrainOverlapFix()可能无法返回正确的结果" );
		}

		// 确定物体在地面上，这样可以排除上下的干扰
		Vector2 groundPoint = new Vector2 ( targetPosition.x,
			hit.collider != null ? hit.collider.bounds.max.y : targetPosition.y );

		//Debug.Log($"groundPoint: {groundPoint.x},{groundPoint.y}");

		// 物体Body碰撞体中心点
		Vector2 boxCenterPoint = new Vector2 ( targetPosition.x + boxOffset.x * targetScale.x,
			groundPoint.y + boxSize.y * targetScale.y / 2.0f );

		//Debug.Log($"origin: {targetPosition.x},{targetPosition.y} boxCenterPoint: {boxCenterPoint.x},{boxCenterPoint.y}");

		// 在物体位置使用矩形判断左右
		Collider2D[] obstacles = Physics2D.OverlapBoxAll ( boxCenterPoint,
			new Vector2 ( boxSize.x * targetScale.x, boxSize.y * targetScale.y ), 0,
			LayerMask.GetMask ( "Platform", "Wall" ) );
		//DebugExtensions.DrawBox ( boxCenterPoint, new Vector2 ( boxSize.x * targetScale.x, boxSize.y * targetScale.y ), 0f, Color.yellow, 1f );

		foreach ( var obstacle in obstacles ) {
			// 忽略上下的干扰
			if ( obstacle == hit.collider )
				continue;

			// 判断障碍在左边还是右边
			int obstacleDir = obstacle.bounds.center.x > boxCenterPoint.x ? 1 : -1;

			// 如果障碍物在右边，则把物体位置向左移动
			if ( obstacleDir == 1 ) {
				var offsetX = ( boxCenterPoint.x + boxSize.x * targetScale.x / 2 ) - obstacle.bounds.min.x;
				boxCenterPoint.x -= offsetX;
				//Debug.Log($"TerrainDetectionSystem::TerrainOverlapFix() 目标与右边平台障碍物相交 {obstacle.transform.GetPath()} {offsetX} 修正坐标：{boxCenterPoint - boxOffset} 源: {targetPosition}");
				return boxCenterPoint - boxOffset * targetScale;
			}
			else {
				var offsetX = obstacle.bounds.max.x - ( boxCenterPoint.x - boxSize.x * targetScale.x / 2 );
				boxCenterPoint.x += offsetX;
				//DebugExtensions.DrawArrow ( boxCenterPoint - boxOffset * targetScale, Vector3.up, Color.cyan, 1f );
				//Debug.Log ( $"TerrainDetectionSystem::TerrainOverlapFix() 目标与左边平台障碍物相交 {obstacle.transform.GetPath ()} {offsetX} 修正坐标：{boxCenterPoint} 源: {targetPosition}" );
				return boxCenterPoint - boxOffset * targetScale;
			}
		}

		//Debug.Log($"TerrainOverlapFix() return {boxCenterPoint - boxOffset} boxCenterPoint: {boxCenterPoint}");

		return boxCenterPoint - boxOffset * targetScale;
	}


	/// <summary>
	/// 选择一个远离目标的传送点
	/// </summary>
	/// <param name="target"></param>
	/// <param name="self"></param>
	/// <param name="bodyCollider"></param>
	/// <param name="distance"></param>
	/// <returns></returns>
	public static Vector2 SelectPositionForTeleportAwayFrom ( Transform target, Transform self, BoxCollider2D bodyCollider, float distance ) {
		
		Vector2 safePosition         = Vector2.zero;
		Vector2 currentPlatformRange = Vector2.zero;

		// 获取目标所在平台
		var hit = Physics2D.Raycast ( target.position, Vector2.down, float.MaxValue,
			LayerMask.GetMask ( "Platform", "OneWayPlatform", "Wall" ) );

		// 确定平台的两个端点(平台X轴范围)
		if ( hit.collider != null ) {
			currentPlatformRange.x = hit.collider.bounds.center.x - hit.collider.bounds.extents.x;
			currentPlatformRange.y = hit.collider.bounds.center.x + hit.collider.bounds.extents.x;

			//Debug.Log($"Action_30005_Move 平台两端({currentPlatformRange.x}, {currentPlatformRange.y})");
		}

		// 尝试移动到SafeRange外
		float leftX  = target.position.x - distance;
		float rightX = target.position.x + distance;

		//Debug.Log($"Action_30005_Move 拟定X轴位置 左:{leftX} 右:{rightX}");

		bool leftFirst = MathUtils.RandomBool ();

		// 优先选择目标面朝方向的反方向
		if ( leftFirst ) {
			// 对比左
			if ( leftX >= currentPlatformRange.x ) {
				// OK
				self.position = new Vector3 ( leftX, self.position.y );
				var result = TerrainOverlapFix ( self.position, bodyCollider.size, bodyCollider.offset, self.localScale );

				//Debug.Log($"Action_30005_Move 通常情况 - 左 {new Vector2(leftX, Owner.transform.position.y)} 校正结果:{result}");

				return result;
			}

			// 对比右
			if ( rightX <= currentPlatformRange.y ) {
				// OK
				//Debug.Log($"Action_30005_Move 通常情况 - 右 {new Vector2(rightX, Owner.transform.position.y)}");
				self.position = new Vector3 ( rightX, self.position.y );
				return TerrainOverlapFix ( self.position, bodyCollider.size, bodyCollider.offset, self.localScale );
			}
		}
		else {
			// 对比右
			if ( rightX <= currentPlatformRange.y ) {
				// OK
				self.position = new Vector3 ( rightX, self.position.y );
				return TerrainOverlapFix ( self.position, bodyCollider.size, bodyCollider.offset, self.localScale );
			}

			// 对比左
			if ( leftX >= currentPlatformRange.x ) {
				// OK
				self.position = new Vector3 ( leftX, self.position.y );
				return TerrainOverlapFix ( self.position, bodyCollider.size, bodyCollider.offset, self.localScale );
			}
		}


		// 如果无法正常位移到SafeRange外，尝试对比平台左右端点与目标的距离，取距离大的位置
		if ( Mathf.Abs ( currentPlatformRange.x - target.position.x ) >
		     Mathf.Abs ( currentPlatformRange.y - target.position.x ) ) {
			// 取平台左端
			//Debug.Log($"Action_30005_Move 复杂情况 - 平台左");
			return TerrainOverlapFix ( new Vector3 ( currentPlatformRange.x, self.position.y ), bodyCollider.size, bodyCollider.offset, self.localScale );
		}
		else {
			// 取平台右端
			//Debug.Log($"Action_30005_Move 复杂情况 - 平台右");
			return TerrainOverlapFix ( new Vector3 ( currentPlatformRange.y, self.position.y ), bodyCollider.size, bodyCollider.offset, self.localScale );
		}
	}


	/// <summary>
	/// 将两个点限制在平台内
	/// </summary>
	/// <param name="platform"></param>
	/// <param name="leftPoint"></param>
	/// <param name="rightPoint"></param>
	public static void ClampPlatformEdgePoint ( Collider2D platform, ref float leftPoint, ref float rightPoint ) {

		var edgePoint = GetPlatformEdgePoint ( platform );
		float edgePointLeft = edgePoint[ 0 ].x;
		float edgePointRight = edgePoint[ 1 ].x;

		if ( leftPoint >= edgePointLeft && rightPoint <= edgePointRight ) {
			// Do Nothing
			
		}
		else {
			// 如果范围大于地形边界范围
			if ( Mathf.Abs ( rightPoint - leftPoint ) >= Mathf.Abs ( edgePointRight - edgePointLeft ) ) {
				rightPoint = edgePointRight;
				leftPoint  = edgePointLeft;
			}
			else {
				if ( rightPoint > edgePointRight ) {
					var ppLeft  = leftPoint;
					var ppRight = rightPoint;

					rightPoint = edgePointRight;
					leftPoint  = ppLeft - Mathf.Abs ( ppRight - edgePointRight );
				}
				else {
					var ppLeft  = leftPoint;
					var ppRight = rightPoint;

					rightPoint = ppRight + Mathf.Abs ( edgePointLeft - ppLeft );
					leftPoint  = edgePointLeft;
				}
			}
		}
	}

	
	/// <summary>
	/// 获取下方平台的交点
	/// </summary>
	/// <returns></returns>
	public static Vector3 GetDownPlatformPoint ( Vector3 pos ) {
		var hit = Physics2D.Raycast ( pos, Vector2.down, float.MaxValue, LayerMask.GetMask ( "Platform", "OneWayPlatform", "Wall" ) );
		return hit.point;
	}
}
