
public static class GlobalSymbol {

	#region 战斗属性相关
	public const string HP                    = "HP";
	public const string ENERGY                = "Energy";
	public const string ENERGY_RECOVERY_SPEED = "EnergyRecoverySpeed";
	public const string ATK                   = "Atk";
	public const string ATK3                  = "Atk_3";
	public const string ATK_SKILL             = "Atk_skill";
	public const string ATK_TYPE              = "AttackType";
	public const string DEF                   = "Def";
	public const string CRIT_RATE             = "Crit_rate";
	public const string CRIT_DAMAGE           = "Crit_damage";
	public const string ANTI_INJURY           = "Anti_injury";
	public const string BURNING_RATE          = "Burning_rate";
	public const string BURNING_DAMAGE        = "Burning_damage";
	public const string FROZEN_RATE           = "Frozen_rate";
	public const string FROZEN_DAMAGE         = "Frozen_damage";
	public const string POISON_RATE           = "Poison_rate";
	public const string POISON_DAMAGE         = "Poison_damage";
	public const string STUN_RATE             = "Stun_rate";
	public const string DODGE_RATE            = "Dodge_rate";
	public const string WEIGHT                = "Weight";
	public const string LUCKY                 = "Lucky";
	public const string LIFESTEAL             = "LifeSteal";
	public const string LIFESTEAL_RATE        = "LifeSteal_rate";
	public const string LIFESTEAL_SKILL       = "LifeSteal_skill";
	public const string BAG_NOREDUCE_RATE     = "Bag_NoReduce_rate";
	public const string FLAG_INVINCIBLE       = "Invincible";
	public const string FLAG_DEFENSE          = "Defense";
	public const string DEFENSE_DIR           = "DefenseDirection"; // 0: 左右， 1: 世界方向右， -1:世界方向左
	public const string FLAG_SUPER_DEFENSE    = "SuperDefense";
	public const string STAMINA               = "Stamina";
	public const string SUFFERANCE            = "Sufferance";
	public const string EASY_SUPER_DEFENSE    = "EasySuperDefense";
	public const string FLAG_TARGET_LOCKED    = "TargetLocked";    // 黑板目标锁定标志
	public const string FLAG_BLOCK_FIRE       = "BlockFire";       // 火属性无效标志
	public const string FLAG_BLOCK_ICE        = "BlockIce";        // 冰属性无效标志
	public const string FLAG_BLOCK_POISON     = "BlockPoison";     // 毒属性无效标志
	public const string FLAG_BLOCK_STUN       = "BlockStun";       // 眩晕无效标志
	public const string FLAG_BLOCK_DEEPFREEZE = "BlockDeepFreeze"; // 冰封无效标志
	public const string FLAG_BLOCK_TAUNT      = "BlockTaunt";      // 嘲讽无效标志
	public const string FLAG_BLOCK_CHAOS      = "BlockChaos";      // 混乱无效标志
	public const string FLAG_BLOCK_DEVOUR     = "BlockDevour";     // 吞噬无效标志
	#endregion

	#region 2D平台控制属性相关
	public const string PROTECTION_TIME = "ProtectionTime";
	public const string MOVE_SPEED = "MoveSpeed";
	public const string MOVE_SPEED_AIR = "MoveSpeed_Air";
	public const string MOVE_SPEED_X_DROP = "MoveSpeedX_Drop";
	public const string ROLL_SPEED = "RollSpeed";
	public const string ACCELERATION_TIME_GROUND = "AccelerationTimeGround";
	public const string ACCELERATION_TIME_AIR = "AccelerationTimeAir";
	public const string AIR_SPRINT_DISTANCE = "AirSprintDistance";
	public const string AIR_SPRINT_TIME = "AirSprintTime";
	public const string JUMP_MAX_SEGMENTS = "JumpMaxSegments";
	public const string GRAVITY = "Gravity";
	public const string MAX_JUMP_HEIGHT = "MaxJumpHeight";
	public const string MIN_JUMP_HEIGHT = "MinJumpHeight";
	public const string TIME_TO_JUMP_APEX = "TimeToJumpApex";
	public const string TIME_TO_WALLJUMP_APEX = "TimeToWallJumpApex";
	public const string MAX_WALLJUMP_HEIGHT = "MaxWallJumpHeight";
	public const string WALLJUMP_CLIMB_X = "WallJumpClimbX";
	public const string WALLJUMP_CLIMB_Y = "WallJumpClimbY";
	public const string WALL_SLIDE_SPEED_MAX = "WallSlideSpeedMax";
	public const string MAX_CLIMB_DISTANCE_X = "MaxClimbDistanceX";
	public const string MAX_CLIMB_DISTANCE_Y = "MaxClimbDistanceY";
	public const string CLIMB_TIME_Y = "ClimbTimeY";
	public const string CLIMB_OFFSET_Y = "ClimbOffsetY";
	public const string AFTER_CLIMBING_MOVEMENT_X = "AfterClimbingMovementX";
	public const string AFTER_CLIMBING_MOVEMENT_X_TIME = "AfterClimbingMovementXTime";
	
	public const string WALK_SPEED = "WalkSpeed";
	public const string RUN_SPEED = "RunSpeed";
	public const string EXPLORE_SPEED = "ExploreSpeed";
	public const string JUMP_BACK_DISTANCE_X = "JumpBackDistanceX";
	public const string JUMP_BACK_DISTANCE_Y = "JumpBackDistanceY";
	public const string JUMP_BACK_ANGLE = "JumpBackAngle";
	public const string JUMP_BACK_GRAVITY = "JumpBackGravity";
	public const string JUMP_BACK_SPEED_X = "JumpBackSpeedX";
	public const string JUMP_BACK_SPEED_Y = "JumpBackSpeedY";
	public const string JUMP_ATK_DURATION = "JumpAttackDuration";
	public const string JUMP_ATK_FRAME_RATE = "JumpAttackFrameRate";
	public const string JUMP_ATK_HEIGHT = "JumpAttackHeight";
	#endregion

	#region 2D平台Layer相关
	public const string LAYER_PLATFORM = "Platform";
	public const string LAYER_ONEWAY_PLATFORM = "OneWayPlatform";
	public const string LAYER_WALL = "Wall";
	#endregion

	#region 范围相关
	public const string LOOK_RANGE_X = "LookRangeX";
	public const string LOOK_RANGE_Y = "LookRangeY";
	public const string CHASE_RANGE_X = "ChaseRangeX";
	public const string CHASE_RANGE_Y = "ChaseRangeY";
	public const string MELEE_ATTACK_RANGE_X = "MeleeAttackRangeX";
	public const string MELEE_ATTACK_RANGE_Y = "MeleeAttackRangeY";
	public const string REMOTE_ATTACK_RANGE_X = "RemoteAttackRangeX";
	public const string REMOTE_ATTACK_RANGE_Y = "RemoteAttackRangeY";
	public const string JUMP_ATTACK_RANGE = "JumpAttackRange";
	public const string SAFE_RANGE_X = "SafeRangeX";
	public const string SAFE_RANGE_Y = "SafeRangeY";
	public const string PATROL_DISTANCE = "PatrolDistance";
	public const string CENTER_OFFSET_X = "CenterOffsetX";
	public const string CENTER_OFFSET_Y = "CenterOffsetY";
	#endregion

	#region UI数字相关
	public const string UI_NUM_DAMAGE_NORMAL_PREFIX = "damage_normal_";
	public const string UI_NUM_DAMAGE_CRIT_PREFIX = "damage_crit_";
	public const string UI_NUM_DAMAGE_POISON_PREFIX = "damage_poison_";
	public const string UI_NUM_DAMAGE_BURNING_PREFIX = "damage_burning_";
	public const string UI_NUM_DAMAGE_SPECIAL_PREFIX = "damage_special_";
	public const string UI_NUM_DAMAGE_DUALBLADE_PREFIX = "damage_special_common_";
	public const string UI_NUM_LIFE_PREFIX = "life_";
	#endregion

	#region 匿名统计数据URL
	public const string URL_ROLE_SKIN_VOTE = "https://seedhunter.herokuapp.com/vote/send_role_skin_vote.json";
	public const string URL_ALTAR_REWARD_STATISTIC = "https://seedhunter.herokuapp.com/statistic/send_altar_reward_statistic.json";
	public const string URL_STEAM_USER_STATISTIC = "https://seedhunter.herokuapp.com/statistic/send_steam_user_statistic.json";
	#endregion
}
