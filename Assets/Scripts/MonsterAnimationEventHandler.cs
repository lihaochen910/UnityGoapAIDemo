using UnityEngine;
using Enemy;


[RequireComponent(typeof(Animator))]
public class MonsterAnimationEventHandler : MonoBehaviour
{
    public event System.Action<string> OnCustomAnimationEvent;

    private SpriteRenderer actionRenderer;
    private Animator particleSEAnimator;

    private RuntimeAnimatorController particleSEAnimatorController;

    private Monster m_Monster;
    void Awake()
    {
        actionRenderer = GetComponent<SpriteRenderer>();
        particleSEAnimator = transform.Find("ParticleSEAnimator")?.GetComponent<Animator>();

        OnAnimatorControllerChanged();

        m_Monster = GetComponentInParent<Monster>();
    }
    /// <summary>
    /// 动画控制改变时，需要调用此方法记录控制器，防止在播放动作动画时控制器不是对应的
    /// </summary>
    public void OnAnimatorControllerChanged()
    {
        particleSEAnimatorController = particleSEAnimator?.runtimeAnimatorController;
    }
    /// <summary>
    /// 自定义动画事件分发
    /// </summary>
    /// <param name="customEvent"></param>
    public void DispatchCustomEvent(string customEvent)
    {
        OnCustomAnimationEvent?.Invoke(customEvent);
    }

	public void ExecuteSoundFXEvent ( string sfx ) {

		try {

			var str = sfx.Split(' ');

			string path, volumeScale = "1";
			string delay = "0";
			string pitch = "1";

			path = str [ 0 ];

			if ( str.Length > 1 ) {
				volumeScale = str[1];
			}
			if ( str.Length > 2 ) {
				delay = str [ 2 ];
			}
			if ( str.Length > 3 ) {
				pitch = str [ 3 ];
			}

			DoSFXCommand ( path, volumeScale, delay, pitch );
		}
		catch ( System.Exception e ) {
			Debug.LogError ( $"{transform.parent?.gameObject.name} 音效事件错误:{sfx} 动画关键帧:{actionRenderer.sprite.name}" );
		}

	}

	private void DoSFXCommand ( string path, string volumeScale, string delay, string pitch ) {
		// AudioPlayManager.PlaySFX2D ( path, ParseUtil.ParseFloat ( volumeScale ), ParseUtil.ParseFloat ( delay ), ParseUtil.ParseFloat ( pitch ) );
	}

	/// <summary>
	/// 处理自定义动画位移
	/// </summary>
	/// <param name="movementEvent">位移指令</param>
	public void ExecuteCustomAnimationMovementEventInfo(string movementEvent)
    {
        var str = movementEvent.Split(' ');

        if (str == null || str.Length < 2) {
            Debug.LogError($"MonsterAnimationEventHandler.ExecuteCustomAnimationMovementEventInfo() 动画事件格式不正确: {movementEvent}");
            return;
        }

        var cmd = str[0];
        var vec2 = str[1];

        DoMovementCommand(cmd, vec2);
    }
    /// <summary>
    /// 执行位移指令
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="moveAmount"></param>
    private void DoMovementCommand(string cmd, string moveAmount)
    {
        switch (cmd)
        {
            case "move":
                var deltaMovement = parseVector(moveAmount);
                deltaMovement.x *= m_Monster.FaceDirection;
                m_Monster.Move(deltaMovement);
                break;
        }
    }
    /// <summary>
    /// 处理自定义的动画事件
    /// </summary>
    /// <param name="animationEventInfo">自定义命令使用空格分隔</param>
    public void ExecuteCustomAnimationEventInfo(string animationEvent)
    {
        var str = animationEvent.Split(' ');

        var cmd = str[0];
        var clipID = str[1];
        var vec2 = str.Length > 2 ? str[2] : string.Empty;
        var depth = str.Length > 3 ? str[3] : string.Empty;

        //Debug.Assert(cmd != null, "cmd == null");
        //Debug.Assert(clipID != null, "clipID == null");
        //Debug.Log(cmd + " " + clipID + " " + vec2);

        try
        {
            DoCommand(cmd, clipID, vec2, depth);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"MonsterAnimationEventHandler::ExecuteCustomAnimationEventInfo() 解析{actionRenderer.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.name}动画事件时出现问题:{animationEvent}");
        }
    }

	/// <summary>
	/// 实例化通用特效
	/// </summary>
	/// <param name="animationEventInfo">自定义命令使用空格分隔</param>
	public void ApplySpecificEffect ( string animationEvent ) {

		var str = animationEvent.Split(' ');

		try {

			// var clipID = str[0];
			// var vec2 = str.Length > 1 ? ParseUtil.ParseVector2 ( str[1] ) : Vector2.zero;
			// var orderInLayer = str.Length > 2 ? ParseUtil.ParseInt ( str[2] ) : 1;
			// var follow = str.Length > 3 ? ParseUtil.ParseInt ( str[3] ) : 1;
			// var defaultShader = str.Length > 4 ? ParseUtil.ParseInt ( str[4] ) : 0;
			//
			// var eff = EffectManager.ApplySpecificEffectAt ( clipID, actionRenderer.transform.position + (Vector3)vec2 );
			// eff.GetComponent<SpriteRenderer> ().sortingOrder = orderInLayer;
			// if ( clipID == "AtkWarningEffect_Monster" ) {
			// 	eff.GetComponent< SpriteRenderer > ().sortingLayerName = "NearLayer";
			// }
			//
			// if ( follow == 1 ) {
			// 	eff.transform.SetParent ( actionRenderer.transform, true );
			// }
			//
			// if ( defaultShader != 1 ) {
			// 	eff.GetComponent<SpriteRenderer> ().material = EffectManager.Sprite_Additive;
			// }

		}
		catch ( System.Exception e ) {
			Debug.LogError ( $"MonsterAnimationEventHandler::ApplySpecificEffect() 解析{actionRenderer.GetComponent<Animator> ().GetCurrentAnimatorClipInfo ( 0 ) [ 0 ].clip.name}动画事件时出现问题:{animationEvent}" );
		}
	}
	/// <summary>
	/// 播放一次，结束时隐藏
	/// </summary>
	/// <param name="animator"></param>
	/// <param name="clipName"></param>
	/// <returns></returns>
	private Animator PlayOnceAndDisable(Animator animator, string clipName)
    {
        /// 取消使用缩放
        //animator.transform.localScale = actionRenderer.transform.localScale;
        /// 方向以人物动作精灵渲染器为准
        //animator.GetComponent<SpriteRenderer>().flipX = actionRenderer.flipX;

        /// 如果使用flipX属性直接改变精灵方向会导致碰撞盒与精灵图不一致，
        /// 尝试直接使用transform的Rotation绕Z轴旋转180度解决
        if (actionRenderer.transform.localRotation != Quaternion.identity)
            animator.transform.localEulerAngles = new Vector3(180, 0, 180);
        else animator.transform.localRotation = Quaternion.identity;
#if SHOWCLIP
        Debug.Log("<color=#FF1493>" + clipName + "</color>");
#endif

        animator.Play(clipName);

        animator.AnimationEndCallback(this, () =>
        {
            /// 播放招式后进入共有空闲状态Empty
            animator.Play("Empty");
        });

        return animator;
    }
    /// <summary>
    /// 用于播放一次性的粒子特效
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="clipName"></param>
    private void PlayOnceAndDestroy(Animator animator, string clipName)
    {
        PlayOnceAndDisable(animator, clipName).AnimationEndCallback(this, () =>
            {
                Destroy(animator.gameObject);
            });
    }
    /// <summary>
    /// 执行自定义动画事件
    /// </summary>
    /// <param name="cmd">命令</param>
    /// <param name="clipID">动画ID</param>
    /// <param name="vec2">动画实例化坐标</param>
    private void DoCommand(string cmd, string clipID, string vec2 = null, string depth = null)
    {
        switch (cmd)
        {
            /// 使用人物身上的武器粒子动画组件播放EffectAnimation，与刀光不同的是不会复用，不会跟随玩家的坐标，播放完成后会被销毁
            /// ParticleSEAnimation
            case "pse":
#if SHOWCMD
                Debug.Log("<color=#FF6A6A>pse : " + clipID + "</color>");
#endif
#if UNITY_EDITOR
                //Debug.Assert(!string.IsNullOrEmpty(vec2), "pse指令格式不正确");
#endif
                if (particleSEAnimator.runtimeAnimatorController != particleSEAnimatorController)
                    particleSEAnimator.runtimeAnimatorController = particleSEAnimatorController;

                var animator = Object.Instantiate(particleSEAnimator.gameObject,
                    transform.TransformPoint(string.IsNullOrEmpty(vec2) ? Vector3.zero : parseVector(vec2)), Quaternion.identity).GetComponent<Animator>();

                //var atkListener = animator.GetComponent<AttackEventListener>();
                //if (atkListener != null)
                //{
                //    var sourceListener = particleSEAnimator.GetComponent<AttackEventListener>();
                //    atkListener.onCollisionEnterEvent = sourceListener.onCollisionEnterEvent;
                //    atkListener.onCollisionStayEvent = sourceListener.onCollisionStayEvent;
                //    atkListener.onCollisionExitEvent = sourceListener.onCollisionExitEvent;
                //}

                if (!string.IsNullOrEmpty(depth))
                    animator.GetComponent<SpriteRenderer>().sortingOrder = actionRenderer.sortingOrder + int.Parse(depth);

                PlayOnceAndDestroy(animator, clipID);
                break;

            case "particle":

                bool flipX = actionRenderer.transform.rotation != Quaternion.identity;

                Vector3 offset_L = parseVector(vec2);
                Vector3 offset_R = parseVector(depth);

                // EffectManager.ApplyParticleEffectAt(clipID, 
                //     transform.position + (flipX ? offset_L : offset_R), flipX);

                break;
        }
    }
    /// <summary>
    /// 解析字符串坐标
    /// </summary>
    /// <param name="vec2">0,0</param>
    private Vector3 parseVector(string vec2)
    {
        var x = float.Parse(vec2.Split(',')[0]) * (actionRenderer.flipX ? -1 : 1);
        var y = float.Parse(vec2.Split(',')[1]);
        return new Vector3(x, y, 0);
    }
}

