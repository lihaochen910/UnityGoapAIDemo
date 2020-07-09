#define DEBUG_CC2D_RAYS
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prime31 {

    [RequireComponent( typeof( BoxCollider2D )/*, typeof( Rigidbody2D )*/ )]
    public class CharacterController2D : MonoBehaviour
    {
	    #region internal types

	    public struct CharacterRaycastOrigins
	    {
		    public Vector3 topLeft;
		    public Vector3 bottomRight;
		    public Vector3 bottomLeft;
	    }

	    public class CharacterCollisionState2D
	    {
		    public bool above, below, left, right;
            public Vector2 abovePoint, belowPoint, leftPoint, rightPoint;
            public Vector2 aboveNormal, belowNormal, leftNormal, rightNormal;
            public Collider2D aboveObject, belowObject, leftObject, rightObject;
            public bool becameGroundedThisFrame;
		    public bool wasGroundedLastFrame;
		    public bool movingDownSlope;
		    public float slopeAngle;

	        public int faceDir;

		    public bool hasCollision()
		    {
			    return below || right || left || above;
		    }


		    public void reset()
		    {
			    right = left = above = below = becameGroundedThisFrame = movingDownSlope = false;
                aboveObject = belowObject = leftObject = rightObject = null;
                abovePoint = belowPoint = leftPoint = rightPoint = Vector2.zero;
                aboveNormal = belowNormal = leftNormal = rightNormal = Vector2.zero;
                slopeAngle = 0f;
		    }


		    public void CopyTo( CharacterCollisionState2D other ) {
			    other.right = right;
			    other.left = left;
			    other.above = above;
			    other.below = below;
			    other.becameGroundedThisFrame = becameGroundedThisFrame;
			    other.movingDownSlope = movingDownSlope;
			    other.aboveObject = aboveObject;
			    other.belowObject = belowObject;
			    other.leftObject = leftObject;
			    other.rightObject = rightObject;
			    other.abovePoint = abovePoint;
			    other.belowPoint = belowPoint;
			    other.leftPoint = leftPoint;
			    other.rightPoint = rightPoint;
			    other.aboveNormal = aboveNormal;
			    other.belowNormal = belowNormal;
			    other.leftNormal = leftNormal;
			    other.rightNormal = rightNormal;
			    other.slopeAngle = slopeAngle;
		    }

		    public override string ToString()
		    {
			    return string.Format( "[CharacterCollisionState2D] r: {0}, l: {1}, a: {2}, b: {3}, movingDownSlope: {4}, angle: {5}, wasGroundedLastFrame: {6}, becameGroundedThisFrame: {7}",
			                         right, left, above, below, movingDownSlope, slopeAngle, wasGroundedLastFrame, becameGroundedThisFrame );
		    }
	    }

	    #endregion


	    #region events, properties and fields

	    public event Action<RaycastHit2D> onControllerCollidedEvent;
	    public event Action<Collider2D> onTriggerEnterEvent;
	    public event Action<Collider2D> onTriggerStayEvent;
	    public event Action<Collider2D> onTriggerExitEvent;


	    /// <summary>
	    /// when true, one way platforms will be ignored when moving vertically for a single frame
	    /// 如果打开，单帧的一个方向的平台将会被忽略
	    /// </summary>
	    [HideInInspector]
	    public bool ignoreOneWayPlatformsThisFrame;

	    [SerializeField]
	    [Range( 0.001f, 0.3f )]
	    float _skinWidth = 0.02f;

	    /// <summary>
	    /// defines how far in from the edges of the collider rays are cast from. If cast with a 0 extent it will often result in ray hits that are
	    /// not desired (for example a foot collider casting horizontally from directly on the surface can result in a hit)
	    /// 定义了从碰撞盒射线的边缘投射的距离。 如果使用值0进行投射，则通常会导致不希望的射线命中（例如，从表面上直接水平投影的脚对撞机可能会导致命中）
	    /// </summary>
	    public float skinWidth
	    {
		    get { return _skinWidth; }
		    set
		    {
			    _skinWidth = value;
			    recalculateDistanceBetweenRays();
		    }
	    }


	    /// <summary>
	    /// mask with all layers that the player should interact with
	    /// 标记与玩家可以产生互动的所有层
	    /// </summary>
	    public LayerMask platformMask = 0;

        /// <summary>
        /// 保存原层
        /// </summary>
        protected LayerMask _platformMaskSave;

        /// <summary>
        /// mask with all layers that trigger events should fire when intersected
        /// 标记可以产生事件互动的层级
        /// </summary>
        //public LayerMask triggerMask = 0;

	    /// <summary>
	    /// mask with all layers that should act as one-way platforms. Note that one-way platforms should always be EdgeCollider2Ds. This is because it does not support being
	    /// updated anytime outside of the inspector for now.
	    /// </summary>
	    [SerializeField]
        public LayerMask oneWayPlatformMask = 0;

        public LayerMask blockMask = 0;

        /// <summary>
        /// the max slope angle that the CC2D can climb
        /// 人物可以攀爬的最大倾角
        /// </summary>
        /// <value>The slope limit.</value>
        [Range( 0f, 90f )]
	    public float slopeLimit = 30f;

	    /// <summary>
	    /// the threshold in the change in vertical movement between frames that constitutes jumping
	    /// 构成跳跃的帧之间的垂直运动变化的阈值
	    /// </summary>
	    /// <value>The jumping threshold.</value>
	    public float jumpingThreshold = 0.07f;


	    /// <summary>
	    /// curve for multiplying speed based on slope (negative = down slope and positive = up slope)
	    /// </summary>
	    public AnimationCurve slopeSpeedMultiplier = new AnimationCurve( new Keyframe( -90f, 1.5f ), new Keyframe( 0f, 1f ), new Keyframe( 90f, 0f ) );

	    [Range( 2, 20 )]
	    public int totalHorizontalRays = 8;
	    [Range( 2, 20 )]
	    public int totalVerticalRays = 4;

        /// <summary>
        /// this is used to calculate the downward ray that is cast to check for slopes. We use the somewhat arbitrary value 75 degrees
        /// to calculate the length of the ray that checks for slopes.
        /// </summary>
        float _slopeLimitTangent = Mathf.Tan( 75f * Mathf.Deg2Rad );


	    [HideInInspector][NonSerialized]
	    public new Transform transform;
	    [HideInInspector][NonSerialized]
	    public BoxCollider2D boxCollider;
	    [HideInInspector][NonSerialized]
	    public Rigidbody2D rigidBody2D;

	    [HideInInspector][NonSerialized]
	    public CharacterCollisionState2D collisionState = new CharacterCollisionState2D();
	    [HideInInspector][NonSerialized]
	    public CharacterCollisionState2D previousCollisionState = new CharacterCollisionState2D();
	    [HideInInspector][NonSerialized]
	    public Vector3 velocity;
	    public bool isGrounded { get { return collisionState.below; } }

	    const float kSkinWidthFloatFudgeFactor = 0.001f;

        #endregion


        /// <summary>
        /// holder for our raycast origin corners (TR, TL, BR, BL)
        /// 射线的原点
        /// </summary>
        public CharacterRaycastOrigins raycastOrigins => _raycastOrigins;
        private CharacterRaycastOrigins _raycastOrigins;

        /// <summary>
        /// stores our raycast hit during movement
        /// 在移动时存放的检测结果
        /// </summary>
        RaycastHit2D _raycastHit;

        /// <summary>
        /// cache
        /// </summary>
        LevelPlatform _movingPlatform;

        /// <summary>
        /// stores any raycast hits that occur this frame. we have to store them in case we get a hit moving
        /// horizontally and vertically so that we can send the events after all collision state is set
        /// 存储发生在此帧的任何raycast hit。 我们必须存储它们，以防我们得到水平和垂直的命中，以便我们可以在所有碰撞状态设置后发送事件
        /// </summary>
        public List<RaycastHit2D> _raycastHitsXThisFrame = new List<RaycastHit2D>( 2 );
        public List<RaycastHit2D> _raycastHitsYThisFrame = new List<RaycastHit2D>();

        // horizontal/vertical movement data
        //水平/竖直射线的间距
        float _verticalDistanceBetweenRays;
	    float _horizontalDistanceBetweenRays;

        public float verticalDistanceBetweenRays => _verticalDistanceBetweenRays;

        // we use this flag to mark the case where we are travelling up a slope and we modified our delta.y to allow the climb to occur.
        // the reason is so that if we reach the end of the slope we can make an adjustment to stay grounded
        //我们使用这个标志来标记我们正在爬坡的情况，我们修改了我们的delta.y以允许攀登发生。
        //原因是如果我们到达边坡的尽头，我们可以进行调整以保持接地
        bool _isGoingUpSlope = false;


	    #region Monobehaviour

	    void Awake()
	    {
	        _platformMaskSave = platformMask;

            // add our one-way platforms to our normal platform mask so that we can land on them from above
            platformMask |= oneWayPlatformMask;

		    // cache some components
		    transform = GetComponent<Transform>();
		    boxCollider = GetComponent<BoxCollider2D>();
		    rigidBody2D = GetComponent<Rigidbody2D>();

		    // here, we trigger our properties that have setters with bodies
		    skinWidth = _skinWidth;

	        collisionState.faceDir = 1;

            // we want to set our CC2D to ignore all collision layers except what is in our triggerMask
	        // 我们希望将CC2D设置为忽略除了我们的triggerMask中的所有碰撞层
            //for ( var i = 0; i < 32; i++ )
		    //{
		    //	// see if our triggerMask contains this layer and if not ignore it
		    //	if( ( triggerMask.value & 1 << i ) == 0 )
		    //		Physics2D.IgnoreLayerCollision( gameObject.layer, i );
		    //}
	    }


	    public void OnTriggerEnter2D( Collider2D col )
	    {
		    if( onTriggerEnterEvent != null )
			    onTriggerEnterEvent( col );
	    }


	    public void OnTriggerStay2D( Collider2D col )
	    {
		    if( onTriggerStayEvent != null )
			    onTriggerStayEvent( col );
	    }


	    public void OnTriggerExit2D( Collider2D col )
	    {
		    if( onTriggerExitEvent != null )
			    onTriggerExitEvent( col );
	    }

	    #endregion


	    [System.Diagnostics.Conditional( "DEBUG_CC2D_RAYS" )]
	    void DrawRay( Vector3 start, Vector3 dir, Color color )
	    {
		    Debug.DrawRay( start, dir, color );
	    }


	    #region Public

	    /// <summary>
	    /// attempts to move the character to position + deltaMovement. Any colliders in the way will cause the movement to
	    /// stop when run into.
	    /// 尝试将角色移动到position + deltaMovement。 任何碰撞方式都会导致运行停止。
	    /// </summary>
	    /// <param name="deltaMovement">Delta movement.</param>
	    public void Move( Vector2 deltaMovement , bool standingOnPlatform = false)
	    {
            if (!this.enabled)
                return;
            
            collisionState.CopyTo ( previousCollisionState );

		    // save off our current grounded state which we will use for wasGroundedLastFrame and becameGroundedThisFrame
            // 节省我们目前的接地状态，我们将用于wasGroundedLastFrame并成为GroundedThisFrame
		    collisionState.wasGroundedLastFrame = collisionState.below;

		    // clear our state
            // 清理上一帧的状态
		    collisionState.reset();
	        _raycastHitsXThisFrame.Clear();
	        _raycastHitsYThisFrame.Clear();
            _isGoingUpSlope = false;

            //if (deltaMovement.x != 0f) {
            //    collisionState.faceDir = (int)Mathf.Sign(deltaMovement.x);
            //}

            primeRaycastOrigins();

            // first, we check for a slope below us before moving
            // only check slopes if we are going down and grounded
            // 首先，我们在移动之前检查我们下方的一个坡度
            // 如果我们正在下降并接地，只能检查斜坡
            if ( deltaMovement.y < 0f && collisionState.wasGroundedLastFrame )
			    handleVerticalSlope( ref deltaMovement );

            // now we check movement in the horizontal dir
            // 检查水平方向的位移
            if ( deltaMovement.x != 0f )
                moveHorizontally(ref deltaMovement);
            //moveHorizontally(ref deltaMovement);

            // next, check movement in the vertical dir
            // 然后检查竖直方向的位移
            if ( deltaMovement.y != 0f )
		        moveVertically( ref deltaMovement );
            //moveVertically( ref deltaMovement );

            //if (deltaMovement.x != 0f)
            //    handleBlockLayer(ref deltaMovement);

            // move then update our state
            // 移动然后更新我们的状态
            //deltaMovement.z = 0;
            transform.Translate( deltaMovement, Space.World );

		    // only calculate velocity if we have a non-zero deltaTime
            // 如果我们有一个非零deltaTime，只计算速度
		    if( Time.deltaTime > 0f )
			    velocity = deltaMovement / Time.deltaTime;

            if (standingOnPlatform)
                collisionState.below = true;

            // set our becameGrounded state based on the previous and current collision state
            // 根据前一帧的和当前的碰撞状态设置我们的接地状态
            if ( !collisionState.wasGroundedLastFrame && collisionState.below )
			    collisionState.becameGroundedThisFrame = true;

		    // if we are going up a slope we artificially set a y velocity so we need to zero it out here
            // 如果我们正在上坡，我们人为地设置一个y轴速度，所以我们需要在这里清零
		    if( _isGoingUpSlope )
			    velocity.y = 0;

		    // send off the collision events if we have a listener
            // 推送碰撞事件
		    if( onControllerCollidedEvent != null )
		    {
			    for( var i = 0; i < _raycastHitsXThisFrame.Count; i++ )
				    onControllerCollidedEvent( _raycastHitsXThisFrame[i] );
		    }

		    ignoreOneWayPlatformsThisFrame = false;

            // 检查MovePlatform Attach
            if (collisionState.becameGroundedThisFrame && collisionState.belowObject != null)
            {
                LevelPlatform movePlatform = collisionState.belowObject.GetComponent<LevelPlatform>();

                if (movePlatform != null)
                {
                    _movingPlatform = movePlatform;
                    _movingPlatform.AddChild(transform);
                    //Debug.Log($"Attach {collisionState.belowObject}");
                }
            }

            // 检查MovePlatform Detach
            if (collisionState.wasGroundedLastFrame && !collisionState.below)
            {
                if (_movingPlatform)
                {
                    //Debug.Log($"Detach {_movingPlatform} !!!!!!!!!!!!!!!!!!!");
                    _movingPlatform.OnPlayerExit(transform);
                    transform.SetParent(null, true);
                    _movingPlatform = null;
                }
            }
            //else if(collisionState.wasGroundedLastFrame && collisionState.below)
            //{
            //    if (_movingPlatform && !_movingPlatform.enabled)
            //    {
            //        //Debug.Log($"Detach {_movingPlatform} !!!!!!!!!!!!!!!!!!!");
            //        _movingPlatform.OnPlayerExit(transform);
            //        transform.SetParent(null, true);
            //        _movingPlatform = null;
            //    }
            //}

        }


        /// <summary>
        /// moves directly down until grounded
        /// 直直的下落直到接触地面
        /// </summary>
        public void warpToGrounded()
	    {
		    do
		    {
		        Move( new Vector3( 0, -1f, 0 ), true);
		    } while( !isGrounded );
	    }


	    /// <summary>
	    /// this should be called anytime you have to modify the BoxCollider2D at runtime. It will recalculate the distance between the rays used for collision detection.
	    /// It is also used in the skinWidth setter in case it is changed at runtime.
	    /// 只要您在运行时修改BoxCollider2D，就应该调用这个，它将重新计算用于碰撞检测的射线之间的距离。
	    /// 它也用于skinWidth setter，以防在运行时更改。
	    /// </summary>
	    public void recalculateDistanceBetweenRays()
	    {
		    // figure out the distance between our rays in both directions
            // 找出我们两个方向的射线之间的距离
		    // horizontal
		    var colliderUseableHeight = boxCollider.size.y * Mathf.Abs( transform.localScale.y ) - ( 2f * _skinWidth );
		    _verticalDistanceBetweenRays = colliderUseableHeight / ( totalHorizontalRays - 1 );

		    // vertical
		    var colliderUseableWidth = boxCollider.size.x * Mathf.Abs( transform.localScale.x ) - ( 2f * _skinWidth );
		    _horizontalDistanceBetweenRays = colliderUseableWidth / ( totalVerticalRays - 1 );
	    }

        /// <summary>
        /// Resets the collision mask with the default settings
        /// </summary>
        public virtual void CollisionsOn()
        {
            platformMask = _platformMaskSave;
            platformMask |= oneWayPlatformMask;
        }

        /// <summary>
        /// Turns all collisions off
        /// </summary>
        public virtual void CollisionsOff()
        {
            platformMask = 0;
        }

        /// <summary>
        /// Disables collisions only with the one way platform layers
        /// </summary>
        public virtual void CollisionsOffWithOneWayPlatforms()
        {
            platformMask -= oneWayPlatformMask;
        }

        /// <summary>
        /// 简单的垂直方向射线检测
        /// </summary>
        /// <param name="directionY">1/-1</param>
        /// <returns>检测结果列表</returns>
        public List<RaycastHit2D> SimpleRaycastY(float directionY, LayerMask mask)
        {
            List<RaycastHit2D> hitList = new List<RaycastHit2D>();

            //float rayLength = 2 * skinWidth;
            float rayLength = boxCollider.bounds.extents.y / 1.5f;

            for (int i = 0; i < totalVerticalRays; i++)
            {

                Vector2 rayOrigin = (directionY == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
                rayOrigin += Vector2.right * _horizontalDistanceBetweenRays * i;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, mask);

                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.cyan);

                if (hit)
                {
                    hitList.Add(hit);
                }
            }

            return hitList;
        }
        /// <summary>
        /// 简单的水平方向射线检测
        /// </summary>
        /// <param name="directionX">1/-1</param>
        /// <returns>检测结果列表</returns>
        public List<RaycastHit2D> SimpleRaycastX(float directionX, LayerMask mask)
        {
            List<RaycastHit2D> hitList = new List<RaycastHit2D>();

            //float rayLength = 2 * skinWidth;
            float rayLength = boxCollider.bounds.extents.x;

            for (int i = 0; i < totalHorizontalRays; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (_verticalDistanceBetweenRays * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, mask);

                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.cyan);

                if (hit)
                {
                    hitList.Add(hit);
                }
            }

            return hitList;
        }

        public void ManualHorizontalCollisionDetection(int directionX)
        {
            var rayDistance = _skinWidth * 2;
            var rayDirection = directionX == 1 ? Vector2.right : -Vector2.right;
            var initialRayOrigin = directionX == 1 ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;

            for (var i = 0; i < totalHorizontalRays; i++)
            {
                var ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y + i * _verticalDistanceBetweenRays);

                DrawRay(ray, rayDirection * rayDistance, Color.red);

                // if we are grounded we will include oneWayPlatforms only on the first ray (the bottom one). this will allow us to
                // walk up sloped oneWayPlatforms
                if (i == 0 && collisionState.wasGroundedLastFrame)
                    _raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, platformMask | blockMask);
                else
                    _raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, platformMask & ~oneWayPlatformMask | blockMask);

                if (_raycastHit)
                {
                    // 尝试添加距离为0时的逻辑
                    //if (_raycastHit.distance == 0)
                    //{
                    //    if (_raycastHit.collider.gameObject.layer != blockMask.value)
                    //        deltaMovement.x = 0;
                    //    continue;
                    //}

                    // remember to remove the skinWidth from our deltaMovement
                    if (directionX == 1)
                    {
                        collisionState.right = true;
                        collisionState.rightPoint = _raycastHit.point;
                        collisionState.rightObject = _raycastHit.collider;
                    }
                    else
                    {
                        collisionState.left = true;
                        collisionState.leftPoint = _raycastHit.point;
                        collisionState.leftObject = _raycastHit.collider;
                    }

                    _raycastHitsXThisFrame.Add(_raycastHit);

                    // we add a small fudge factor for the float operations here. if our rayDistance is smaller
                    // than the width + fudge bail out because we have a direct impact
                    if (rayDistance < _skinWidth + kSkinWidthFloatFudgeFactor)
                        break;
                }
            }
        }

        void handleBlockLayer( ref Vector2 deltaMovement )
        {
            var rayDistance = Mathf.Abs(deltaMovement.x == 0 ? _skinWidth : deltaMovement.x) + _skinWidth;
            var rayDirection = collisionState.faceDir == 1 ? Vector2.right : -Vector2.right;
            var initialRayOrigin = collisionState.faceDir == 1 ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;

            for (var i = 0; i < totalHorizontalRays; i++)
            {
                var ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y + i * _verticalDistanceBetweenRays);

                // if we are grounded we will include oneWayPlatforms only on the first ray (the bottom one). this will allow us to
                // walk up sloped oneWayPlatforms
                _raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, blockMask);

                if (_raycastHit)
                {
                    // 尝试添加距离为0时的逻辑
                    //if (_raycastHit.distance == 0)
                    //{
                    //    if (_raycastHit.collider.gameObject.layer != blockMask.value)
                    //        deltaMovement.x = 0;
                    //    continue;
                    //}

                    // set our new deltaMovement and recalculate the rayDistance taking it into account
                    deltaMovement.x = 0;
                    rayDistance = Mathf.Abs(_raycastHit.point.x - ray.x);

                    // we add a small fudge factor for the float operations here. if our rayDistance is smaller
                    // than the width + fudge bail out because we have a direct impact
                    if (rayDistance < _skinWidth + kSkinWidthFloatFudgeFactor)
                        break;
                }
            }
        }

        void handleBlockOverlapping( ref Vector2 deltaMovement )
        {
            Debug.Log("重叠了，尝试推出去");

            int pushDir = 0;

            if (_raycastHit.transform.position.x >= transform.position.x)
            {
                pushDir = -1;
            }
            else
            {
                pushDir = 1;
            }

            // 位移自身直到不再相交
            if (Mathf.Abs((boxCollider.bounds.center.x + boxCollider.bounds.extents.x) - (_raycastHit.collider.bounds.center.x - _raycastHit.collider.bounds.extents.x)) > 0.3f)
            {
                deltaMovement.x += pushDir * 3f * Time.deltaTime;
            }
            else
            {
                var targetPos = transform.position;

                if (pushDir == -1)
                {
                    targetPos.x = (_raycastHit.collider.bounds.center.x - _raycastHit.collider.bounds.extents.x) - boxCollider.size.x * 0.5f;
                    deltaMovement.x = -Mathf.Abs(targetPos.x - transform.position.x);
                }
                else
                {
                    targetPos.x = (_raycastHit.collider.bounds.center.x + _raycastHit.collider.bounds.extents.x) + boxCollider.size.x * 0.5f;
                    deltaMovement.x = Mathf.Abs(targetPos.x - transform.position.x);
                }
            }
        }

        #endregion


        #region Movement Methods

        /// <summary>
        /// resets the raycastOrigins to the current extents of the box collider inset by the skinWidth. It is inset
        /// to avoid casting a ray from a position directly touching another collider which results in wonky normal data.
        /// 更新射线原点
        /// </summary>
        void primeRaycastOrigins()
        {
	        // our raycasts need to be fired from the bounds inset by the skinWidth
	        var modifiedBounds = boxCollider.bounds;
            modifiedBounds.Expand( -2f * _skinWidth );

            _raycastOrigins.topLeft = new Vector2( modifiedBounds.min.x, modifiedBounds.max.y );
	        _raycastOrigins.bottomRight = new Vector2( modifiedBounds.max.x, modifiedBounds.min.y );
	        _raycastOrigins.bottomLeft = modifiedBounds.min;
        }


	    /// <summary>
	    /// we have to use a bit of trickery in this one. The rays must be cast from a small distance inside of our
	    /// collider (skinWidth) to avoid zero distance rays which will get the wrong normal. Because of this small offset
	    /// we have to increase the ray distance skinWidth then remember to remove skinWidth from deltaMovement before
	    /// actually moving the player
	    /// 我们必须在这一点上使用一些诡计。 
	    /// 射线必须从我们的碰撞盒（skinWidth）内的一小段距离投射，以避免零距离射线，这将导致错误。
	    /// 因为这个小的偏移，我们必须增加射线距离skinWidth，然后记住在实际移动玩家之前从deltaMovement量中减去skinWidth
	    /// </summary>
	    void moveHorizontally( ref Vector2 deltaMovement )
	    {
		    var isGoingRight = deltaMovement.x > 0;
		    var rayDistance = Mathf.Abs( deltaMovement.x == 0 ? _skinWidth : deltaMovement.x) + _skinWidth;
		    var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
            //var rayDirection = collisionState.faceDir * Vector2.right;
		    var initialRayOrigin = isGoingRight ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;
            //var initialRayOrigin = collisionState.faceDir == 1 ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;

		    for( var i = 0; i < totalHorizontalRays; i++ )
		    {
			    var ray = new Vector2( initialRayOrigin.x, initialRayOrigin.y + i * _verticalDistanceBetweenRays );
#if UNITY_EDITOR
                DrawRay( ray, rayDirection * rayDistance, Color.red );
#endif
                // if we are grounded we will include oneWayPlatforms only on the first ray (the bottom one). this will allow us to
                // walk up sloped oneWayPlatforms
                if( i == 0 && collisionState.wasGroundedLastFrame )
				    _raycastHit = Physics2D.Raycast( ray, rayDirection, rayDistance, platformMask | blockMask);
			    else
				    _raycastHit = Physics2D.Raycast( ray, rayDirection, rayDistance, platformMask & ~oneWayPlatformMask | blockMask);

			    if( _raycastHit )
			    {
                    // 尝试添加距离为0时的逻辑
                    if (_raycastHit.distance == 0)
                    {
                        if (_raycastHit.collider.gameObject.layer != blockMask.value)
                        {
                            deltaMovement.x = 0;
                            break;
                        }

                        //  如果重叠了，尝试推出去
//                        if (_raycastHit.collider.gameObject.layer != blockMask.value
//                            && _raycastHit.collider.bounds.Intersects(boxCollider.bounds))
//                        {
//                            handleBlockOverlapping(ref deltaMovement);
//                        }
                    }

                    // the bottom ray can hit a slope but no other ray can so we have special handling for these cases
                    if ( i == 0 && handleHorizontalSlope( ref deltaMovement, Vector2.Angle( _raycastHit.normal, Vector2.up ) ) )
				    {
					    _raycastHitsXThisFrame.Add( _raycastHit );
					    break;
				    }

				    // set our new deltaMovement and recalculate the rayDistance taking it into account
				    deltaMovement.x = _raycastHit.point.x - ray.x;
				    rayDistance = Mathf.Abs( deltaMovement.x );

				    // remember to remove the skinWidth from our deltaMovement
				    if( isGoingRight )
				    {
                        deltaMovement.x -= _skinWidth;
					    collisionState.right = true;
                        collisionState.rightPoint = _raycastHit.point;
                        collisionState.rightNormal = _raycastHit.normal;
                        collisionState.rightObject = _raycastHit.collider;
                    }
				    else
				    {
                        deltaMovement.x += _skinWidth;
                        collisionState.left = true;
                        collisionState.leftPoint = _raycastHit.point;
                        collisionState.leftNormal = _raycastHit.normal;
                        collisionState.leftObject = _raycastHit.collider;
                    }

                    //Debug.Log($"deltaMovement = {deltaMovement}");
                    //Debug.Log($"rayDistance = {rayDistance}");
                    //Debug.Log($"_raycastHit.distance = {_raycastHit.distance}");
                    //Debug.Log("=================");

                    //collisionState.left = collisionState.faceDir == -1;
                    //collisionState.right = collisionState.faceDir == 1;

                    _raycastHitsXThisFrame.Add( _raycastHit );

				    // we add a small fudge factor for the float operations here. if our rayDistance is smaller
				    // than the width + fudge bail out because we have a direct impact
				    if( rayDistance < _skinWidth + kSkinWidthFloatFudgeFactor )
					    break;
			    }
		    }
	    }


	    /// <summary>
	    /// handles adjusting deltaMovement if we are going up a slope.
	    /// 如果我们正在上坡，则处理deltaMovement
	    /// </summary>
	    /// <returns><c>true</c>, if horizontal slope was handled, <c>false</c> otherwise.</returns>
	    /// <param name="deltaMovement">Delta movement.</param>
	    /// <param name="angle">Angle.</param>
	    bool handleHorizontalSlope( ref Vector2 deltaMovement, float angle )
	    {
		    // disregard 90 degree angles (walls)
		    if( Mathf.RoundToInt( angle ) == 90 )
			    return false;

		    // if we can walk on slopes and our angle is small enough we need to move up
		    if( angle < slopeLimit )
		    {
			    // we only need to adjust the deltaMovement if we are not jumping
			    // TODO: this uses a magic number which isn't ideal! The alternative is to have the user pass in if there is a jump this frame
			    if( deltaMovement.y < jumpingThreshold )
			    {
				    // apply the slopeModifier to slow our movement up the slope
				    var slopeModifier = slopeSpeedMultiplier.Evaluate( angle );
				    deltaMovement.x *= slopeModifier;

				    // we dont set collisions on the sides for this since a slope is not technically a side collision.
				    // smooth y movement when we climb. we make the y movement equivalent to the actual y location that corresponds
				    // to our new x location using our good friend Pythagoras
				    deltaMovement.y = Mathf.Abs( Mathf.Tan( angle * Mathf.Deg2Rad ) * deltaMovement.x );
				    var isGoingRight = deltaMovement.x > 0;

				    // safety check. we fire a ray in the direction of movement just in case the diagonal we calculated above ends up
				    // going through a wall. if the ray hits, we background off the horizontal movement to stay in bounds.
				    var ray = isGoingRight ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;
				    RaycastHit2D raycastHit;
				    if( collisionState.wasGroundedLastFrame )
					    raycastHit = Physics2D.Raycast( ray, deltaMovement.normalized, deltaMovement.magnitude, platformMask );
				    else
					    raycastHit = Physics2D.Raycast( ray, deltaMovement.normalized, deltaMovement.magnitude, platformMask & ~oneWayPlatformMask );

				    if( raycastHit )
				    {
					    // we crossed an edge when using Pythagoras calculation, so we set the actual delta movement to the ray hit location
					    deltaMovement = (Vector3)raycastHit.point - ray;
					    if( isGoingRight )
						    deltaMovement.x -= _skinWidth;
					    else
						    deltaMovement.x += _skinWidth;
				    }

				    _isGoingUpSlope = true;
				    collisionState.below = true;
                    collisionState.belowPoint = _raycastHit.point;
                    collisionState.belowNormal = _raycastHit.normal;
                    collisionState.belowObject = _raycastHit.collider;
                }
		    }
		    else // too steep. get out of here
		    {
			    deltaMovement.x = 0;
		    }

		    return true;
	    }


	    void moveVertically( ref Vector2 deltaMovement )
	    {
		    var isGoingUp = deltaMovement.y > 0;
		    var rayDistance = Mathf.Abs( deltaMovement.y ) + _skinWidth;
		    var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
		    var initialRayOrigin = isGoingUp ? _raycastOrigins.topLeft : _raycastOrigins.bottomLeft;

		    // apply our horizontal deltaMovement here so that we do our raycast from the actual position we would be in if we had moved
		    initialRayOrigin.x += deltaMovement.x;

		    // if we are moving up, we should ignore the layers in oneWayPlatformMask
		    var mask = platformMask;
		    //if( ( isGoingUp && !collisionState.wasGroundedLastFrame ) || ignoreOneWayPlatformsThisFrame )
            if (isGoingUp || ignoreOneWayPlatformsThisFrame)
                mask &= ~oneWayPlatformMask;

		    for( var i = 0; i < totalVerticalRays; i++ )
		    {
			    var ray = new Vector2( initialRayOrigin.x + i * _horizontalDistanceBetweenRays, initialRayOrigin.y );
#if UNITY_EDITOR
                DrawRay( ray, rayDirection * rayDistance, Color.red );
#endif
                _raycastHit = Physics2D.Raycast( ray, rayDirection, rayDistance, mask );
			    if( _raycastHit )
			    {
				    // set our new deltaMovement and recalculate the rayDistance taking it into account
				    deltaMovement.y = _raycastHit.point.y - ray.y;
				    rayDistance = Mathf.Abs( deltaMovement.y );

				    // remember to remove the skinWidth from our deltaMovement
				    if( isGoingUp )
				    {
                        deltaMovement.y -= _skinWidth;
                        collisionState.above = true;
                        collisionState.abovePoint = _raycastHit.point;
                        collisionState.aboveNormal = _raycastHit.normal;
                        collisionState.aboveObject = _raycastHit.collider;
                    }
				    else
				    {
                        deltaMovement.y += _skinWidth;
                        collisionState.below = true;
                        collisionState.belowPoint = _raycastHit.point;
                        collisionState.belowNormal = _raycastHit.normal;
                        collisionState.belowObject = _raycastHit.collider;
                    }

				    _raycastHitsYThisFrame.Add( _raycastHit );

				    // this is a hack to deal with the top of slopes. if we walk up a slope and reach the apex we can get in a situation
				    // where our ray gets a hit that is less then skinWidth causing us to be ungrounded the next frame due to residual velocity.
				    if( !isGoingUp && deltaMovement.y > 0.00001f )
					    _isGoingUpSlope = true;

				    // we add a small fudge factor for the float operations here. if our rayDistance is smaller
				    // than the width + fudge bail out because we have a direct impact
				    if( rayDistance < _skinWidth + kSkinWidthFloatFudgeFactor )
					    break;
			    }
		    }
	    }


	    /// <summary>
	    /// checks the center point under the BoxCollider2D for a slope. If it finds one then the deltaMovement is adjusted so that
	    /// the player stays grounded and the slopeSpeedModifier is taken into account to speed up movement.
	    /// 检查BoxCollider2D下方的中心点是否有斜坡，
	    /// 如果它找到一个，那么调整deltaMovement，以便玩家保持接地，并且考虑slopeSpeedModifier以加快运动。
	    /// </summary>
	    /// <param name="deltaMovement">Delta movement.</param>
	    private void handleVerticalSlope( ref Vector2 deltaMovement )
	    {
		    // slope check from the center of our collider
		    var centerOfCollider = ( _raycastOrigins.bottomLeft.x + _raycastOrigins.bottomRight.x ) * 0.5f;
		    var rayDirection = -Vector2.up;

		    // the ray distance is based on our slopeLimit
		    var slopeCheckRayDistance = _slopeLimitTangent * ( _raycastOrigins.bottomRight.x - centerOfCollider );

		    var slopeRay = new Vector2( centerOfCollider, _raycastOrigins.bottomLeft.y );
		    DrawRay( slopeRay, rayDirection * slopeCheckRayDistance, Color.yellow );
		    _raycastHit = Physics2D.Raycast( slopeRay, rayDirection, slopeCheckRayDistance, platformMask );
		    if( _raycastHit )
		    {
			    // bail out if we have no slope
			    var angle = Vector2.Angle( _raycastHit.normal, Vector2.up );
			    if( angle == 0 )
				    return;

			    // we are moving down the slope if our normal and movement direction are in the same x direction
			    var isMovingDownSlope = Mathf.Sign( _raycastHit.normal.x ) == Mathf.Sign( deltaMovement.x );
			    if( isMovingDownSlope )
			    {
				    // going down we want to speed up in most cases so the slopeSpeedMultiplier curve should be > 1 for negative angles
				    var slopeModifier = slopeSpeedMultiplier.Evaluate( -angle );
				    // we add the extra downward movement here to ensure we "stick" to the surface below
				    deltaMovement.y += _raycastHit.point.y - slopeRay.y - skinWidth;
				    deltaMovement.x *= slopeModifier;
				    collisionState.movingDownSlope = true;
				    collisionState.slopeAngle = angle;
			    }
		    }
	    }

        #endregion

    }
    
}
