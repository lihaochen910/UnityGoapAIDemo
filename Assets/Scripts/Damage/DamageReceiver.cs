// Cuphead DamageReceiver
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: Cuphead\Cuphead_Data\Managed\Assembly-CSharp.dll
using System;
using System.Threading;
using UnityEngine;
/// <summary>
/// 参考茶杯头的伤害信息接收器
/// </summary>
public class DamageReceiver : MonoBehaviour
{
    /// <summary>
    /// 伤害接收类型
    /// </summary>
    public enum Type
    {
        Enemy,
        Player,
        Other
    }

    public delegate void OnDamageTakenHandler(DamageInfo info);
    // public delegate void OnDamageBuffTakenHandler(DamageBuffInfo buffInfo);

    /// <summary>
    /// 设定此组件可以接受哪种目标类型的伤害
    /// </summary>
    public Type type;

    private event OnDamageTakenHandler onDamageTaken;
    public event OnDamageTakenHandler OnDamageTaken
    {
        add
        {
            OnDamageTakenHandler onDamageTakenHandler = this.onDamageTaken;
            OnDamageTakenHandler onDamageTakenHandler2;
            do
            {
                onDamageTakenHandler2 = onDamageTakenHandler;
                onDamageTakenHandler = Interlocked.CompareExchange<OnDamageTakenHandler>(ref this.onDamageTaken, (OnDamageTakenHandler)Delegate.Combine(onDamageTakenHandler2, value), onDamageTakenHandler);
            }
            while ((object)onDamageTakenHandler != onDamageTakenHandler2);
        }
        remove
        {
            OnDamageTakenHandler onDamageTakenHandler = this.onDamageTaken;
            OnDamageTakenHandler onDamageTakenHandler2;
            do
            {
                onDamageTakenHandler2 = onDamageTakenHandler;
                onDamageTakenHandler = Interlocked.CompareExchange<OnDamageTakenHandler>(ref this.onDamageTaken, (OnDamageTakenHandler)Delegate.Remove(onDamageTakenHandler2, value), onDamageTakenHandler);
            }
            while ((object)onDamageTakenHandler != onDamageTakenHandler2);
        }
    }

    // private event OnDamageBuffTakenHandler onDamageBuffTaken;
    // public event OnDamageBuffTakenHandler OnDamageBuffTaken
    // {
    //     add
    //     {
    //         OnDamageBuffTakenHandler onDamageBuffTakenHandler = this.onDamageBuffTaken;
    //         OnDamageBuffTakenHandler onDamageBuffTakenHandler2;
    //         do
    //         {
    //             onDamageBuffTakenHandler2 = onDamageBuffTakenHandler;
    //             onDamageBuffTakenHandler = Interlocked.CompareExchange<OnDamageBuffTakenHandler>(ref this.onDamageBuffTaken, (OnDamageBuffTakenHandler)Delegate.Combine(onDamageBuffTakenHandler2, value), onDamageBuffTakenHandler);
    //         }
    //         while ((object)onDamageBuffTakenHandler != onDamageBuffTakenHandler2);
    //     }
    //     remove
    //     {
    //         OnDamageBuffTakenHandler onDamageBuffTakenHandler = this.onDamageBuffTaken;
    //         OnDamageBuffTakenHandler onDamageBuffTakenHandler2;
    //         do
    //         {
    //             onDamageBuffTakenHandler2 = onDamageBuffTakenHandler;
    //             onDamageBuffTakenHandler = Interlocked.CompareExchange<OnDamageBuffTakenHandler>(ref this.onDamageBuffTaken, (OnDamageBuffTakenHandler)Delegate.Remove(onDamageBuffTakenHandler2, value), onDamageBuffTakenHandler);
    //         }
    //         while ((object)onDamageBuffTakenHandler != onDamageBuffTakenHandler2);
    //     }
    // }

    void OnEnable() { }
    void OnDisable() { }
    
    /// <summary>
    /// 伤害公用接口
    /// </summary>
    /// <param name="info"></param>
    public virtual void TakeDamage(DamageInfo info)
    {
        if (base.enabled)
        {
            onDamageTaken?.Invoke(info);
        }
    }
    
    /// <summary>
    /// buff公用接口
    /// </summary>
    // public virtual void ApplyBuff(DamageBuffInfo buffInfo)
    // {
    //     if (base.enabled)
    //     {
    //         onDamageBuffTaken?.Invoke(buffInfo);
    //     }
    // }
}
