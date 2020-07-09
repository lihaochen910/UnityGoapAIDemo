using System.Collections;
using UnityEngine;

public class PlatformingLevelPlatformSag : LevelPlatform
{
    [SerializeField]
    private float sagAmount = 30f;

    private const EaseUtils.EaseType FALL_BOUNCE_EASE = EaseUtils.EaseType.easeOutBounce;

    public const float FALL_TIME = 0.4f;

    public const float RISE_TIME = 0.6f;

    private float localPosY;

    protected override void Start()
    {
        base.Start();
        Vector3 localPosition = base.transform.localPosition;
        this.localPosY = localPosition.y;
    }

    public override void AddChild(Transform player)
    {
        base.AddChild(player);
        base.StartCoroutine(this.fall_cr());
    }

    public override void OnPlayerExit(Transform player)
    {
        base.OnPlayerExit(player);
        base.StartCoroutine(this.go_up_cr());
    }

    private IEnumerator goTo_cr(float start, float end, float time, EaseUtils.EaseType ease)
    {
        float t = 0f;
        base.transform.SetLocalPosition(null, start, null);
        while (t < time)
        {
            float val = t / time;
            base.transform.SetLocalPosition(null, EaseUtils.Ease(ease, start, end, val), null);
            t += CupheadTime.GlobalDelta;
            yield return StartCoroutine(base.WaitForPause_CR());
        }
        base.transform.SetLocalPosition(null, end, null);
    }

    private IEnumerator fall_cr()
    {
        Vector3 localPosition = transform.localPosition;
        yield return StartCoroutine(this.goTo_cr(localPosition.y, this.localPosY - this.sagAmount, 0.4f, EaseUtils.EaseType.easeOutBounce));
    }

    private IEnumerator go_up_cr()
    {
        Vector3 localPosition = base.transform.localPosition;
        yield return StartCoroutine(this.goTo_cr(localPosition.y, this.localPosY, 0.6f, EaseUtils.EaseType.easeOutBounce));
    }
}
