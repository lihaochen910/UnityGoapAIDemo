// EaseUtils
using UnityEngine;

public class EaseUtils
{
    public enum EaseType
    {
        easeInQuad,
        easeOutQuad,
        easeInOutQuad,
        easeInCubic,
        easeOutCubic,
        easeInOutCubic,
        easeInQuart,
        easeOutQuart,
        easeInOutQuart,
        easeInQuint,
        easeOutQuint,
        easeInOutQuint,
        easeInSine,
        easeOutSine,
        easeInOutSine,
        easeInExpo,
        easeOutExpo,
        easeInOutExpo,
        easeInCirc,
        easeOutCirc,
        easeInOutCirc,
        linear,
        spring,
        easeInBounce,
        easeOutBounce,
        easeInOutBounce,
        easeInBack,
        easeOutBack,
        easeInOutBack,
        easeInElastic,
        easeOutElastic,
        easeInOutElastic,
        punch
    }

    public static float EaseInOut(EaseType inEase, EaseType outEase, float start, float end, float value)
    {
        if (value < 0.5f)
        {
            float value2 = Mathf.Clamp(value * 2f, 0f, 1f);
            float end2 = Mathf.Lerp(start, end, 0.5f);
            return EaseUtils.Ease(inEase, start, end2, value2);
        }
        if (value > 0.5f)
        {
            float value2 = Mathf.Clamp(value * 2f - 1f, 0f, 1f);
            float start2 = Mathf.Lerp(start, end, 0.5f);
            return EaseUtils.Ease(outEase, start2, end, value2);
        }
        return Mathf.Lerp(start, end, 0.5f);
    }

    public static float Ease(EaseType ease, float start, float end, float value)
    {
        switch (ease)
        {
            case EaseType.easeInBack:
                return EaseUtils.EaseInBack(start, end, value);
            case EaseType.easeInBounce:
                return EaseUtils.EaseInBounce(start, end, value);
            case EaseType.easeInCirc:
                return EaseUtils.EaseInCirc(start, end, value);
            case EaseType.easeInCubic:
                return EaseUtils.EaseInCubic(start, end, value);
            case EaseType.easeInElastic:
                return EaseUtils.EaseInElastic(start, end, value);
            case EaseType.easeInExpo:
                return EaseUtils.EaseInExpo(start, end, value);
            case EaseType.easeInOutBack:
                return EaseUtils.EaseInOutBack(start, end, value);
            case EaseType.easeInOutBounce:
                return EaseUtils.EaseInOutBounce(start, end, value);
            case EaseType.easeInOutCirc:
                return EaseUtils.EaseInOutCirc(start, end, value);
            case EaseType.easeInOutCubic:
                return EaseUtils.EaseInOutCubic(start, end, value);
            case EaseType.easeInOutElastic:
                return EaseUtils.EaseInOutElastic(start, end, value);
            case EaseType.easeInOutExpo:
                return EaseUtils.EaseInOutExpo(start, end, value);
            case EaseType.easeInOutQuad:
                return EaseUtils.EaseInOutQuad(start, end, value);
            case EaseType.easeInOutQuart:
                return EaseUtils.EaseInOutQuart(start, end, value);
            case EaseType.easeInOutQuint:
                return EaseUtils.EaseInOutQuint(start, end, value);
            case EaseType.easeInOutSine:
                return EaseUtils.EaseInOutSine(start, end, value);
            case EaseType.easeInQuad:
                return EaseUtils.EaseInQuad(start, end, value);
            case EaseType.easeInQuart:
                return EaseUtils.EaseInQuart(start, end, value);
            case EaseType.easeInQuint:
                return EaseUtils.EaseInQuint(start, end, value);
            case EaseType.easeInSine:
                return EaseUtils.EaseInSine(start, end, value);
            case EaseType.easeOutBack:
                return EaseUtils.EaseOutBack(start, end, value);
            case EaseType.easeOutBounce:
                return EaseUtils.EaseOutBounce(start, end, value);
            case EaseType.easeOutCirc:
                return EaseUtils.EaseOutCirc(start, end, value);
            case EaseType.easeOutCubic:
                return EaseUtils.EaseOutCubic(start, end, value);
            case EaseType.easeOutElastic:
                return EaseUtils.EaseOutElastic(start, end, value);
            case EaseType.easeOutExpo:
                return EaseUtils.EaseOutExpo(start, end, value);
            case EaseType.easeOutQuad:
                return EaseUtils.EaseOutQuad(start, end, value);
            case EaseType.easeOutQuart:
                return EaseUtils.EaseOutQuart(start, end, value);
            case EaseType.easeOutQuint:
                return EaseUtils.EaseOutQuint(start, end, value);
            case EaseType.easeOutSine:
                return EaseUtils.EaseOutSine(start, end, value);
            case EaseType.spring:
                return EaseUtils.Spring(start, end, value);
            default:
                return Mathf.Lerp(start, end, value);
        }
    }

    public static float Linear(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value);
    }

    public static float Clerp(float start, float end, float value)
    {
        float num = 0f;
        float num2 = 360f;
        float num3 = Mathf.Abs((num2 - num) / 2f);
        float num4 = 0f;
        float num5 = 0f;
        if (end - start < 0f - num3)
        {
            num5 = (num2 - start + end) * value;
            return start + num5;
        }
        if (end - start > num3)
        {
            num5 = (0f - (num2 - end + start)) * value;
            return start + num5;
        }
        return start + (end - start) * value;
    }

    public static float Spring(float start, float end, float value)
    {
        value = Mathf.Clamp01(value);
        value = (Mathf.Sin(value * 3.14159274f * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + 1.2f * (1f - value));
        return start + (end - start) * value;
    }

    public static float EaseInQuad(float start, float end, float value)
    {
        end -= start;
        return end * value * value + start;
    }

    public static float EaseOutQuad(float start, float end, float value)
    {
        end -= start;
        return (0f - end) * value * (value - 2f) + start;
    }

    public static float EaseInOutQuad(float start, float end, float value)
    {
        value /= 0.5f;
        end -= start;
        if (value < 1f)
        {
            return end / 2f * value * value + start;
        }
        value -= 1f;
        return (0f - end) / 2f * (value * (value - 2f) - 1f) + start;
    }

    public static float EaseInCubic(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value + start;
    }

    public static float EaseOutCubic(float start, float end, float value)
    {
        value -= 1f;
        end -= start;
        return end * (value * value * value + 1f) + start;
    }

    public static float EaseInOutCubic(float start, float end, float value)
    {
        value /= 0.5f;
        end -= start;
        if (value < 1f)
        {
            return end / 2f * value * value * value + start;
        }
        value -= 2f;
        return end / 2f * (value * value * value + 2f) + start;
    }

    public static float EaseInQuart(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value + start;
    }

    public static float EaseOutQuart(float start, float end, float value)
    {
        value -= 1f;
        end -= start;
        return (0f - end) * (value * value * value * value - 1f) + start;
    }

    public static float EaseInOutQuart(float start, float end, float value)
    {
        value /= 0.5f;
        end -= start;
        if (value < 1f)
        {
            return end / 2f * value * value * value * value + start;
        }
        value -= 2f;
        return (0f - end) / 2f * (value * value * value * value - 2f) + start;
    }

    public static float EaseInQuint(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value * value + start;
    }

    public static float EaseOutQuint(float start, float end, float value)
    {
        value -= 1f;
        end -= start;
        return end * (value * value * value * value * value + 1f) + start;
    }

    public static float EaseInOutQuint(float start, float end, float value)
    {
        value /= 0.5f;
        end -= start;
        if (value < 1f)
        {
            return end / 2f * value * value * value * value * value + start;
        }
        value -= 2f;
        return end / 2f * (value * value * value * value * value + 2f) + start;
    }

    public static float EaseInSine(float start, float end, float value)
    {
        end -= start;
        return (0f - end) * Mathf.Cos(value / 1f * 1.57079637f) + end + start;
    }

    public static float EaseOutSine(float start, float end, float value)
    {
        end -= start;
        return end * Mathf.Sin(value / 1f * 1.57079637f) + start;
    }

    public static float EaseInOutSine(float start, float end, float value)
    {
        end -= start;
        return (0f - end) / 2f * (Mathf.Cos(3.14159274f * value / 1f) - 1f) + start;
    }

    public static float EaseInExpo(float start, float end, float value)
    {
        end -= start;
        return end * Mathf.Pow(2f, 10f * (value / 1f - 1f)) + start;
    }

    public static float EaseOutExpo(float start, float end, float value)
    {
        end -= start;
        return end * (0f - Mathf.Pow(2f, -10f * value / 1f) + 1f) + start;
    }

    public static float EaseInOutExpo(float start, float end, float value)
    {
        value /= 0.5f;
        end -= start;
        if (value < 1f)
        {
            return end / 2f * Mathf.Pow(2f, 10f * (value - 1f)) + start;
        }
        value -= 1f;
        return end / 2f * (0f - Mathf.Pow(2f, -10f * value) + 2f) + start;
    }

    public static float EaseInCirc(float start, float end, float value)
    {
        end -= start;
        return (0f - end) * (Mathf.Sqrt(1f - value * value) - 1f) + start;
    }

    public static float EaseOutCirc(float start, float end, float value)
    {
        value -= 1f;
        end -= start;
        return end * Mathf.Sqrt(1f - value * value) + start;
    }

    public static float EaseInOutCirc(float start, float end, float value)
    {
        value /= 0.5f;
        end -= start;
        if (value < 1f)
        {
            return (0f - end) / 2f * (Mathf.Sqrt(1f - value * value) - 1f) + start;
        }
        value -= 2f;
        return end / 2f * (Mathf.Sqrt(1f - value * value) + 1f) + start;
    }

    public static float EaseInBounce(float start, float end, float value)
    {
        end -= start;
        float num = 1f;
        return end - EaseUtils.EaseOutBounce(0f, end, num - value) + start;
    }

    public static float EaseOutBounce(float start, float end, float value)
    {
        value /= 1f;
        end -= start;
        if (value < 0.363636374f)
        {
            return end * (7.5625f * value * value) + start;
        }
        if (value < 0.727272749f)
        {
            value -= 0.545454562f;
            return end * (7.5625f * value * value + 0.75f) + start;
        }
        if ((double)value < 0.90909090909090906)
        {
            value -= 0.8181818f;
            return end * (7.5625f * value * value + 0.9375f) + start;
        }
        value -= 0.954545438f;
        return end * (7.5625f * value * value + 0.984375f) + start;
    }

    public static float EaseInOutBounce(float start, float end, float value)
    {
        end -= start;
        float num = 1f;
        if (value < num / 2f)
        {
            return EaseUtils.EaseInBounce(0f, end, value * 2f) * 0.5f + start;
        }
        return EaseUtils.EaseOutBounce(0f, end, value * 2f - num) * 0.5f + end * 0.5f + start;
    }

    public static float EaseInBack(float start, float end, float value)
    {
        end -= start;
        value /= 1f;
        float num = 1.70158f;
        return end * value * value * ((num + 1f) * value - num) + start;
    }

    public static float EaseOutBack(float start, float end, float value)
    {
        float num = 1.70158f;
        end -= start;
        value = value / 1f - 1f;
        return end * (value * value * ((num + 1f) * value + num) + 1f) + start;
    }

    public static float EaseInOutBack(float start, float end, float value)
    {
        float num = 1.70158f;
        end -= start;
        value /= 0.5f;
        if (value < 1f)
        {
            num *= 1.525f;
            return end / 2f * (value * value * ((num + 1f) * value - num)) + start;
        }
        value -= 2f;
        num *= 1.525f;
        return end / 2f * (value * value * ((num + 1f) * value + num) + 2f) + start;
    }

    public static float Punch(float amplitude, float value)
    {
        float num = 9f;
        if (value == 0f)
        {
            return 0f;
        }
        if (value == 1f)
        {
            return 0f;
        }
        float num2 = 0.3f;
        num = num2 / 6.28318548f * Mathf.Asin(0f);
        return amplitude * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * 1f - num) * 6.28318548f / num2);
    }

    public static float EaseInElastic(float start, float end, float value)
    {
        end -= start;
        float num = 1f;
        float num2 = num * 0.3f;
        float num3 = 0f;
        float num4 = 0f;
        if (value == 0f)
        {
            return start;
        }
        if ((value /= num) == 1f)
        {
            return start + end;
        }
        if (num4 == 0f || num4 < Mathf.Abs(end))
        {
            num4 = end;
            num3 = num2 / 4f;
        }
        else
        {
            num3 = num2 / 6.28318548f * Mathf.Asin(end / num4);
        }
        return 0f - num4 * Mathf.Pow(2f, 10f * (value -= 1f)) * Mathf.Sin((value * num - num3) * 6.28318548f / num2) + start;
    }

    public static float EaseOutElastic(float start, float end, float value)
    {
        end -= start;
        float num = 1f;
        float num2 = num * 0.3f;
        float num3 = 0f;
        float num4 = 0f;
        if (value == 0f)
        {
            return start;
        }
        if ((value /= num) == 1f)
        {
            return start + end;
        }
        if (num4 == 0f || num4 < Mathf.Abs(end))
        {
            num4 = end;
            num3 = num2 / 4f;
        }
        else
        {
            num3 = num2 / 6.28318548f * Mathf.Asin(end / num4);
        }
        return num4 * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * num - num3) * 6.28318548f / num2) + end + start;
    }

    public static float EaseInOutElastic(float start, float end, float value)
    {
        end -= start;
        float num = 1f;
        float num2 = num * 0.3f;
        float num3 = 0f;
        float num4 = 0f;
        if (value == 0f)
        {
            return start;
        }
        if ((value /= num / 2f) == 2f)
        {
            return start + end;
        }
        if (num4 == 0f || num4 < Mathf.Abs(end))
        {
            num4 = end;
            num3 = num2 / 4f;
        }
        else
        {
            num3 = num2 / 6.28318548f * Mathf.Asin(end / num4);
        }
        if (value < 1f)
        {
            return -0.5f * (num4 * Mathf.Pow(2f, 10f * (value -= 1f)) * Mathf.Sin((value * num - num3) * 6.28318548f / num2)) + start;
        }
        return num4 * Mathf.Pow(2f, -10f * (value -= 1f)) * Mathf.Sin((value * num - num3) * 6.28318548f / num2) * 0.5f + end + start;
    }
}
