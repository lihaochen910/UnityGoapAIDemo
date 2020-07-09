using UnityEngine;

/// <summary>
/// 跟随相机
/// </summary>
public class FollowCamera : AbstractPausableComponent {
    
    private const float MIN_DISTANCE = 0.005f;    // 最小距离，小于这个值可以忽略不计了
    
    public FollowMode followMode = FollowMode.Transform;
    public bool inertial = false;
    public float smoothTime_X = 0.8f;
    public float smoothTime_Y = 0.5f;
    public Vector2 Offset = Vector2.zero;

	private float smoothXspeed = 0f;
	private float smoothYspeed = 0f;
	private float zOffset;
    private Camera camera;

    [SerializeField] private Transform _target;

    [SerializeField] private Vector3 _point;

    // [SerializeField] private float _LR_offset;
    // [SerializeField] private float _LR_speed_rate = 0.5f;
    // private Player.CharacterController2D _LR_target;

    /// <summary>
    /// 相机范围限定
    /// </summary>
    public bool useEdgeLimit;
	private float ex = 2f;
	public Vector3 P1, P2, P3, P4;
	public float minX, minY, maxX, maxY;

	public float Camera2D_Height
	{
		get { return camera.orthographicSize * 2.0f; }
	}

	public float Camera2D_Width
	{
		get { return Camera2D_Height * camera.aspect; }
	}

    public Vector2 TopLeft => new Vector2(transform.position.x - Camera2D_Width / 2, transform.position.y + Camera2D_Height / 2);
    public Vector2 BottomLeft => new Vector2(transform.position.x - Camera2D_Width / 2, transform.position.y - Camera2D_Height / 2);
    public Vector2 TopRight => new Vector2(transform.position.x + Camera2D_Width / 2, transform.position.y + Camera2D_Height / 2);
    public Vector2 BottomRight => new Vector2(transform.position.x + Camera2D_Width / 2, transform.position.y - Camera2D_Height / 2);

    public bool IsStable {
        get {
            switch ( followMode ) {
                
                case FollowMode.Point:
                    return FollowTargetDistance < MIN_DISTANCE;
                case FollowMode.Transform:
                case FollowMode.SaltAndSanctuary:

                    if ( _target == null ) return true;
                    
                    return FollowTargetDistance < MIN_DISTANCE;
                default: return true;
            }
        }
    }

    public Vector2 FollowTargetPosition {
        get {
            switch ( followMode ) {
                
                case FollowMode.Point:
                    return (Vector2)_point + new Vector2 ( Offset.x, Offset.y );
                case FollowMode.Transform:
                case FollowMode.SaltAndSanctuary:

                    if ( _target == null ) return transform.position;
                    
                    return (Vector2)_target.position + new Vector2 ( Offset.x, Offset.y );

                default: return transform.position;
            }
        }
    }

    public float FollowTargetDistance {
        get { return Vector2.Distance ( transform.position, FollowTargetPosition ); }
    }

    protected override void Awake () {
        timeLayer = CupheadTime.Layer.Default;

        zOffset = transform.position.z;
        camera = GetComponent< Camera > ();
    }

    public void SetFollowMode ( FollowMode mode ) {
        followMode = mode;
    }

    public void SetTarget(Transform target)
    {
        if (target != null)
            _target = target;
    }

    // public void SetTarget(Player.CharacterController2D controller2D)
    // {
    //     this._LR_target = controller2D;
    //     this._target = controller2D.transform;
    // }

    public void SetFollowPoint ( Vector3 point ) {
        this._point = point;
    }

    public void SetZOffset ( float zOffset ) {
        this.zOffset = zOffset;
    }

    protected override void LateUpdate()
    {
        Vector3 newPos = default;

        switch (followMode)
        {
            case FollowMode.Transform:
                if (_target == null)
                    return;
                
                if (inertial)
                    newPos = InertialFollow(_target.transform.position.x, _target.transform.position.y);
                else
                    newPos = SmoothFollow(_target.transform.position.x + Offset.x, _target.transform.position.y + Offset.y);

                if (useEdgeLimit)
                {
                    if (newPos.x >= maxX)
                        newPos.x = maxX;
                    else if (newPos.x <= minX)
                        newPos.x = minX;
                    if (newPos.y >= maxY)
                        newPos.y = maxY;
                    else if (newPos.y <= minY)
                        newPos.y = minY;
                }
                transform.position = newPos;
                break;

            case FollowMode.Point:
                if (inertial)
                    newPos = InertialFollow(_point.x, _point.y);
                else
                    newPos = SmoothFollow(_point.x + Offset.x, _point.y + Offset.y);
                
                transform.position = newPos;
                ClampPosition ();
                break;

            case FollowMode.SaltAndSanctuary:

                // float realOffset = _LR_target.FaceDirection == 1 ? Mathf.Abs(_LR_offset) : -Mathf.Abs(_LR_offset);
                //
                // float t_X, t_Y;
                //
                // t_X = smoothTime_X;
                // t_Y = smoothTime_Y;
                //
                // if (inertial)
                // {
                //     newPos = InertialFollow(_LR_target.transform.position.x + realOffset, _LR_target.transform.position.y,
                //         t_X, t_Y);
                // }
                // else
                // {
                //     newPos = SmoothFollow(_LR_target.transform.position.x + realOffset, _LR_target.transform.position.y,
                //         t_X, t_Y);
                // }
                //
                // newPos.x += _LR_target.GetCurrentVelocity().x * _LR_speed_rate * LocalDeltaTime;
                //
                // transform.position = newPos;
                // ClampPosition ();

                break;
            
//            case FollowMode.Static:
//                break;

            default: return;
        }
    }

	public void Center_to_the_player(bool ignoreEdge = false)
	{
		var playerPos = _target.transform.position;
        var targetPos = new Vector3(playerPos.x + Offset.x, playerPos.y + Offset.y, transform.position.z);

        // 检查目标坐标是否超出边界
        if (!ignoreEdge && useEdgeLimit)
        {
            if (targetPos.x > maxX)
                targetPos.x = maxX;
            else if (targetPos.x < minX)
                targetPos.x = minX;
            if (targetPos.y > maxY)
                targetPos.y = maxY;
            else if (targetPos.y < minY)
                targetPos.y = minY;
        }

        transform.position = targetPos;
    }

    public void ClampPosition () {
        
        // 检查目标坐标是否超出边界
        if ( useEdgeLimit ) {
            if ( transform.position.x > maxX )
                transform.SetPosition ( maxX );
            else if ( transform.position.x < minX )
                transform.SetPosition ( minX );
            if ( transform.position.y > maxY )
                transform.SetPosition ( null, maxY );
            else if ( transform.position.y < minY )
                transform.SetPosition ( null, minY );
        }
    }

    public void ResetPoint()
	{
		P1 = new Vector3(transform.position.x - ex, transform.position.y + ex);
		P2 = new Vector3(transform.position.x + ex, transform.position.y + ex);
		P3 = new Vector3(transform.position.x + ex, transform.position.y - ex);
		P4 = new Vector3(transform.position.x - ex, transform.position.y - ex);
	}

	private Vector3 InertialFollow(float targetX, float targetY)
    {
        return InertialFollow(targetX, targetY, this.smoothTime_X, this.smoothTime_Y);
    }

    private Vector3 InertialFollow(float targetX, float targetY, float smoothTime_X, float smoothTime_Y)
    {
        float x = InertialDamp(transform.position.x, targetX, smoothTime_X);
        float y = InertialDamp(transform.position.y, targetY, smoothTime_Y);
        return new Vector3(x, y, zOffset);
    }

    private Vector3 SmoothFollow(float targetX, float targetY)
    {
        return SmoothFollow(targetX, targetY, this.smoothTime_X, this.smoothTime_Y);
    }

    private Vector3 SmoothFollow(float targetX, float targetY, float smoothTime_X, float smoothTime_Y)
    {
        float x = SmoothDamp(transform.position.x, targetX, ref smoothXspeed, smoothTime_X);
        float y = SmoothDamp(transform.position.y, targetY, ref smoothYspeed, smoothTime_Y);
        return new Vector3(x, y, zOffset);
    }

    private float InertialDamp(float previousValue, float targetValue, float smoothTime)
    {
        float x = previousValue - targetValue;
        float newValue = x + LocalDeltaTime * (-1f / smoothTime * x);
        return targetValue + newValue;
    }

    private float SmoothDamp(float previousValue, float targetValue, ref float speed, float smoothTime)
    {
        float T1 = 0.36f * smoothTime;
        float T2 = 0.64f * smoothTime;
        float x = previousValue - targetValue;
        float newSpeed = speed + LocalDeltaTime * (-1f / (T1 * T2) * x - (T1 + T2) / (T1 * T2) * speed);
        float newValue = x + LocalDeltaTime * speed;
        speed = newSpeed;
        return targetValue + newValue;
    }

    public enum FollowMode
    {
        Transform,
        Point,
        /// <summary>
        /// 盐与避难所的相机模式
        /// </summary>
        SaltAndSanctuary,
//        Static
    }
}