// Decompiled with JetBrains decompiler
// Type: DamageDealer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E20EE8E7-4E23-4A85-927C-65F4005A4EC2
// Assembly location: D:\Games\Cuphead\Cuphead_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RPGStatSystem;

/// <summary>
/// 攻击判定结算类
/// </summary>
public class DamageDealer
{
    public float damage = 0f;
//    public float damageRate = 1f;
    public float damageMultiplier = 1f;
    public int damageApplyCount = 1;                        // 伤害结算次数，默认为1
    public DamageInfo damageInfo;
    public AttackType attackType;
    public AttackRangeType attackRangeType;
	public string moveName = string.Empty;					// 攻击招式命名
    public bool dontDealDamage = false;                     // 不结算伤害，只结算buff的特殊选项
    public bool dontApplyAttackType = false;                // 不会对目标造成硬直, 只造成伤害
    public bool ignoreReceiverOnDealDamage = false;         // 判定对每个Receiver只造成一次判定
    public bool ignoreBlock = false;                        // 无视目标格挡，即该攻击判定不会被格挡
    public bool ignoreDef = false;                          // 无视目标防御属性

    public Vector3 hitPoint;

    public bool isCrit;

    #region 玩家专属字段
    // public MoveType moveType = MoveType.ATK;
    public float hitStopTime = -1f;
    public float normalDamage, critDamage;          // 某些延迟伤害需要提前在初始化时计算好
    public bool usePreCalcDamage = false;           // 预先计算好伤害? 此开关激活时，isCrit标志位将在DealDamage被调用时计算，无需提前在外部赋值
    public bool dontCalcDamageInternal = false;     // 不在内部计算伤害，伤害已经在其他类中计算好的
    public bool dontApplySkillCardEffect = false;   // 不要结算技能卡效果
    public bool dontApplyLifeSteal = false;         // 不要触发生命偷取效果
    #endregion

    #region 怪专属字段
    public string monsterID = string.Empty;
    public RPGStatCollection statCollection;
	#endregion

#if DEBUG
	/// <summary>
	/// 记录DamageDealer生成的调用堆栈
	/// </summary>
	public string CtorStackTrace = System.Environment.StackTrace;
#endif

	public List<DamageReceiver> IgnoredReceivers { get; set; } = new List<DamageReceiver>(8);


    public delegate void OnDealDamageHandler(float damage, DamageReceiver receiver, DamageDealer dealer);

    public event OnDealDamageHandler OnDealDamage;


    public static DamageSource lastPlayerDamageSource;

    public bool useDamageInterval = false;
    public float damageInterval;
    private Dictionary<int, float> timers;
    private DamageDirection direction;
    public Transform origin;
    public DamageSource damageSource;
    private DamageTypesManager damageTypes;
    
    public DamageDealer ( float damage ) {
        Setup ( damage );
    }

    public DamageDealer(
      float damage,
      bool damagesPlayer,
      bool damagesEnemy,
      bool damagesOther)
    {
        this.Setup(damage, DamageDealer.DamageSource.Neutral, damagesPlayer, damagesEnemy, damagesOther, 1f);
    }

    public DamageDealer(
      float damage,
      DamageDealer.DamageSource damageSource,
      bool damagesPlayer,
      bool damagesEnemy,
      bool damagesOther)
    {
        this.Setup(damage, damageSource, damagesPlayer, damagesEnemy, damagesOther, 1f);
    }

    #region Creation Function
    public static DamageDealer NewPlayer () {
        return new DamageDealer ( 1f, DamageDealer.DamageSource.Player, true, false, true );
    }
    public static DamageDealer NewEnemy () {
        return new DamageDealer ( 1f, DamageDealer.DamageSource.Enemy, true, false, true );
    }
    #endregion

    private void Setup ( float damage ) {
        Setup ( damage, DamageSource.Neutral, true, false, false, 1f );
    }

    private void Setup ( float damage, DamageSource damageSource ) {
        Setup ( damage, damageSource, true, false, false, 1f );
    }

    private void Setup(
      float damage,
//      float damageRate,
      DamageDealer.DamageSource damageSource,
      bool damagesPlayer,
      bool damagesEnemy,
      bool damagesOther,
      float damageMultiplier = 1f)
    {
        this.damage = damage;
//        this.damageRate = damageRate;
        this.damageMultiplier = damageMultiplier;
        this.damageTypes = new DamageDealer.DamageTypesManager();
        this.SetDamageFlags(damagesPlayer, damagesEnemy, damagesOther);
        this.damageSource= damageSource;
        this.timers = new Dictionary<int, float>();
        this.hitStopTime = -1f;
    }

    public void SetDamageFlags ( bool damagesPlayer, bool damagesEnemy, bool damagesOther ) {
        damageTypes.Player = damagesPlayer;
        damageTypes.Enemies = damagesEnemy;
        damageTypes.Other = damagesOther;
    }

    public void SetDirection ( DamageDirection direction, Transform origin ) {
        this.direction = direction;
        this.origin = origin;

		if ( origin == null) {
            Debug.LogError ($"尝试设置伤害来源为Null，这可能导致后续逻辑出现问题\n{System.Environment.StackTrace}");
		}
    }
    
    public void IgnoreReceiver ( DamageReceiver receiver ) {
        if ( IgnoredReceivers.Contains ( receiver ) )
            return;
        IgnoredReceivers.Add ( receiver );
    }

    public void ClearIgnoredReceiver () {
        IgnoredReceivers.Clear ();
    }

    public bool DealDamage ( GameObject hit, Vector3 hitPoint ) {
        
        DamageReceiver receiver = FindObjectDamageReceiver ( hit );
        
        // 检查开关
        if ( receiver == null || !receiver.enabled )
            return false;

        // 检查忽略
        if ( IgnoredReceivers.Contains ( receiver ) )
            return false;

        // 检查类型匹配
        if ( !damageTypes.GetType ( receiver.type ) ) {
            //Debug.Log($"{hit.transform.GetPath()}目标类型与DamageDealer类型不匹配:{receiver.type} {damageTypes}");
            return false;
        }
        
        // 检查伤害判定间隔
        if ( useDamageInterval &&
             timers.ContainsKey ( receiver.GetInstanceID () ) &&
             timers[ receiver.GetInstanceID () ] < damageInterval ) {
            return false;
        }
        
        this.hitPoint = hitPoint;

        // 实际伤害值
        float realDamage = 0f;
        
        // 实际伤害结算次数
        int realDamageApplyCount = damageApplyCount;

        switch ( damageSource ) {
            
            case DamageSource.Player:
                break;

            case DamageSource.Enemy:
                
                if ( statCollection == null ) {
                    statCollection = origin?.GetComponent < RPGStatCollection > ();
                }

                if ( !dontDealDamage ) {
                    damageInfo                  = new DamageInfo ( damage, origin.gameObject );
                    damageInfo.damageSourceType = damageSource;
                    damageInfo.attackType       = attackType;
                    damageInfo.attackRangeType  = attackRangeType;
                    damageInfo.movesName        = moveName;
                }

                break;

            case DamageSource.Trap:

                if ( !dontDealDamage ) {
                    damageInfo                  = new DamageInfo ( damage, origin.gameObject );
                    damageInfo.damageSourceType = damageSource;
                    damageInfo.attackType       = attackType;
                    damageInfo.attackRangeType  = attackRangeType;
                    damageInfo.movesName        = moveName;
                    damageInfo.ignoreBlock      = ignoreBlock;
                }

                break;
        }

        // 伤害结算
        if ( damageInfo != null ) {
            
			//Debug.Log ( $"对{receiver.transform.GetPath ()}结算伤害" );
            for ( var i = 0; i < realDamageApplyCount; ++i ) {
                receiver.TakeDamage ( damageInfo );
            }
        }
        
        // 伤害结算完毕后，是否要忽略后续对相同Receiver的判定
        if ( ignoreReceiverOnDealDamage ) {
            IgnoreReceiver ( receiver );
        }

        if ( useDamageInterval ) {
            if ( !timers.ContainsKey ( receiver.GetInstanceID () ) ) {
                timers.Add ( receiver.GetInstanceID (), 0f );
            }
            timers[ receiver.GetInstanceID () ] = 0.0f;
        }
        

        OnDealDamage?.Invoke ( damage * damageMultiplier, receiver, this );
        
        return true;
    }
    
    public void Update () {
        foreach ( var id in timers.Keys.ToList () ) {
            timers [ id ] += CupheadTime.Delta;
        }
    }

    /// <summary>
    /// A的攻击是否被B格挡?
    /// 结构如下:
    /// RPGStatCollection
    ///     ‖―― FLAG_DEFENSE 0/1
    ///     ‖―― DEFENSE_DIR 0/1/-1
    /// </summary>
    static public bool IsBlocked ( Transform t_A, Transform t_B ) {
        
        RPGStatCollection statCollection = t_B.GetComponentInChildren < RPGStatCollection >();

        if ( statCollection == null ||
             !statCollection.ContainStat ( GlobalSymbol.FLAG_DEFENSE ) ||
             !statCollection.ContainStat ( GlobalSymbol.DEFENSE_DIR ) ) {
            return false;
        }

        if ( statCollection.GetStatValue ( GlobalSymbol.FLAG_DEFENSE ) == 0f ) {
            return false;
        }

        // 格挡方向
        int blockDirection = (int)statCollection.GetStatValue ( GlobalSymbol.DEFENSE_DIR );

        // A相对于B的方向
        int A_relative_to_the_B_direction = (int)Mathf.Sign ( t_A.position.x - t_B.position.x );

//        BoxCollider2D dBodyCollider = t_B.Find("DefenseBody")?.GetComponent<BoxCollider2D>();

//        if (dBodyCollider != null && dBodyCollider.enabled)
//            blockDirection = (int)Mathf.Sign(dBodyCollider.offset.x);

//        Debug.Log($"DamageDealer::IsBlocked() blockDirection = {blockDirection}" +
//                       $", A_relative_to_the_B_direction = {A_relative_to_the_B_direction}");

        // 标志为0时视为无死角防御
        if ( blockDirection == 0 ) {
            return true;
        }

        if ((A_relative_to_the_B_direction == 1 && blockDirection == 1) ||
            (A_relative_to_the_B_direction == -1 && blockDirection == -1))
        {
            return true;
        }

        if ((A_relative_to_the_B_direction == 1 && blockDirection == -1) ||
            (A_relative_to_the_B_direction == -1 && blockDirection == 1))
        {
            return false;
        }

        Debug.LogError($"DamageDealer::IsBlocked() 方法出现问题, blockDirection = {blockDirection}" +
                       $", A_relative_to_the_B_direction = {A_relative_to_the_B_direction}");
        return false;
    }

    /// <summary>
    /// 将伤害反弹给源物体
    /// </summary>
    /// <param name="damageInfo"></param>
    static public void ReboundDamage ( DamageInfo damageInfo ) {

        if ( damageInfo.source == null ) {
            Debug.Log ( $"DamageDealer::ReboundDamage() 无法追溯伤害来源: {damageInfo}" );
            return;
        }

        DamageReceiver receiver = null;
        
        switch ( damageInfo.damageSourceType ) {
            case DamageSource.Player:
                
                receiver = damageInfo.source.transform.Find ( "ActionAnimator" )?.GetComponent<DamageReceiver>();

                if ( receiver != null ) {
                    receiver.TakeDamage ( damageInfo );
                    
                }
                break;
            case DamageSource.Enemy:

                receiver = damageInfo.source.transform.Find ( "BodyCollider" )?.GetComponent<DamageReceiver>();

                if ( receiver != null ) {
                    receiver.TakeDamage ( damageInfo );
                    
                }
                break;
        }
    }

    /// <summary>
    /// 检查给定物体是否可以接受伤害
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    static public bool CheckObjectCanDealDamage ( GameObject obj ) {
        
        DamageReceiver receiver = obj.GetComponent<DamageReceiver>();

        if (receiver == null)
        {
            DamageReceiverChild component = obj.GetComponent<DamageReceiverChild>();
            if (component != null && component.enabled)
                receiver = component.Receiver;
        }

        if (receiver == null || !receiver.enabled)
            return false;

        return true;
    }

    /// <summary>
    /// 查找目标身上的伤害接收器组件
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    static public DamageReceiver FindObjectDamageReceiver ( GameObject obj ) {
        
        DamageReceiver receiver = obj?.GetComponent < DamageReceiver >();
        if ( receiver != null ) {
            return receiver;
        }

        DamageReceiverChild receiverChild = obj.GetComponent < DamageReceiverChild >();
        if ( receiverChild != null ) {
            return receiverChild.Receiver;
        }
        
        receiver = obj.GetComponentInChildren < DamageReceiver >();
        if ( receiver != null ) {
            return receiver;
        }
        
        receiverChild = obj.GetComponentInChildren < DamageReceiverChild >();
        if ( receiverChild != null ) {
            return receiverChild.Receiver;
        }

        return null;
    }

    /// <summary>
    /// 查找目标身上的RPG属性组件
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    static public RPGStatCollection FindObjectStatCollection ( GameObject obj ) {
        if ( obj == null ) {
            return null;
        }

        RPGStatCollection statCollection = null;

        statCollection = obj.GetComponentInParent < RPGStatCollection >();
        if ( statCollection == null ) {
            statCollection = obj.GetComponentInChildren < RPGStatCollection >();
        }
        
        return statCollection;
    }

    /// <summary>
    /// 查找目标身上的指定组件
    /// </summary>
    /// <param name="obj"></param>
    /// <typeparam name="T"></typeparam>
    static public T FindObjectT<T> ( GameObject obj ) where T : Component {
        
        if ( obj == null ) {
            return null;
        }

        T comp = null;

        comp = obj.GetComponentInParent < T >();
        if ( comp == null ) {
            comp = obj.GetComponentInChildren < T >();
        }
        
        return comp;
    }

    /// <summary>
    /// 目标是Boss??
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    static public bool TargetIsBoss ( GameObject obj ) {
        
        var statCollection = FindObjectStatCollection ( obj );
        if ( statCollection ) {
            return statCollection.name.Contains ( "Boss" );
        }

        return false;
    }
    
    static public bool CanShowDamageMsg ( DamageReceiver receiver ) {

        if ( receiver.type == DamageReceiver.Type.Other ) {
            if ( !receiver.CompareTag ( "RequireHitSE" ) || receiver.CompareTag ( "NoSE" ) ) {
                return false;
            }
        }

        return true;
    }
    
    /// <summary>
    /// 伤害来源
    /// </summary>
    public enum DamageSource {
        Neutral,
        Player,
        Enemy,
        Ex,
        Super,
        Pit,
        Trap
    }


    /// <summary>
    /// 伤害目标类型
    /// 用于筛选目标
    /// </summary>
    [Serializable]
    public class DamageTypesManager
    {
        public bool Player;
        public bool Enemies;
        public bool Other;

        public DamageDealer.DamageTypesManager Copy()
        {
            return this.MemberwiseClone() as DamageDealer.DamageTypesManager;
        }

        public void SetAll(bool b)
        {
            this.Player = b;
            this.Enemies = b;
            this.Other = b;
        }

        /// <summary>
        /// 设置伤害只对玩家生效
        /// </summary>
        public DamageDealer.DamageTypesManager OnlyPlayer()
        {
            this.SetAll(false);
            this.Player = true;
            return this;
        }

        /// <summary>
        /// 设置伤害只对敌人生效
        /// </summary>
        public DamageDealer.DamageTypesManager OnlyEnemies()
        {
            this.SetAll(false);
            this.Enemies = true;
            return this;
        }

        /// <summary>
        /// 该伤害类型是否可以对目标DamageReceiver组件的类型生效?
        /// 可以实现友军伤害控制
        /// </summary>
        /// <param name="type">DamageReceiver组件可接收伤害目标类型</param>
        public bool GetType(DamageReceiver.Type type)
        {
            switch (type)
            {
                case DamageReceiver.Type.Enemy:
                    return this.Enemies;
                case DamageReceiver.Type.Player:
                    return this.Player;
                case DamageReceiver.Type.Other:
                    return this.Other;
                default:
                    Debug.LogWarning("DamageType " + type + " not set up!");
                    return false;
            }
        }

        public override string ToString()
        {
            return $"Player:{Player}, Enemies:{Enemies}, Other:{Other}";
        }
    }
}
