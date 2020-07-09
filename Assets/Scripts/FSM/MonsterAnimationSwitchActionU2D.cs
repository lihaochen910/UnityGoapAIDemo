using Assets.Scripts.AI.BehaviorTree;
using Enemy;
using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory(ActionCategory.Animation)]
public class MonsterAnimationSwitchActionU2D : MonsterFsmStateAction
{
    public RuntimeAnimatorController controller;
    public FsmString animationClip;
    /// <summary>
    /// 动画正常播放结束后抛出事件
    /// </summary>
    public FsmEvent CompletedEvent;

    public bool forceFace2Player = false;
    /// <summary>
    /// 如果即将播放的动画与当前未播放完的动画相同，是否打断当前动画并覆盖
    /// </summary>
    public bool interruptSameAnimation = false;

    private Animator animator;
    /// <summary>
    /// 调试某些情况下，动画播放完成的回调事件没有正常触发
    /// </summary>
    //private float debugTimer;
    //private float debugLength;

    private PlayMakerFSM playmakerFSM;
    private Coroutine waitForAnimationToEndCoroutine;

    public override void Awake()
    {
        base.Awake();
        animator = transform.Find("ActionAnimator").GetComponent<Animator>();
        //debugLength = FsmEvent.IsNullOrEmpty(CompletedEvent) ? -1 : animator.GetAnimationClip(animationClip.Value).length;
        playmakerFSM = Owner.GetComponent<PlayMakerFSM>();
    }

    public override void OnEnter()
    {
#if UNITY_EDITOR
        if (controller == null)
            Debug.LogError($"{Owner.name} 位于" + State.Name + "状态的RuntimeAnimatorController引用没有被赋值");
#endif
        if (animator.runtimeAnimatorController != controller)
        {
            animator.runtimeAnimatorController = controller;
            //if (animator.GetComponent<AnimatorFinishEventTrigger>() == null)
            //    animator.gameObject.AddComponent<AnimatorFinishEventTrigger>();
            //animator.GetComponent<AnimatorFinishEventTrigger>().OnAnimatorControllerChanged();
        }
        /// 处理当前相同动画的情况
        if (interruptSameAnimation && animator.CurrentStateEqual(animationClip.Value))
        {
            animator.Play(animationClip.Value, 0, 0f);
        }
        else
            animator.Play(animationClip.Value);

        if (!FsmEvent.IsNullOrEmpty(CompletedEvent))
            waitForAnimationToEndCoroutine = animator.AnimationEndCallback(playmakerFSM, () => Fsm.Event(CompletedEvent));
        //animator.OnCurrentAnimationCompleted(() => Fsm.Event(CompletedEvent));

        if (forceFace2Player)
        {
            var target = Owner.GetComponent<NodeCanvas.Framework.Blackboard>().GetValue<Transform>("Target");

            // if (target == null)
            //     target = m_Player.transform;

            m_Monster.FaceDirection = Util.GetPlayerDirectionRelative(target.position, transform.position);
        }
    }

    public override void OnExit()
    {
        //debugTimer = 0;

        if (waitForAnimationToEndCoroutine != null)
            playmakerFSM.StopCoroutine(waitForAnimationToEndCoroutine);
    }

//    public override void OnUpdate()
//    {
//        if (debugLength < 0)
//            return;

//        if (debugTimer > debugLength * 1.2f)
//        {
//#if UNITY_EDITOR
//            Debug.LogWarning(Fsm.ActiveStateName + " : 动画完成事件可能没有正常被执行");
//#endif
//            Fsm.Event(CompletedEvent);
//        }

//        debugTimer += Time.deltaTime;
//    }
}
