using UnityEngine;
using System.Collections;

/// <summary>
/// Unity Animator扩展类
/// </summary>
public static class AnimatorEx {

    /// <summary>
    /// 给定动画剪辑的名字，反向查找动画剪辑资源
    /// </summary>
    /// <param name="name"></param>
    public static AnimationClip GetAnimationClip(this Animator animator, string name)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }
#if UNITY_EDITOR
        Debug.LogWarning("No " + name + " Found in " + animator.name + " with " + animator.runtimeAnimatorController.name);
#endif  
        return null; // no clip by that name
    }

    public static AnimationClip GetAnimationClip(this Animator animator, RuntimeAnimatorController runtimeController, string name)
    {
        foreach (AnimationClip clip in runtimeController.animationClips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }
#if UNITY_EDITOR
        Debug.LogWarning("No " + name + " Found in " + animator.name + " with " + runtimeController.name);
#endif  
        return null; // no clip by that name
    }
    /// <summary>
    /// 判断某Animator中的第0层是否存在指定状态
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateName"></param>
    /// <returns></returns>
    public static bool HasState(this Animator animator, string stateName)
    {
        return animator.HasState(0, Animator.StringToHash(stateName));
    }

    /// <summary>
    /// 获取Animator的精灵渲染器
    /// </summary>
    /// <returns>2D sprite renderer</returns>
    public static SpriteRenderer GetRenderer ( this Animator animator ) {
        return animator.GetComponent< SpriteRenderer > ();
    }

    /// <summary>
    /// 比较当前正在播放的动画状态
    /// </summary>
    /// <param name="stateName"></param>
    public static bool CurrentStateEqual ( this Animator animator, string stateName ) {
        return animator.GetCurrentAnimatorStateInfo ( 0 ).shortNameHash.Equals ( Animator.StringToHash ( stateName ) );
    }

    /// <summary>
    /// 获取当前动画的播放进度0~1(NormalizedTime)
    /// 如果是循环动画则可能返回>1的值
    /// </summary>
    public static float GetCurrentAnimationProgress ( this Animator animator, bool ignoreLoop = false ) {
        if ( !ignoreLoop ) {
            return animator.GetCurrentAnimatorStateInfo ( 0 ).normalizedTime;
        }

        return Mathf.Repeat ( animator.GetCurrentAnimatorStateInfo ( 0 ).normalizedTime, 1.0f );
    }

    /// <summary>
    /// 当前动画结束后执行一个回调，比动画事件更加可靠
    /// </summary>
    public static Coroutine AnimationEndCallback(this Animator animator, MonoBehaviour parent, System.Action callback = null, bool waitForEndOfFrame = false)
    {
        return parent.StartCoroutine(animationEndCallback_cr(animator, parent, callback, waitForEndOfFrame));
    }

    /// <summary>
    /// 当前动画结束后执行一个回调，比动画事件更加可靠
    /// </summary>
    public static Coroutine AnimationEndCallback(this Animator animator, MonoBehaviour parent, System.Action<string> callback = null)
    {
        return parent.StartCoroutine(animationEndCallback_cr2(animator, parent, callback));
    }

    private static IEnumerator animationEndCallback_cr(this Animator animator, MonoBehaviour parent, System.Action callback, bool waitForEndOfFrame)
    {
        // Animator.Play之后当前帧不会立刻切换到下一状态，需要等待一帧
        yield return null;
        // 第二个空帧是避免玩家状态机加载玩家预输入指令时，可能有过早判定为结束的情况
        yield return null;

        yield return new WaitForAnimation(animator, 0);

        //Debug.Log("after:" + animator.GetCurrentAnimatorClipInfo(0)?[0].clip.name);

        /// 执行回调
        if (callback != null)
            callback();
    }

    private static IEnumerator animationEndCallback_cr2(this Animator animator, MonoBehaviour parent, System.Action<string> callback)
    {
        // Animator.Play之后当前帧不会立刻切换到下一状态，需要等待一帧
        yield return null;
        // 第二个空帧是避免玩家状态机加载玩家预输入指令时，可能有过早判定为结束的情况
        yield return null;

        yield return new WaitForAnimation(animator, 0);

        /// 执行回调
        //if (callback != null)
        //    callback(animator.GetCurrentAnimatorClipInfo(0)?[0].clip.name);
        callback?.Invoke(string.Empty);
    }
    
}
