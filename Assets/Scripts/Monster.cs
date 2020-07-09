using UnityEngine;
using RPGStatSystem;

namespace Enemy
{
    /// <summary>
    /// 怪物基础控制类，拥有走，跑的基础动作
    /// </summary>
    [RequireComponent(typeof(Prime31.CharacterController2D))]
    public class Monster : AbstractPausableComponent
    {
        [HideInInspector]
        public RPGStatCollection StatCollection;
        /// <summary>
        /// 控制器引用
        /// </summary>
        [HideInInspector]
        public Prime31.CharacterController2D controller;
        /// <summary>
        /// 精灵渲染器引用
        /// </summary>
        private SpriteRenderer m_SpriteRenderer;
        public SpriteRenderer Renderer { get { return m_SpriteRenderer; } }
        /// <summary>
        /// 面朝方向，精灵图的默认朝向是右
        /// </summary>
        public int FaceDirection
        {
            get { return m_SpriteRenderer.transform.rotation == Quaternion.identity ? 1 : -1; }
            set {
                if (value == -1)
                {
                    controller.boxCollider.offset = new Vector2(-Mathf.Abs(controller.boxCollider.offset.x), controller.boxCollider.offset.y);
                    m_SpriteRenderer.transform.localEulerAngles = new Vector3(180, 0, 180);
                    if (controller.collisionState.faceDir != value) processCollision();
                    controller.collisionState.faceDir = -1;
                }
                else
                {
                    controller.boxCollider.offset = new Vector2(Mathf.Abs(controller.boxCollider.offset.x), controller.boxCollider.offset.y);
                    m_SpriteRenderer.transform.localRotation = Quaternion.identity;
                    if (controller.collisionState.faceDir != value) processCollision();
                    controller.collisionState.faceDir = 1;
                }
            }
        }
        /// <summary>
        /// 输入轴值
        /// </summary>
        Vector2 directionalInput;
        /// <summary>
        /// 质量
        /// </summary>
        public float Mass;
        /// <summary>
        /// 速度
        /// </summary>
        public Vector2 velocity;
        private Vector2 lastFrameVelocity;
        float velocityXSmoothing;
        /// <summary>
        /// 重力
        /// </summary>
        float gravity
        {
            get { return StatCollection.GetStatValue(GlobalSymbol.GRAVITY); }
            set { StatCollection.SetStatValue(GlobalSymbol.GRAVITY, value); }
        }
        /// <summary>
        /// 是否激活重力
        /// </summary>
        public bool ActiveGravity = true;

        [Header("调试控制")] public bool MoveByDirectionInput = false;
        /// <summary>
        /// 是否激活控制
        /// </summary>
        private bool _activeControl = true;
        public bool ActiveControl
        {
            get { return _activeControl; }
            set
            {
                if (value == false)
                    directionalInput = Vector2.zero;
                _activeControl = value;
            }
        }
        /// <summary>
        /// 当前是否在地面上
        /// </summary>
        public bool isGrounded
        {
            get { return controller.isGrounded; }
        }

        protected virtual void Awake()
        {
            base.Awake();
            base.ignoreGlobalTime = false;
            base.timeLayer = CupheadTime.Layer.Enemy;
            
            controller = GetComponent<Prime31.CharacterController2D>();
            m_SpriteRenderer = transform.Find("ActionAnimator")?.GetComponent<SpriteRenderer>();
            StatCollection = GetComponent<RPGStatCollection>();

            if (StatCollection == null)
            {
                Debug.LogError($"{gameObject.name} 物体上没有找到<RPGStatCollection>组件，无法继续初始化<Monster>组件");
            }
        }

        protected virtual void Update()
        {
            /// 重力加速度
            if (ActiveGravity)
                velocity.y += gravity * CupheadTime.Delta[timeLayer];

            if (MoveByDirectionInput)
            {
                float targetVelocityX = directionalInput.x * StatCollection.GetStatValue(GlobalSymbol.RUN_SPEED);

                velocity.x = Mathf.MoveTowards(velocity.x, targetVelocityX,
                    isGrounded ? StatCollection.GetStatValue(GlobalSymbol.ACCELERATION_TIME_GROUND) : StatCollection.GetStatValue(GlobalSymbol.ACCELERATION_TIME_AIR));
            }

            if (!_activeControl)
                velocity.x = 0;

            /// 平台碰撞
            //if (controller.collisions.above || controller.collisions.below)
            //    velocity.y = 0;

            controller.Move(velocity * CupheadTime.Delta[timeLayer]);

            /// 平台碰撞
            if (controller.collisionState.above && velocity.y > 0f || controller.collisionState.below && velocity.y < 0f)
                velocity.y = 0.0f;
            if (controller.collisionState.left && velocity.x < 0f || controller.collisionState.right && velocity.x > 0f)
                velocity.x = 0.0f;
        }

#if UNITY_EDITOR
        //void OnGUI()
        //{
        //    GUI.Label(new Rect(0, 20, 150, 100), "Monster Panel");
        //    GUI.Label(new Rect(0, 40, 300, 100), "ActiveControl:" + ActiveControl);
        //    GUI.Label(new Rect(0, 60, 300, 100), "gravity:" + gravity);
        //    GUI.Label(new Rect(0, 80, 300, 100), "velocity:" + velocity + controller.velocity);
        //    GUI.Label(new Rect(0, 100, 300, 100), "moveAmount:" + velocity * Time.deltaTime);
        //    GUI.Label(new Rect(0, 120, 300, 100), "left:" + controller.collisionState.left + " " +
        //                                          "right:" + controller.collisionState.right + " " +
        //                                          "below:" + controller.collisionState.below + " " +
        //                                          "above:" + controller.collisionState.above);
        //    GUI.Label(new Rect(0, 140, 300, 100), "faceDir:" + controller.collisionState.faceDir);
        //    GUI.Label(new Rect(0, 160, 300, 100), "FaceDirection:" + FaceDirection);
        //    GUI.Label(new Rect(0, 180, 300, 100), "directionalInput:" + directionalInput);
        //}
#endif
        
        /// <summary>
        /// 更改faceDir之后需要处理平台碰撞盒与地形的重叠
        /// </summary>
        private void processCollision()
        {
            var center = (Vector2)transform.position + controller.boxCollider.offset;
            var size = controller.boxCollider.size;

            Collider2D[] obstacles = Physics2D.OverlapBoxAll(center, size, 0, controller.platformMask & ~controller.oneWayPlatformMask);

            foreach (var obstacle in obstacles)
            {
                //　忽略脚下
                if (obstacle.bounds.max.y <= controller.boxCollider.bounds.min.y + controller.skinWidth)
                    continue;

                // 判断障碍在左边还是右边
                int obstacleDir = obstacle.bounds.center.x > center.x ? 1 : -1;

                // 如果障碍物在右边，则把位置向左移动，直到不再与地形相交
                if (obstacleDir == 1)
                {
                    var offsetX = (center.x + size.x / 2) - obstacle.bounds.min.x;
                    //transform.SetPosition(transform.position.x - offsetX, null, null);
                    controller.Move(new Vector2(-offsetX, 0f));
                    //Debug.Log($"processCollision() Right {obstacle.transform.GetPath()}");
                    return;
                }
                else
                {
                    var offsetX = obstacle.bounds.max.x - (center.x - size.x / 2);
                    //transform.SetPosition(transform.position.x + offsetX, null, null);
                    controller.Move(new Vector2(offsetX, 0f));
                    //Debug.Log($"processCollision() Left {obstacle.transform.GetPath()}");
                    return;
                }
            }
        }

        /// <summary>
        /// 设置输入轴值
        /// </summary>
        /// <param name="input"></param>
        public void SetDirectionalInput(Vector2 input)
        {
            if (!_activeControl)
                return;
            directionalInput = input;
        }

        public void ClearVelocity()
        {
            velocity = Vector2.zero;
        }

        public Vector2 GetDirectionalInput()
        {
            return directionalInput;
        }

        #region Action at EveryFrame
        public void Walk(int dir, bool syncFaceDir = false)
        {
            if (syncFaceDir && dir != FaceDirection)
                FaceDirection = dir;

            velocity.x = Mathf.MoveTowards(velocity.x, 
                dir * StatCollection.GetStatValue(GlobalSymbol.WALK_SPEED), 
                controller.collisionState.below ? StatCollection.GetStatValue(GlobalSymbol.ACCELERATION_TIME_GROUND) : StatCollection.GetStatValue(GlobalSymbol.ACCELERATION_TIME_AIR));

            //controller.Move(velocity * Time.deltaTime, velocity);
        }

        public void Walk()
        {
            velocity.x = Mathf.MoveTowards(velocity.x,
                FaceDirection * StatCollection.GetStatValue(GlobalSymbol.WALK_SPEED),
                controller.collisionState.below ? StatCollection.GetStatValue(GlobalSymbol.ACCELERATION_TIME_GROUND) : StatCollection.GetStatValue(GlobalSymbol.ACCELERATION_TIME_AIR));
        }

        public void Run(int dir, bool syncFaceDir = false)
        {
            if (syncFaceDir && dir != FaceDirection)
                FaceDirection = dir;

            velocity.x = Mathf.MoveTowards(velocity.x,
                dir * StatCollection.GetStatValue(GlobalSymbol.RUN_SPEED), 
                controller.collisionState.below ? StatCollection.GetStatValue(GlobalSymbol.ACCELERATION_TIME_GROUND) : StatCollection.GetStatValue(GlobalSymbol.ACCELERATION_TIME_AIR));

            //controller.Move(velocity * Time.deltaTime, velocity);
        }

        public void Run()
        {
            velocity.x = Mathf.MoveTowards(velocity.x,
                FaceDirection * StatCollection.GetStatValue(GlobalSymbol.RUN_SPEED),
                controller.collisionState.below ? StatCollection.GetStatValue(GlobalSymbol.ACCELERATION_TIME_GROUND) : StatCollection.GetStatValue(GlobalSymbol.ACCELERATION_TIME_AIR));
        }

        public void Explore(int dir)
        {
            //velocity.x = Mathf.MoveTowards(velocity.x,
            //    dir * property.exploreSpeed, 
            //    controller.collisionState.below ? property.AccelerationTimeGrounded : property.AccelerationTimeAirborne);

            velocity.x = Mathf.Lerp(velocity.x, dir * StatCollection.GetStatValue(GlobalSymbol.EXPLORE_SPEED),
                controller.collisionState.below ? StatCollection.GetStatValue(GlobalSymbol.ACCELERATION_TIME_GROUND) : StatCollection.GetStatValue(GlobalSymbol.ACCELERATION_TIME_AIR));

            //controller.Move(velocity * Time.deltaTime, velocity);
        }
        /// <summary>
        /// 自定义位移
        /// </summary>
        /// <param name="moveAmount"></param>
        public void Move(Vector2 moveAmount)
        {
            controller.Move(moveAmount);
        }

        /// <summary>
        /// 对控制器主角施加一个力
        /// </summary>
        /// <param name="vel">速度向量</param>
        /// <param name="mode">力模式</param>
        public void AddForce(Vector2 forceVelocity, ForceMode2D mode)
        {
            if (this.Mass >= 1000.0f)
                return;
            if (mode == ForceMode2D.Force)
            {
                this.velocity += forceVelocity / this.Mass;
            }
            else
            {
                if (forceVelocity.x != 0.0f)
                    this.velocity.x = forceVelocity.x / this.Mass;
                if (forceVelocity.y == 0.0f)
                    return;
                this.velocity.y = forceVelocity.y / this.Mass;
            }
        }
        #endregion

        public void Jump()
        {
            velocity.y = Mathf.Abs(gravity) * StatCollection.GetStatValue(GlobalSymbol.TIME_TO_JUMP_APEX);
        }
    }
}
