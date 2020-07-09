using System.Collections.Generic;
// using RSG;

/// <summary>
/// 全局时间回调管理器，时间层引用CupheadTime，
/// 执行时间序列任务的实现依赖RSG.Promise类库
/// </summary>
public class TimeManager : SingletonBehaviour<TimeManager>
{
    /// <summary>
    /// 引擎计时器
    /// </summary>
    // public static PromiseTimer GlobalPromiseTimer { get; private set; }

    public static float DefaultTime { get; private set; }
    public static float PlayerTime { get; private set; }
    public static float EnemyTime { get; private set; }
    public static float UITime { get; private set; }

    /// <summary>
    /// 层计时器列表
    /// </summary>
    // private static Dictionary<CupheadTime.Layer, PromiseTimer> _internalPromiseTimer;

    protected override void Awake()
    {
        // Persistent = true;
        //
        // base.Awake();
        //
        // GlobalPromiseTimer = new PromiseTimer();
        //
        // _internalPromiseTimer = new Dictionary<CupheadTime.Layer, PromiseTimer>();

        // CupheadTime.Layer[] values = EnumUtils.GetValues<CupheadTime.Layer>();
        //
        // foreach (var layer in values)
        // {
        //     _internalPromiseTimer.Add(layer, new PromiseTimer());
        // }
    }

    private void Update()
    {
        // GlobalPromiseTimer.Update(CupheadTime.GlobalDelta);
        //
        // foreach (var kv in _internalPromiseTimer)
        // {
        //     _internalPromiseTimer[kv.Key].Update(CupheadTime.Delta[kv.Key]);
        // }

        DefaultTime += CupheadTime.Delta [ CupheadTime.Layer.Default ];
        PlayerTime += CupheadTime.Delta [ CupheadTime.Layer.Player ];
        EnemyTime += CupheadTime.Delta [ CupheadTime.Layer.Enemy ];
        UITime += CupheadTime.Delta [ CupheadTime.Layer.UI ];
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        //var pTimer = GetPromiseTimer(CupheadTime.Layer.Player);

        //GUI.Label(new Rect(0, 100f, 300, 100), $"{pTimer.curFrame} {pTimer.curTime}");

        //GUI.Label(new Rect(0, 120f, 300, 100), $"Waiting List Count:{pTimer.waiting.Count}");

        //int i = 0;
        //foreach (var promise in pTimer.waiting)
        //{
        //    GUI.Label(new Rect(0, 140f + i * 20f, 300, 100), $"{promise.pendingPromise.Id} {promise.timeStarted}");
        //    i++;
        //}
    }
#endif

    // public static PromiseTimer GetPromiseTimer(CupheadTime.Layer layer)
    // {
    //     return _internalPromiseTimer[layer];
    // }
    //
    //
    // /// <summary>
    // /// 指定时间后执行回调
    // /// </summary>
    // /// <param name="t"></param>
    // /// <param name="layer">时间层</param>
    // /// <param name="onTimeUp"></param>
    // /// <returns>RSG.IPromise序列</returns>
    // public static IPromise WaitForSeconds(float t, CupheadTime.Layer layer, System.Action onTimeUp)
    // {
    //     return GetPromiseTimer(layer).WaitFor(t)
    //         .Then(() => onTimeUp?.Invoke());
    // }
    //
    //
    // /// <summary>
    // /// 指定时间后执行回调(忽略时间层)
    // /// </summary>
    // /// <param name="t"></param>
    // /// <param name="layer">时间层</param>
    // /// <param name="onTimeUp"></param>
    // /// <returns>RSG.IPromise序列</returns>
    // public static IPromise WaitForSecondsIgnoreTimeLayer(float t, System.Action onTimeUp)
    // {
    //     return GlobalPromiseTimer.WaitFor(t)
    //         .Then(() => onTimeUp?.Invoke());
    // }
    //
    //
    // /// <summary>
    // /// 指定玩家层时间后执行回调
    // /// </summary>
    // /// <param name="t"></param>
    // /// <param name="layer">时间层</param>
    // /// <param name="onTimeUp"></param>
    // /// <returns>RSG.IPromise序列</returns>
    // public static IPromise WaitForSeconds_Player(float t, System.Action onTimeUp)
    // {
    //     return WaitForSeconds(t, CupheadTime.Layer.Player, onTimeUp);
    // }
    //
    // /// <summary>
    // /// 指定怪物层时间后执行回调
    // /// </summary>
    // /// <param name="t"></param>
    // /// <param name="layer">时间层</param>
    // /// <param name="onTimeUp"></param>
    // /// <returns>RSG.IPromise序列</returns>
    // public static IPromise WaitForSeconds_Enemy(float t, System.Action onTimeUp)
    // {
    //     return WaitForSeconds(t, CupheadTime.Layer.Enemy, onTimeUp);
    // }
}
