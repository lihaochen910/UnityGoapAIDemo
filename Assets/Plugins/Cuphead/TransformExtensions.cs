// TransformExtensions
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static void ResetScale(this Transform transform)
    {
        transform.localScale = Vector3.one;
    }

    public static void ResetPosition(this Transform transform)
    {
        transform.position = Vector3.zero;
    }

    public static void ResetLocalPosition(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
    }

    public static void ResetRotation(this Transform transform)
    {
        transform.eulerAngles = Vector3.zero;
    }

    public static void ResetLocalRotation(this Transform transform)
    {
        transform.localEulerAngles = Vector3.zero;
    }

    public static void ResetTransforms(this Transform transform)
    {
        transform.ResetPosition();
        transform.ResetRotation();
        transform.ResetScale();
    }

    public static void ResetLocalTransforms(this Transform transform)
    {
        transform.ResetLocalPosition();
        transform.ResetLocalRotation();
        transform.ResetScale();
    }

    public static void SetPosition(this Transform transform, float? x = default(float?), float? y = default(float?), float? z = default(float?))
    {
        Vector3 position = transform.position;
        if (x.HasValue)
        {
            position.x = x.Value;
        }
        if (y.HasValue)
        {
            position.y = y.Value;
        }
        if (z.HasValue)
        {
            position.z = z.Value;
        }
        transform.position = position;
    }

    public static void SetLocalPosition(this Transform transform, float? x = default(float?), float? y = default(float?), float? z = default(float?))
    {
        Vector3 localPosition = transform.localPosition;
        if (x.HasValue)
        {
            localPosition.x = x.Value;
        }
        if (y.HasValue)
        {
            localPosition.y = y.Value;
        }
        if (z.HasValue)
        {
            localPosition.z = z.Value;
        }
        transform.localPosition = localPosition;
    }

    public static void SetEulerAngles(this Transform transform, float? x = default(float?), float? y = default(float?), float? z = default(float?))
    {
        Vector3 eulerAngles = transform.eulerAngles;
        if (x.HasValue)
        {
            eulerAngles.x = x.Value;
        }
        if (y.HasValue)
        {
            eulerAngles.y = y.Value;
        }
        if (z.HasValue)
        {
            eulerAngles.z = z.Value;
        }
        transform.eulerAngles = eulerAngles;
    }

    public static void SetLocalEulerAngles(this Transform transform, float? x = default(float?), float? y = default(float?), float? z = default(float?))
    {
        Vector3 localEulerAngles = transform.localEulerAngles;
        if (x.HasValue)
        {
            localEulerAngles.x = x.Value;
        }
        if (y.HasValue)
        {
            localEulerAngles.y = y.Value;
        }
        if (z.HasValue)
        {
            localEulerAngles.z = z.Value;
        }
        transform.localEulerAngles = localEulerAngles;
    }

    public static void SetScale(this Transform transform, float? x = default(float?), float? y = default(float?), float? z = default(float?))
    {
        Vector3 localScale = transform.localScale;
        if (x.HasValue)
        {
            localScale.x = x.Value;
        }
        if (y.HasValue)
        {
            localScale.y = y.Value;
        }
        if (z.HasValue)
        {
            localScale.z = z.Value;
        }
        transform.localScale = localScale;
    }

    public static void AddPosition(this Transform transform, float x = 0f, float y = 0f, float z = 0f)
    {
        Vector3 position = transform.position;
        position.x += x;
        position.y += y;
        position.z += z;
        transform.position = position;
    }

    public static void AddLocalPosition(this Transform transform, float x = 0f, float y = 0f, float z = 0f)
    {
        Vector3 localPosition = transform.localPosition;
        localPosition.x += x;
        localPosition.y += y;
        localPosition.z += z;
        transform.localPosition = localPosition;
    }

    public static void AddPositionForward2D(this Transform transform, float forward)
    {
        transform.position += transform.right * forward;
    }

    public static void AddEulerAngles(this Transform transform, float x = 0f, float y = 0f, float z = 0f)
    {
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.x += x;
        eulerAngles.y += y;
        eulerAngles.z += z;
        transform.eulerAngles = eulerAngles;
    }

    public static void AddLocalEulerAngles(this Transform transform, float x = 0f, float y = 0f, float z = 0f)
    {
        Vector3 localEulerAngles = transform.localEulerAngles;
        localEulerAngles.x += x;
        localEulerAngles.y += y;
        localEulerAngles.z += z;
        transform.localEulerAngles = localEulerAngles;
    }

    public static void AddScale(this Transform transform, float x = 0f, float y = 0f, float z = 0f)
    {
        Vector3 localScale = transform.localScale;
        localScale.x += x;
        localScale.y += y;
        localScale.z += z;
        transform.localScale = localScale;
    }

    public static void MoveForward(this Transform transform, float amount)
    {
        transform.position += transform.forward * amount;
    }

    public static void MoveForward2D(this Transform transform, float amount)
    {
        transform.position += transform.right * amount;
    }

    public static void LookAt2D(this Transform transform, Transform target)
    {
        transform.LookAt2D(target.position);
    }

    public static void LookAt2D(this Transform transform, Vector3 target)
    {
        Vector3 vector = target - transform.position;
        vector.Normalize();
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(vector.y, vector.x) * 57.29578f);
    }

    public static Transform[] GetChildTransforms(this Transform transform)
    {
        List<Transform> list = new List<Transform>();
        IEnumerator enumerator = transform.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Transform item = (Transform)enumerator.Current;
                list.Add(item);
            }
        }
        finally
        {
            IDisposable disposable;
            if ((disposable = (enumerator as IDisposable)) != null)
            {
                disposable.Dispose();
            }
        }
        list.Remove(transform);
        return list.ToArray();
    }

    public static string GetPath(this Transform current)
    {
        if (current.parent == null)
            return "/" + current.name;
        return current.parent.GetPath() + "/" + current.name;
    }

    public static int IndexOf(this Transform transform, Transform child)
    {
        if (child == null || transform.childCount == 0)
            return -1;

        int index = 0;
        for(;index < transform.childCount; index++)
        {
            if (transform.GetChild(index) == child)
                return index;
        }

        return -1;
    }

    public static int ActiveIndexOf ( this Transform transform, Transform child ) {
        
        if ( child == null || transform.childCount == 0 )
            return -1;

        int index = 0;
        foreach ( Transform t in transform ) {
            if ( t == child ) {
                return index;
            }

            if ( t.gameObject.activeSelf ) {
                index++;
            }
        }
        
        return -1;
    }

    private static Quaternion opposit = Quaternion.Euler ( 0f, 0f, 180f );
    public static void SetDirection2D ( this Transform t, Vector3 direction ) {
        
        if ( t.right == -direction ) {
            t.rotation *= TransformExtensions.opposit;
            return;
        }

        t.right = direction;
    }

    /// <summary>
    /// Play mode only!
    /// </summary>
    public static bool IsPrefab(this Transform This)
    {
        if (Application.isEditor && !Application.isPlaying) throw new InvalidOperationException("MethodIsOnlyAllowedInPlayMode:IsPrefab()");

        return This.gameObject.scene.buildIndex < 0;
    }
}
