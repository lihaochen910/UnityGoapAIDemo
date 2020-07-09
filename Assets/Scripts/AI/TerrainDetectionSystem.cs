using UnityEngine;

static public class TerrainDetectionSystem
{
    /// <summary>
    /// 给定位置是否与地形/墙相交?
    /// </summary>
    /// <param name="boxCenterPosition"></param>
    /// <param name="boxSize"></param>
    /// <param name="boxOffset"></param>
    /// <returns></returns>
    public static bool IntersectsWithPlatform(Vector2 boxCenterPosition, Vector2 boxSize, Vector2 boxOffset)
    {
        Collider2D obstacles = Physics2D.OverlapBox(boxCenterPosition + boxOffset, boxSize, 0, LayerMask.GetMask("Platform", "Wall"));

        if (obstacles != null)
            return true;

        return false;
    }

    /// <summary>
    /// 尝试修正穿墙物体的坐标
    /// </summary>
    public static bool IntersectionPositionCorrection(ref Vector2 boxCenterPosition, Vector2 boxSize, Vector2 boxOffset)
    {
        Collider2D obstacle = Physics2D.OverlapBox(boxCenterPosition + boxOffset, boxSize, 0, LayerMask.GetMask("Platform", "Wall"));

        if (obstacle == null)
            return false;
        else
        {
            // 判断障碍在左边还是右边
            int obstacleDir = obstacle.bounds.center.x > boxCenterPosition.x ? 1 : -1;

            // 如果障碍物在右边，则把位置向左移动
            if (obstacleDir == 1)
            {
                var offsetX = (boxCenterPosition.x + boxSize.x / 2) - obstacle.bounds.min.x;
                boxCenterPosition.x -= offsetX;
            }
            else
            {
                var offsetX = obstacle.bounds.max.x - (boxCenterPosition.x - boxSize.x / 2);
                boxCenterPosition.x += offsetX;
            }

            return true;
        }
    }

    /// <summary>
    /// 获取宝箱生成坐标
    /// </summary>
    /// <param name="rayOriginPoint"></param>
    /// <param name="boxSize"></param>
    /// <param name="boxOffset"></param>
    /// <returns></returns>
    public static Vector2 TreasureBoxTerrainDetection(Vector2 rayOriginPoint, Vector2 boxSize, Vector2 boxOffset)
    {
        // 从玩家脚下位置发射一条射线
        RaycastHit2D hit = Physics2D.Raycast(rayOriginPoint, Vector2.down, float.MaxValue, LayerMask.GetMask("Platform", "Wall"));
#if UNITY_EDITOR
        // DebugExtensions.DrawArrow(rayOriginPoint, Vector3.down, Color.cyan);
#endif
        if (hit.collider == null)
        {
            return default(Vector2);
        }

        // 确定地面的点
        Vector2 groundPoint = new Vector2(rayOriginPoint.x, hit.collider.bounds.max.y);
#if UNITY_EDITOR
        // DebugExtensions.DrawPoint(groundPoint, Color.red, 0.2f, Time.deltaTime, true);
#endif
        // 确保箱子落点
        Vector2 boxCenterPoint = new Vector2(groundPoint.x, groundPoint.y + boxSize.y / 2 - boxOffset.y);

        //DebugExtensions.DebugPoint(boxCenterPoint, Color.yellow, 0.4f, Time.deltaTime, true);

        //Debug.Log($"地面:{hit.collider.gameObject.name} {groundPoint}");

        // 在箱子中心位置使用矩形判断左右
        Collider2D[] obstacles = Physics2D.OverlapBoxAll(boxCenterPoint, boxSize, 0, LayerMask.GetMask("Platform", "Wall"));

        foreach (var obstacle in obstacles)
        {
            if (obstacle == hit.collider)
                continue;

            //Debug.Log($"障碍物:{obstacle.name}");

            // 判断障碍在左边还是右边
            int obstacleDir = obstacle.bounds.center.x > boxCenterPoint.x ? 1 : -1;

            // 如果障碍物在右边，则把箱子出生位置向左移动
            if (obstacleDir == 1)
            {
                var offsetX = (boxCenterPoint.x + boxSize.x / 2) - obstacle.bounds.min.x;
                boxCenterPoint.x -= offsetX;
                //Debug.Log($"障碍物在右边 坐标：{boxCenterPoint}");
#if UNITY_EDITOR
                // DebugExtensions.DrawBounds(obstacle.bounds, Color.gray);
                // DebugExtensions.DrawPoint(boxCenterPoint, Color.yellow, 0.4f, Time.deltaTime, true);
                // DebugExtensions.DrawBox(boxCenterPoint, boxSize, 0, Color.green, Time.deltaTime);
#endif
                return boxCenterPoint;
            }
            else
            {
                var offsetX = obstacle.bounds.max.x - (boxCenterPoint.x - boxSize.x / 2);
                boxCenterPoint.x += offsetX;
                //Debug.Log($"障碍物在左边 坐标：{boxCenterPoint}");
#if UNITY_EDITOR
                // DebugExtensions.DrawBounds(obstacle.bounds, Color.grey);
                // DebugExtensions.DrawPoint(boxCenterPoint, Color.yellow, 0.4f, Time.deltaTime, true);
                // DebugExtensions.DrawBox(boxCenterPoint, boxSize, 0, Color.green, Time.deltaTime);
#endif
                return boxCenterPoint;
            }
        }
#if UNITY_EDITOR
        // DebugExtensions.DrawBounds(hit.collider.bounds, Color.gray);
        // DebugExtensions.DrawPoint(boxCenterPoint, Color.green, 0.4f, Time.deltaTime, true);
        // DebugExtensions.DrawBox(boxCenterPoint, boxSize, 0, Color.gray, Time.deltaTime);
#endif
        //Debug.Log($"坐标：{boxCenterPoint}");
        return boxCenterPoint;
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
		RaycastHit2D hit = Physics2D.Raycast((Vector2)targetPosition + boxOffset * targetScale, Vector2.down, float.MaxValue, LayerMask.GetMask("Platform", "OneWayPlatform", "Wall"));
		//DebugExtensions.DrawBox ( ( Vector2 )targetPosition + boxOffset * targetScale, boxSize, 0f, Color.red, 1f );

		if ( hit.collider == null ) {
			Debug.LogError ( "无法确定地面，TreasureBoxManager::TerrainOverlapFix()可能无法返回正确的结果" );
		}

		// 确定物体在地面上，这样可以排除上下的干扰
		Vector2 groundPoint = new Vector2(targetPosition.x, hit.collider != null ? hit.collider.bounds.max.y : targetPosition.y);

		//Debug.Log($"groundPoint: {groundPoint.x},{groundPoint.y}");

		// 物体Body碰撞体中心点
		Vector2 boxCenterPoint = new Vector2(targetPosition.x + boxOffset.x * targetScale.x, groundPoint.y + boxSize.y * targetScale.y / 2.0f);

		//Debug.Log($"origin: {targetPosition.x},{targetPosition.y} boxCenterPoint: {boxCenterPoint.x},{boxCenterPoint.y}");

		// 在物体位置使用矩形判断左右
		Collider2D[] obstacles = Physics2D.OverlapBoxAll(boxCenterPoint, new Vector2 ( boxSize.x * targetScale.x, boxSize.y * targetScale.y ), 0, LayerMask.GetMask("Platform", "Wall"));
		//DebugExtensions.DrawBox ( boxCenterPoint, new Vector2 ( boxSize.x * targetScale.x, boxSize.y * targetScale.y ), 0f, Color.yellow, 1f );

		foreach ( var obstacle in obstacles ) {
			// 忽略上下的干扰
			if ( obstacle == hit.collider )
				continue;

			// 判断障碍在左边还是右边
			int obstacleDir = obstacle.bounds.center.x > boxCenterPoint.x ? 1 : -1;

			// 如果障碍物在右边，则把物体位置向左移动
			if ( obstacleDir == 1 ) {
				var offsetX = (boxCenterPoint.x + boxSize.x * targetScale.x / 2) - obstacle.bounds.min.x;
				boxCenterPoint.x -= offsetX;
				//Debug.Log($"TerrainDetectionSystem::TerrainOverlapFix() 目标与右边平台障碍物相交 {obstacle.transform.GetPath()} {offsetX} 修正坐标：{boxCenterPoint - boxOffset} 源: {targetPosition}");
				return boxCenterPoint - boxOffset * targetScale;
			}
			else {
				var offsetX = obstacle.bounds.max.x - (boxCenterPoint.x - boxSize.x * targetScale.x / 2);
				boxCenterPoint.x += offsetX;
				//DebugExtensions.DrawArrow ( boxCenterPoint - boxOffset * targetScale, Vector3.up, Color.cyan, 1f );
				//Debug.Log ( $"TerrainDetectionSystem::TerrainOverlapFix() 目标与左边平台障碍物相交 {obstacle.transform.GetPath ()} {offsetX} 修正坐标：{boxCenterPoint} 源: {targetPosition}" );
				return boxCenterPoint - boxOffset * targetScale;
			}
		}

		//Debug.Log($"TerrainOverlapFix() return {boxCenterPoint - boxOffset} boxCenterPoint: {boxCenterPoint}");

		return boxCenterPoint - boxOffset * targetScale;
	}

	public static Vector2 TerrainOverlapHorizontalFix (Vector3 targetPosition, Vector2 boxSize, Vector2 boxOffset, Vector2 targetScale) {

		// 物体Body碰撞体中心点
		Vector2 boxCenterPoint = new Vector2 ( targetPosition.x + boxOffset.x * targetScale.x, targetPosition.y + boxOffset.y * targetScale.y );

		// 在物体位置使用矩形判断左右
		Collider2D[] obstacles = Physics2D.OverlapBoxAll ( boxCenterPoint, new Vector2 ( boxSize.x * targetScale.x, boxSize.y * targetScale.y ), 0, LayerMask.GetMask ( "Platform", "Wall" ) );

		foreach ( var obstacle in obstacles ) {

			// 判断障碍在左边还是右边
			int obstacleDir = obstacle.bounds.center.x > boxCenterPoint.x ? 1 : -1;

			// 如果障碍物在右边，则把物体位置向左移动
			if ( obstacleDir == 1 ) {
				var offsetX = ( boxCenterPoint.x + boxSize.x * targetScale.x / 2 ) - obstacle.bounds.min.x;
				boxCenterPoint.x -= offsetX;
				return boxCenterPoint - boxOffset * targetScale;
			}
			else {
				var offsetX = obstacle.bounds.max.x - ( boxCenterPoint.x - boxSize.x * targetScale.x / 2 );
				boxCenterPoint.x += offsetX;
				return boxCenterPoint - boxOffset * targetScale;
			}
		}

		return boxCenterPoint - boxOffset * targetScale;
	}
}
