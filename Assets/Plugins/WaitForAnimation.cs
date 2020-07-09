// 【Unity】Animatorのアニメーション終了待ちをする方法
// http://tsubakit1.hateblo.jp/entry/2016/02/11/021743

using UnityEngine;

public class WaitForAnimation : CustomYieldInstruction
{
    Animator m_animator;
    int m_lastStateHash = 0;
    int m_layerNo = 0;

    public WaitForAnimation(Animator animator, int layerNo)
    {
        if (animator == null)
            return;

        //Init(animator, layerNo, animator.GetCurrentAnimatorStateInfo(layerNo).nameHash);
        Init(animator, layerNo, animator.GetCurrentAnimatorStateInfo(layerNo).fullPathHash);
        //Init(animator, layerNo, animator.GetCurrentAnimatorStateInfo(layerNo).shortNameHash);
    }

    void Init(Animator animator, int layerNo, int hash)
    {
        m_layerNo = layerNo;
        m_animator = animator;
        m_lastStateHash = hash;
    }

    public override bool keepWaiting
    {
        get
        {
            if (m_animator == null)
                return false;
            var currentAnimatorState = m_animator.GetCurrentAnimatorStateInfo(m_layerNo);
            return currentAnimatorState.fullPathHash == m_lastStateHash &&
                (currentAnimatorState.normalizedTime < 1);
        }
    }
}