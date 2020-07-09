using UnityEngine;


/// <summary>
/// 受击信息结构体
/// </summary>
public class DamageInfo {
    /// <summary>
    /// 伤害值
    /// </summary>
    public float value;

    /// <summary>
    /// 无视格挡?
    /// </summary>
    public bool ignoreBlock;
    
    /// <summary>
    /// 静止时间
    /// </summary>
    public float hitStopTime;

    /// <summary>
    /// 此攻击不会对目标造成硬直, 只造成伤害
    /// </summary>
    public bool noAttackType;
    
    /// <summary>
    /// How far the victim will fly upon hit.  This overrides the push impulse from attacker hit
    /// </summary>
    public Vector2 pushImpulse;

    /// <summary>
    /// 招式名
    /// </summary>
    public string movesName;

    /// <summary>
    /// 伤害作用方向
    /// </summary>
    public DamageDirection damageDirection;

    /// <summary>
    /// 伤害来源类型
    /// </summary>
    public DamageDealer.DamageSource damageSourceType;

    /// <summary>
    /// 攻击强度类型
    /// </summary>
    public AttackType attackType;

    /// <summary>
    /// 攻击距离类型
    /// </summary>
    public AttackRangeType attackRangeType;
    
    /// <summary>
    /// 伤害来源
    /// </summary>
    public readonly GameObject source;

    /// <summary>
    /// 打击特效生成坐标
    /// </summary>
    public Vector3 effectPoint = Vector3.zero;

    public DamageInfo ( string     MovesName,
                        float      value, AttackType attackType, DamageDirection damageDirection,
                        GameObject source ) {
        this.movesName       = MovesName;
        this.value           = value;
        this.attackType      = attackType;
        this.damageDirection = damageDirection;
        this.source          = source;
    }

    public DamageInfo ( float value, GameObject source ) {
        this.value           = value;
        this.attackType      = AttackType.Hit;
        this.damageDirection = DamageDirection.Neutral;
        this.source          = source;
    }

    public DamageInfo ( float value, AttackType attackType, GameObject source ) {
        this.value           = value;
        this.attackType      = attackType;
        this.damageDirection = DamageDirection.Neutral;
        this.source          = source;
    }

    public DamageInfo Clone () {
        return ( DamageInfo )this.MemberwiseClone ();
    }

    public override string ToString () {
        return
            $"{damageSourceType} {source} {movesName} {attackType} {value} (ignoreBlock:{ignoreBlock}) (hitStopTime:{hitStopTime})";
    }
}


/// <summary>
/// 伤害作用方向
/// </summary>
public enum DamageDirection {
    Neutral = 0,  // 无方向
    Left    = -1, // 伤害作用于2D世界向左
    Right   = 1   // 伤害作用于2D世界向右
}

/// <summary>
/// 攻击强度类型
/// </summary>
public enum AttackType {
    Cut,     // 斩
    Hit,     // 打击
    Remote,  // 远程
    Heavy,   // 重击
    Fatal,   // 致命
    Special, // 特殊
}

/// <summary>
/// 攻击距离类型
/// </summary>
public enum AttackRangeType {
    Melee,   // 打击
    Remote,  // 远程
}
