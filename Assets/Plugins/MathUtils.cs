using System;
using UnityEngine;
using Random = UnityEngine.Random;


/// <summary>
/// 用于游戏开发的数学计算函数库
/// </summary>
public static class MathUtils {
    
    private const float DEGREES = 180.0f / Mathf.PI;
    private const float RADIANS = Mathf.PI / 180.0f;
    
    public static float GetPercentage ( float min, float max, float t ) {
        return ( float )( ( ( double )t - ( double )min ) / ( ( double )max - ( double )min ) );
    }

    public static float RadToDeg ( float aAngle ) {
        return aAngle * DEGREES;
    }

    public static float DegToRad ( float aAngle ) {
        return aAngle * RADIANS;
    }

    public static float AngleDeg ( float aX1, float aY1, float aX2, float aY2 ) {
        return Mathf.Atan2 ( aY2 - aY1, aX2 - aX1 ) * DEGREES;
    }

    public static int PlusOrMinus () {
        return ( double )Random.value > 0.5 ? 1 : -1;
    }

    public static float ExpRandom ( float mean ) {
        return -Mathf.Log ( Random.Range ( 0.0f, 1f ) ) * mean;
    }

    public static bool RandomBool () {
        return ( double )Random.value > 0.5;
    }

    /// <summary>
    /// 随机概率
    /// </summary>
    /// <param name="probability">0~100</param>
    public static bool RandomProbability ( int probability ) {
        return Random.Range ( 1, 101 ) <= probability;
    }

    public static bool RandomProbability ( float probability ) {
        return Random.Range ( 1, 101 ) <= probability * 100f;
    }

    public static Vector2 RandomPointInUnitCircle () {
        return MathUtils.AngleToDirection ( Random.Range ( 0.0f, 360f ) ) * Mathf.Sqrt ( Random.Range ( 0.0f, 1f ) );
    }

	public static Vector2 RandomPointInRectangle ( Vector2 center, float width, float height ) {
		float minX = center.x - width * 0.5f;
		float maxX = center.x + width * 0.5f;
		float minY = center.y - height * 0.5f;
		float maxY = center.y + height * 0.5f;
		return new Vector2 ( Random.Range ( minX, maxX ), Random.Range ( minY, maxY ) );
	}

    public static float DirectionToAngle ( Vector2 direction ) {
        return ( float )( ( double )Mathf.Atan2 ( direction.y, direction.x ) * 360.0 / 6.28318548202515 );
    }

    /// <summary>
    /// 使用right作为起始方向，+逆时针旋转，-顺时针旋转
    /// </summary>
    /// <param name="angle"></param>
    public static Vector2 AngleToDirection ( float angle ) {
        float f = ( float )( ( double )angle * 3.14159274101257 * 2.0 / 360.0 );
        return new Vector2 ( Mathf.Cos ( f ), Mathf.Sin ( f ) );
    }

    public static Vector2 RotatePointDeg ( float aPointX, float aPointY, float aPivotX, float aPivotY, float aAngle ) {
        aAngle = -aAngle * RADIANS;
        float   dx     = aPointX - aPivotX;
        float   dy     = aPivotY - aPointY;
        Vector2 result = new Vector2 ();
        result.x = aPivotX + Mathf.Cos ( aAngle ) * dx - Mathf.Sin ( aAngle ) * dy;
        result.y = aPivotY - ( Mathf.Sin ( aAngle ) * dx + Mathf.Cos ( aAngle ) * dy );
        return result;
    }

    //public static Vector2 Rotate(Vector2 vector, float angle)
    //{
    //    float currentAngle = DirectionToAngle(vector);
    //    currentAngle += angle;
    //    return AngleToDirection(currentAngle);
    //}

    public static Vector2 RandomCircleEdgePoint(Vector2 center, float radius, float minAngle, float maxAngle)
    {
        //var angle_1 = Random.Range(minAngle, maxAngle);
        //var angle_2 = Random.Range(minAngle, maxAngle);

        //Debug.Log($"RandomCircleEdgePoint {radius} {angle_1} {angle_2}");

        //return new Vector2(
        //    center.x + radius * Mathf.Cos(angle_1 * Mathf.PI / 180f),
        //    center.y + radius * Mathf.Sin(angle_2 * Mathf.PI / 180f)
        //);

        return new Vector2(
            center.x + radius * Mathf.Cos(Random.Range(minAngle, maxAngle) * Mathf.PI / 180f),
            center.y + radius * Mathf.Sin(Random.Range(minAngle, maxAngle) * Mathf.PI / 180f)
        );
    }

    public static bool CircleContains ( Vector2 center, float radius, Vector2 point ) {
        return ( double )Mathf.Pow ( point.x - center.x, 2f ) + ( double )Mathf.Pow ( point.y - center.y, 2f ) <
               ( double )Mathf.Pow ( radius, 2f );
    }

    /// <summary>
    /// 给定圆形是否与矩形相交？
    /// </summary>
    public static bool RectangleIntersectCircle(
        Vector2 circleCenter, float radius,
        Vector2 leftTop, Vector2 rightTop, Vector2 rightBottom, Vector2 leftBottom)
    {
        bool pointInRectangle = new Rect(leftTop, new Vector2(Mathf.Abs(rightTop.x - leftTop.x), Mathf.Abs(rightTop.y - rightBottom.y))).Contains(circleCenter);

        return pointInRectangle ||
            CircleIntersectLine(circleCenter, radius, leftTop, rightTop) ||
            CircleIntersectLine(circleCenter, radius, rightTop, rightBottom) ||
            CircleIntersectLine(circleCenter, radius, rightBottom, leftBottom) ||
            CircleIntersectLine(circleCenter, radius, leftBottom, leftTop);
    }

    /// <summary>
    /// 圆形是否与给定直线相交
    /// </summary>
    /// <param name="circleCenter"></param>
    /// <param name="radius"></param>
    /// <param name="A"></param>
    /// <param name="B"></param>
    public static bool CircleIntersectLine ( Vector2 circleCenter, float radius, Vector2 point1, Vector2 point2 ) {
        float dx, dy, A, B, C, det, t;

        dx = point2.x - point1.x;
        dy = point2.y - point1.y;

        A = dx * dx + dy * dy;
        B = 2 * ( dx * ( point1.x - circleCenter.x ) + dy * ( point1.y - circleCenter.y ) );
        C = ( point1.x - circleCenter.x ) * ( point1.x - circleCenter.x ) +
            ( point1.y - circleCenter.y ) * ( point1.y - circleCenter.y ) -
            radius * radius;

        det = B * B - 4 * A * C;

        if ( ( A <= 0.0000001 ) || ( det < 0 ) ) {
            return false;
        }

        return true;
    }

    public static bool CircleIntersectRectangle(
         Vector2 circleCenter, float radius,
         Vector2 leftTop, Vector2 rightTop, Vector2 rightBottom, Vector2 leftBottom)
    {
        var rect = new Rect(leftTop, new Vector2(Mathf.Abs(rightTop.x - leftTop.x), Mathf.Abs(rightTop.y - rightBottom.y)));
        return CircleIntersectRectangle(circleCenter, radius, rect);
    }

    public static bool CircleIntersectRectangle ( Vector2 circleCenter, float radius, Rect rect ) {
        Vector2 v = Vector2.Max ( rect.center - circleCenter, -( rect.center - circleCenter ) );
        Vector2 u = Vector2.Max ( v - rect.size, Vector2.zero );
        return u.sqrMagnitude < radius * radius;
    }

    public static Vector3 Vector2ToVector3 ( Vector2 source ) {
        return new Vector3 ( source.x, source.y, 0 );
    }

    public static int RandomArrayIndex< T > ( T[] array ) {
        if ( array == null || array.Length == 0 )
            return - 1;

        return UnityEngine.Random.Range ( 0, array.Length );
    }

    public static int RandomArrayIndex ( int startIndex, int maxIndex ) {
        return UnityEngine.Random.Range ( startIndex, maxIndex + 1 );
    }

    public static T RandomArrayElement< T > ( T[] array ) {
        if ( array == null || array.Length == 0 )
            return default ( T );

        return array[ RandomArrayIndex ( array ) ];
    }

    public static bool ArrayContainElement < T >( T[] array, T element ) {

        if ( array != null ) {
            foreach ( var elm in array ) {
                if ( elm.Equals ( element ) ) {
                    return true;
                }
            }
        }
        
        return false;
    }

    public static T RandomListElement< T > ( System.Collections.Generic.List< T > list ) {
        if ( list == null || list.Count == 0 )
            return default ( T );

        return list[ RandomArrayIndex ( 0, list.Count - 1 ) ];
    }

    public static T RandomListElementAndRemove< T > ( System.Collections.Generic.List< T > list ) {
        if ( list == null || list.Count == 0 )
            return default ( T );

        T element = list [ RandomArrayIndex ( 0, list.Count - 1 ) ];
        list.Remove ( element );

        return element;
    }

    /// <summary>
    /// 对数组洗牌
    /// </summary>
    /// <param name="list"></param>
    /// <typeparam name="T"></typeparam>
    public static void ListShuffle< T > ( System.Collections.Generic.IList< T > list ) {
        System.Random rnd = new System.Random ();
        for ( var i = 0; i < list.Count - 1; i++ ) {
            ListSwap ( list, i, rnd.Next ( i, list.Count ) );
        }
    }

    public static void ListSwap< T > ( System.Collections.Generic.IList< T > list, int i, int j ) {
        var temp = list[ i ];
        list[ i ] = list[ j ];
        list[ j ] = temp;
    }

    public static float Ease ( float x, float a ) {
        return Mathf.Pow ( x, a ) / ( Mathf.Pow ( x, a ) + Mathf.Pow ( 1f - x, a ) );
    }

    /// <summary>
    /// 旋转一个向量
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="aDegree"></param>
    /// <returns></returns>
    public static Vector2 Rotate ( Vector2 vector, float aDegree ) {
        float rad = aDegree * Mathf.Deg2Rad;
        float s   = Mathf.Sin ( rad );
        float c   = Mathf.Cos ( rad );
        return new Vector2 (
            vector.x * c - vector.y * s,
            vector.y * c + vector.x * s
        );
    }

    /// <summary>
	/// 在相机可视范围内?
	/// </summary>
	/// <param name="camera"></param>
	/// <param name="position"></param>
	/// <returns></returns>
	public static bool IsInCameraView ( Camera camera, Vector3 position ) {

		Transform camTransform = camera.transform;
		Vector2 viewPos = camera.WorldToViewportPoint ( position );
		Vector3 dir = ( position - camTransform.position ).normalized;
		float dot = Vector3.Dot ( camTransform.forward, dir );     //判断物体是否在相机前面  

		if ( dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 )
			return true;
		else
			return false;
	}

    /// <summary>
    /// 获取A相对于B的方向
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    public static int GetDirectionRelative ( Vector3 A, Vector3 B ) {
        if ( A.x > B.x )
            return 1;
        if ( A.x < B.x )
            return -1;
        return 0;
    }

    /// <summary>
    /// 在给定范围内查找指定方向的最近物体
    /// </summary>
    /// <param name="transList">列表</param>
    /// <param name="current">当前物体</param>
    /// <param name="dir">查找方向</param>
    public static Transform FindClosestTrans ( Transform[] transList, Transform current, Vector2 dir ) {
        var alternativeTrans = new System.Collections.Generic.List<Transform>();

        dir = dir.normalized;

        // 筛选指定方向区域的按钮
        foreach (var trans in transList)
        {
            if (trans == current)
                continue;

            if ( dir == Vector2.up ) {
                if (trans.position.y >= current.position.y)
                    alternativeTrans.Add(trans);
            }
            else if ( dir == Vector2.down ) {
                if (trans.position.y <= current.position.y)
                    alternativeTrans.Add(trans);
            }
            else if ( dir == Vector2.left ) {
                if (trans.position.x <= current.position.x)
                    alternativeTrans.Add(trans);
            }
            else if ( dir == Vector2.right ) {
                if (trans.position.x >= current.position.x)
                    alternativeTrans.Add(trans);
            }
        }

        if (alternativeTrans.Count == 0)
            return null;

        if (alternativeTrans.Count == 1)
        {
            return alternativeTrans[0];
        }

        // 查找筛选后点距最小的按钮
        float  minDistance   = float.MaxValue;
        Transform closestTrans = null;

        foreach (var trans in alternativeTrans)
        {
            var dis = Vector2.Distance(trans.position, current.position);
            if (dis < minDistance)
            {
                minDistance   = dis;
                closestTrans = trans;
            }
        }

        return closestTrans;
    }
}
