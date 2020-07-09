using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
/// <summary>
/// 参考Cuphead全局游戏分层速度控制
/// </summary>
public static class CupheadTime
{
    public enum Layer
    {
        Default,
        Player,
        Enemy,
        UI
    }

    public class DeltaObject
    {
        public float this[Layer layer]
        {
            get
            {
                return Time.deltaTime * CupheadTime.GetLayerSpeed(layer) * CupheadTime.GlobalSpeed;
            }
        }

        public static implicit operator float(DeltaObject d)
        {
            return d[Layer.Default] * CupheadTime.GlobalSpeed;
        }
    }

    private static readonly DeltaObject delta;

    private static float globalSpeed;

    private static Dictionary<Layer, float> layers;

    public static DeltaObject Delta
    {
        get
        {
            return CupheadTime.delta;
        }
    }

    public static float GlobalDelta
    {
        get
        {
            return Time.deltaTime;
        }
    }

	public static float GlobalDeltaScaled {
		get {
			return Time.deltaTime * CupheadTime.globalSpeed;
		}
	}

	public static float FixedDelta
    {
        get
        {
            return Time.fixedDeltaTime;
        }
    }

	public static float FixedDeltaScaled {
		get {
			return Time.fixedDeltaTime * CupheadTime.globalSpeed;
		}
	}

	public static float GlobalSpeed
    {
        get
        {
            return CupheadTime.globalSpeed;
        }
        set
        {
            CupheadTime.globalSpeed = Mathf.Clamp(value, 0f, 1f);
            CupheadTime.OnChanged();
        }
    }

    private static event Action onChangedEvent;
    public static event Action OnChangedEvent
    {
        add
        {
            Action action = CupheadTime.onChangedEvent;
            Action action2;
            do
            {
                action2 = action;
                action = Interlocked.CompareExchange<Action>(ref CupheadTime.onChangedEvent, (Action)Delegate.Combine(action2, value), action);
            }
            while (action != action2);
        }
        remove
        {
            Action action = CupheadTime.onChangedEvent;
            Action action2;
            do
            {
                action2 = action;
                action = Interlocked.CompareExchange<Action>(ref CupheadTime.onChangedEvent, (Action)Delegate.Remove(action2, value), action);
            }
            while (action != action2);
        }
    }

    static CupheadTime()
    {
        CupheadTime.globalSpeed = 1f;
        CupheadTime.delta = new DeltaObject();
        CupheadTime.layers = new Dictionary<Layer, float>();
        Layer[] values = EnumUtils.GetValues<Layer>();
        Layer[] array = values;
        foreach (Layer key in array)
        {
            CupheadTime.layers.Add(key, 1f);
        }
    }

    public static float GetLayerSpeed(Layer layer)
    {
        return CupheadTime.layers[layer];
    }

    public static void SetLayerSpeed(Layer layer, float value)
    {
        CupheadTime.layers[layer] = value;
        CupheadTime.OnChanged();
    }

    public static void Reset()
    {
        CupheadTime.SetAll(1f);
    }

    public static void SetAll(float value)
    {
        CupheadTime.GlobalSpeed = value;
        Layer[] values = EnumUtils.GetValues<Layer>();
        foreach (Layer key in values)
        {
            CupheadTime.layers[key] = value;
        }
        CupheadTime.OnChanged();
    }

    private static void OnChanged()
    {
        if (CupheadTime.onChangedEvent != null)
        {
            CupheadTime.onChangedEvent();
        }
    }

    public static bool IsPaused()
    {
        return CupheadTime.GlobalSpeed <= 1E-05f || PauseManager.state == PauseManager.State.Paused;
    }

    public static Coroutine WaitForSeconds(MonoBehaviour m, float time)
    {
        return m.StartCoroutine(CupheadTime.waitForSeconds_cr(time, Layer.Default));
    }

    public static Coroutine WaitForSecondsIgnoreTimeLayer(MonoBehaviour m, float time, Action onCompleted = null, Action<float> onUpdate = null)
    {
        return m.StartCoroutine(waitForSecondsIgnoreTimeLayer_cr(time, onCompleted, onUpdate));
    }

	public static Coroutine WaitForSecondsUnscaledTime ( MonoBehaviour m, float time, Action onCompleted = null, Action<float> onUpdate = null ) {
		return m.StartCoroutine ( waitForSecondsUnscaledTime_cr ( time, onCompleted, onUpdate ) );
	}

	public static Coroutine WaitForSeconds(MonoBehaviour m, float time, Layer layer)
    {
        return m.StartCoroutine(CupheadTime.waitForSeconds_cr(time, layer));
    }

    public static Coroutine WaitForSeconds(MonoBehaviour m, float time, Layer layer, Action onCompleted)
    {
        return m.StartCoroutine(CupheadTime.waitForSeconds_cr(time, layer, onCompleted));
    }

    public static Coroutine WaitForSeconds(MonoBehaviour m, float time, Layer layer, Action<float> onUpdate)
    {
        return m.StartCoroutine(CupheadTime.waitForSeconds_cr(time, layer, null, onUpdate));
    }

    public static Coroutine WaitForSeconds(MonoBehaviour m, float time, Layer layer, Action onCompleted = null, Action<float> onUpdate = null)
    {
        return m.StartCoroutine(waitForSeconds_cr(time, layer, onCompleted, onUpdate));
    }

    private static IEnumerator waitForSeconds_cr(float time, Layer layer, Action onCompleted = null, Action<float> onUpdate = null)
    {
        float t = 0f;
        while (t < time)
        {
            t += CupheadTime.Delta[layer];
            onUpdate?.Invoke(t);
            yield return null;
        }
        onCompleted?.Invoke();
    }

    private static IEnumerator waitForSecondsIgnoreTimeLayer_cr(float time, Action onCompleted = null, Action<float> onUpdate = null)
    {
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            onUpdate?.Invoke(t);
            yield return null;
        }
        onCompleted?.Invoke();
    }

	private static IEnumerator waitForSecondsUnscaledTime_cr ( float time, Action onCompleted = null, Action<float> onUpdate = null ) {
		float t = 0f;
		while ( t < time ) {
			t += Time.unscaledDeltaTime;
			onUpdate?.Invoke ( t );
			yield return null;
		}
		onCompleted?.Invoke ();
	}

	public static Coroutine WaitForUnpause(MonoBehaviour m)
    {
        return m.StartCoroutine(CupheadTime.waitForUnpause_cr());
    }

    private static IEnumerator waitForUnpause_cr()
    {
        while (CupheadTime.GlobalSpeed == 0f)
        {
            yield return null;
        }
    }
}
