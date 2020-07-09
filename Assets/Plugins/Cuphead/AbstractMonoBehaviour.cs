// AbstractMonoBehaviour
using System;
using System.Collections;
using UnityEngine;
/// <summary>
/// 参考自Cuphead的MonoBehaviour抽象类
/// </summary>
public abstract class AbstractMonoBehaviour : MonoBehaviour
{
    public delegate void TweenUpdateHandler(float value);
    public delegate void TweenCompletedHandler();

    private Transform _transform;

    private RectTransform _rectTransform;

    protected bool ignoreGlobalTime;

    protected CupheadTime.Layer timeLayer;

    public Transform baseTransform
    {
        get
        {
            return base.transform;
        }
    }

    public new Transform transform
    {
        get
        {
            if (this._transform == null)
            {
                this._transform = this.baseTransform;
            }
            return this._transform;
        }
    }

    public RectTransform baseRectTransform
    {
        get
        {
            return base.transform as RectTransform;
        }
    }

    public RectTransform rectTransform
    {
        get
        {
            if (this._rectTransform == null)
            {
                this._rectTransform = this.baseRectTransform;
            }
            return this._rectTransform;
        }
    }

    protected float LocalDeltaTime
    {
        get
        {
            if (this.ignoreGlobalTime)
            {
                return Time.deltaTime;
            }
            return CupheadTime.Delta[this.timeLayer];
        }
    }

    protected virtual void Awake()
    {
        base.useGUILayout = false;
    }

    protected virtual void Start() { }

    protected virtual void Reset() { }

    protected virtual void OnEnable() { }

    protected virtual void OnDisable() { }

    protected virtual void OnDestroy() { }

    protected virtual void OnValidate() { }

    protected virtual void Update() { }

    protected virtual void FixedUpdate() { }

    protected virtual void LateUpdate() { }

    protected virtual void OnMouseDown() { }

    protected virtual void OnMouseDrag() { }

    protected virtual void OnMouseEnter() { }

    protected virtual void OnMouseExit() { }

    protected virtual void OnMouseOver() { }

    protected virtual void OnMouseUp() { }

    protected virtual void OnMouseUpAsButton() { }

    protected virtual void OnCollisionEnter(Collision collision) { }

    protected virtual void OnCollisionExit(Collision collision) { }

    protected virtual void OnCollisionStay(Collision collision) { }

    protected virtual void OnTriggerEnter(Collider collider) { }

    protected virtual void OnTriggerExit(Collider collider) { }

    protected virtual void OnTriggerStay(Collider collider) { }

    protected virtual void OnCollisionEnter2D(Collision2D collision) { }

    protected virtual void OnCollisionExit2D(Collision2D collision) { }

    protected virtual void OnCollisionStay2D(Collision2D collision) { }

    protected virtual void OnTriggerEnter2D(Collider2D collider) { }

    protected virtual void OnTriggerExit2D(Collider2D collider) { }

    protected virtual void OnTriggerStay2D(Collider2D collider) { }

    protected virtual void OnApplicationFocus(bool focusStatus) { }

    protected virtual void OnApplicationPause(bool pauseStatus) { }

    protected virtual void OnApplicationQuit() { }

    protected virtual void OnWillRenderObject() { }

    protected virtual void OnPostRender() { }

    protected virtual void OnPreRender() { }

    protected virtual void OnRenderObject() { }

    protected virtual void OnDrawGizmos() { }

    protected virtual void OnDrawGizmosSelected() { }

	#region 对象池接口(可选)
	[HideInInspector] public bool SetActive = true;

	/// <summary>
	/// 对象初始化时调用
	/// </summary>
	public virtual void OnCreate () { }

	/// <summary>
	/// 从对象池中取出时调用
	/// </summary>
	public virtual void OnFetch () { }

	/// <summary>
	/// 被回收时调用
	/// </summary>
	public virtual void OnRecycle () { }

	/// <summary>
	/// 被销毁时调用
	/// </summary>
	public virtual void OnObjectDestroy () { }
	#endregion

	public virtual T InstantiatePrefab<T>() where T : MonoBehaviour
    {
        GameObject gameObject = UnityEngine.Object.Instantiate(base.gameObject);
        gameObject.name = gameObject.name.Replace("(Clone)", string.Empty);
        return gameObject.GetComponent<T>();
    }

    public Coroutine FrameDelayedCallback(Action callback, int frames)
    {
        return base.StartCoroutine(this.frameDelayedCallback_cr(callback, frames));
    }

    public IEnumerator frameDelayedCallback_cr(Action callback, int frames)
    {
        int i = 0;
        while (i++ < frames)
        {
            yield return null;
        }
        callback?.Invoke();
    }

    public Coroutine TweenValue(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenValue_cr(start, end, time, ease, updateCallback));
    }

    protected IEnumerator tweenValue_cr(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
    {
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            updateCallback?.Invoke(EaseUtils.Ease(ease, start, end, val));
            t += this.LocalDeltaTime;
            yield return null;
        }
        updateCallback?.Invoke(end);
        yield return null;
    }

    static public IEnumerator TweenValue_cr(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback, TweenCompletedHandler endCallback = null)
    {
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            updateCallback?.Invoke(EaseUtils.Ease(ease, start, end, val));
            t += CupheadTime.GlobalDelta;
            yield return null;
        }
        updateCallback?.Invoke(end);
        endCallback?.Invoke();
        yield return null;
    }

    static public IEnumerator TweenSpriteRendererColor_cr(SpriteRenderer renderer, Color start, Color end, float time, EaseUtils.EaseType ease, CupheadTime.Layer timeLayer, TweenCompletedHandler endCallback = null)
    {
        float t = 0f;
        Color c = start;
        while (t < time)
        {
            float val = t / time;

            c.r = EaseUtils.Ease(ease, start.r, end.r, val);
            c.g = EaseUtils.Ease(ease, start.g, end.g, val);
            c.b = EaseUtils.Ease(ease, start.b, end.b, val);
            c.a = EaseUtils.Ease(ease, start.a, end.a, val);

            renderer.color = c;

            t += CupheadTime.Delta[timeLayer];
            yield return null;
        }
        endCallback?.Invoke();
        yield return null;
    }

	public Coroutine TweenScale(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenScale_cr(start, end, time, ease, null));
    }

    public Coroutine TweenScale(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenScale_cr(start, end, time, ease, updateCallback));
    }

    private IEnumerator tweenScale_cr(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
    {
        this.transform.SetScale(start.x, start.y, null);
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            float x = EaseUtils.Ease(ease, start.x, end.x, val);
            float y = EaseUtils.Ease(ease, start.y, end.y, val);
            this.transform.SetScale(x, y, null);
            updateCallback?.Invoke(val);
            t += this.LocalDeltaTime;
            yield return null;
        }
        this.transform.SetScale(end.x, end.y, null);
        updateCallback?.Invoke(1f);
        yield return null;
    }

    static public IEnumerator TweenScale_cr(Transform transform, Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, CupheadTime.Layer timeLayer, TweenCompletedHandler endCallback = null)
    {
        transform.SetScale(start.x, start.y, null);
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            float x = EaseUtils.Ease(ease, start.x, end.x, val);
            float y = EaseUtils.Ease(ease, start.y, end.y, val);
            transform.SetScale(x, y, null);
            t += CupheadTime.Delta[timeLayer];
            yield return null;
        }
        transform.SetScale(end.x, end.y, null);
        endCallback?.Invoke();
        yield return null;
    }

    public Coroutine TweenPosition(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenPosition_cr(start, end, time, ease, null));
    }

    public Coroutine TweenPosition(Vector3 start, Vector3 end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenPosition_cr(start, end, time, ease, null));
    }

    public Coroutine TweenPosition(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenPosition_cr(start, end, time, ease, updateCallback));
    }

    private IEnumerator tweenPosition_cr(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
    {
        this.transform.position = start;
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            float x = EaseUtils.Ease(ease, start.x, end.x, val);
            float y = EaseUtils.Ease(ease, start.y, end.y, val);
            this.transform.SetPosition(x, y, 0f);
            updateCallback?.Invoke(val);
            t += this.LocalDeltaTime;
            yield return null;
        }
        this.transform.position = end;
        updateCallback?.Invoke(1f);
        yield return null;
    }

    private IEnumerator tweenPosition_cr(Vector3 start, Vector3 end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
    {
        this.transform.position = start;
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            float x = EaseUtils.Ease(ease, start.x, end.x, val);
            float y = EaseUtils.Ease(ease, start.y, end.y, val);
            float z = EaseUtils.Ease(ease, start.z, end.z, val);
            this.transform.SetPosition(x, y, z);
            updateCallback?.Invoke(val);
            t += this.LocalDeltaTime;
            yield return null;
        }
        this.transform.position = end;
        updateCallback?.Invoke(1f);
        yield return null;
    }

    static public IEnumerator TweenPosition_cr(Transform target, Vector3 start, Vector3 end, float time, EaseUtils.EaseType ease)
    {
        target.position = start;
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            float x = EaseUtils.Ease(ease, start.x, end.x, val);
            float y = EaseUtils.Ease(ease, start.y, end.y, val);
            float z = EaseUtils.Ease(ease, start.z, end.z, val);
            target.SetPosition(x, y, z);
            t += CupheadTime.GlobalDelta;
            yield return null;
        }
        target.position = end;
        yield return null;
    }

    public Coroutine TweenLocalPosition(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenLocalPosition_cr(start, end, time, ease, null));
    }

    public Coroutine TweenLocalPosition(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenLocalPosition_cr(start, end, time, ease, updateCallback));
    }

    private IEnumerator tweenLocalPosition_cr(Vector2 start, Vector2 end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
    {
        this.transform.localPosition = start;
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            float x = EaseUtils.Ease(ease, start.x, end.x, val);
            float y = EaseUtils.Ease(ease, start.y, end.y, val);
            this.transform.SetLocalPosition(x, y, 0f);
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            float num = t + this.LocalDeltaTime;
            yield return null;
        }
        this.transform.localPosition = end;
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        yield return null;
    }

	static public IEnumerator TweenLocalPosition_cr (Transform target, Vector3 start, Vector3 end, float time, EaseUtils.EaseType ease, CupheadTime.Layer timeLayer, TweenUpdateHandler updateCallback, TweenCompletedHandler endCallback = null) {
		target.localPosition = start;
		float t = 0f;
		while ( t < time ) {
			float val = t / time;
			float x = EaseUtils.Ease ( ease, start.x, end.x, val );
			float y = EaseUtils.Ease ( ease, start.y, end.y, val );
			float z = EaseUtils.Ease ( ease, start.z, end.z, val );
			target.SetLocalPosition ( x, y, z );
			updateCallback?.Invoke ( val );
			t += CupheadTime.Delta[timeLayer];
			yield return null;
		}
		target.localPosition = end;
		updateCallback?.Invoke ( 1f );
		endCallback?.Invoke ();
		yield return null;
	}

	public Coroutine TweenPositionX(float start, float end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenPositionX_cr(start, end, time, ease, null));
    }

    public Coroutine TweenPositionX(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenPositionX_cr(start, end, time, ease, updateCallback));
    }

    private IEnumerator tweenPositionX_cr(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
    {
        this.transform.SetPosition(start, null, null);
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            this.transform.SetPosition(EaseUtils.Ease(ease, start, end, val), null, null);
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            float num = t + this.LocalDeltaTime;
            yield return null;
        }
        this.transform.SetPosition(end, null, null);
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        yield return null;
        /*Error: Unable to find new state assignment for yield return*/
        ;
    }

    public Coroutine TweenLocalPositionX(float start, float end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenLocalPositionX_cr(start, end, time, ease, null));
    }

    public Coroutine TweenLocalPositionX(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenLocalPositionX_cr(start, end, time, ease, updateCallback));
    }

    private IEnumerator tweenLocalPositionX_cr(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
    {
        this.transform.SetLocalPosition(start, null, null);
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            this.transform.SetLocalPosition(EaseUtils.Ease(ease, start, end, val), null, null);
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            float num = t + this.LocalDeltaTime;
            yield return null;
        }
        this.transform.SetLocalPosition(end, null, null);
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        yield return null;
    }

    public Coroutine TweenPositionY(float start, float end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenPositionY_cr(start, end, time, ease, null));
    }

    public Coroutine TweenPositionY(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenPositionY_cr(start, end, time, ease, updateCallback));
    }

    private IEnumerator tweenPositionY_cr(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
    {
        this.transform.SetPosition(null, start, null);
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            this.transform.SetPosition(null, EaseUtils.Ease(ease, start, end, val), null);
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            float num = t + this.LocalDeltaTime;
            yield return null;
        }
        this.transform.SetPosition(null, end, null);
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        yield return null;
    }

    public Coroutine TweenLocalPositionY(float start, float end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenLocalPositionY_cr(start, end, time, ease, null));
    }

    public Coroutine TweenLocalPositionY(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenLocalPositionY_cr(start, end, time, ease, updateCallback));
    }

    private IEnumerator tweenLocalPositionY_cr(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
    {
        this.transform.SetLocalPosition(null, start, null);
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            this.transform.SetLocalPosition(null, EaseUtils.Ease(ease, start, end, val), null);
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            float num = t + this.LocalDeltaTime;
            yield return null;
        }
        this.transform.SetLocalPosition(null, end, null);
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        yield return null;
        /*Error: Unable to find new state assignment for yield return*/
        ;
    }

    public Coroutine TweenPositionZ(float start, float end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenPositionZ_cr(start, end, time, ease, null));
    }

    public Coroutine TweenPositionZ(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenPositionZ_cr(start, end, time, ease, updateCallback));
    }

    private IEnumerator tweenPositionZ_cr(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
    {
        this.transform.SetPosition(null, null, start);
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            this.transform.SetPosition(null, null, EaseUtils.Ease(ease, start, end, val));
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            float num = t + this.LocalDeltaTime;
            yield return null;
        }
        this.transform.SetPosition(null, null, end);
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        yield return null;
    }

    public Coroutine TweenLocalPositionZ(float start, float end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenLocalPositionZ_cr(start, end, time, ease, null));
    }

    public Coroutine TweenLocalPositionZ(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenLocalPositionZ_cr(start, end, time, ease, updateCallback));
    }

    private IEnumerator tweenLocalPositionZ_cr(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
    {
        this.transform.SetLocalPosition(null, null, start);
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            this.transform.SetLocalPosition(null, null, EaseUtils.Ease(ease, start, end, val));
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            float num = t + this.LocalDeltaTime;
            yield return null;
        }
        this.transform.SetLocalPosition(null, null, end);
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        yield return null;
    }

    public Coroutine TweenRotation2D(float start, float end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenRotation2D_cr(start, end, time, ease, null));
    }

    public Coroutine TweenRotation2D(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenRotation2D_cr(start, end, time, ease, updateCallback));
    }

    private IEnumerator tweenRotation2D_cr(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
    {
        this.transform.SetEulerAngles(null, null, start);
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            this.transform.SetEulerAngles(null, null, EaseUtils.Ease(ease, start, end, val));
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            float num = t + this.LocalDeltaTime;
            yield return null;
        }
        this.transform.SetEulerAngles(null, null, end);
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        yield return null;
    }

    public Coroutine TweenLocalRotation2D(float start, float end, float time, EaseUtils.EaseType ease)
    {
        return base.StartCoroutine(this.tweenLocalRotation2D_cr(start, end, time, ease, null));
    }

    public Coroutine TweenLocalRotation2D(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback)
    {
        return base.StartCoroutine(this.tweenLocalRotation2D_cr(start, end, time, ease, updateCallback));
    }

    private IEnumerator tweenLocalRotation2D_cr(float start, float end, float time, EaseUtils.EaseType ease, TweenUpdateHandler updateCallback = null)
    {
        this.transform.SetLocalEulerAngles(null, null, start);
        float t = 0f;
        while (t < time)
        {
            float val = t / time;
            this.transform.SetLocalEulerAngles(null, null, EaseUtils.Ease(ease, start, end, val));
            if (updateCallback != null)
            {
                updateCallback(val);
            }
            float num = t + this.LocalDeltaTime;
            yield return null;
        }
        this.transform.SetLocalEulerAngles(null, null, end);
        if (updateCallback != null)
        {
            updateCallback(1f);
        }
        yield return null;
    }

    public new virtual void StopAllCoroutines()
    {
        base.StopAllCoroutines();
    }
}
