using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class RangedFloat
{
    public RangedFloat(float min = float.NegativeInfinity, float max = float.PositiveInfinity)
    {
        this.min = min;
        this.max = max;
    }
    public float min;
    public float max;

    public float difference => max -  min;

    public float length => max - min;
    public float GetPercent(float value)
    {
        return (value - min) / length;
    }

    public bool InRange(float value)
    {
        return value >= min && value <= max;
    }
    public float GetRandom()
    {
        return Random.Range(min, max);
    }
    public float GetRandomSigned()
    {
        Debug.Assert(min >= 0);
        Debug.Assert(max >= 0);
        return Random.Range(min, max) * (Random.value > 0.5f ? 1 : -1);
    }

    public static bool InRange(float value, float min, float max)
    {
        return value >= min && value <= max;
    }
    public float Clamp(float v)
    {
        return Mathf.Clamp(v, min, max);
    }
}

[Serializable]
public class RangedInt
{
    public RangedInt(int min = int.MinValue, int max = int.MaxValue)
    {
        this.min = min;
        this.max = max;
    }
    public int min;
    public int max;

    public int length => max - min;

    public bool InRange(int value)
    {
        return value >= min && value < max;
    }
    public int GetRandom()
    {
        return Random.Range(min, max);
    }
    public int GetRandomSigned()
    {
        return Random.Range(min, max) * (Random.value > 0.5f ? 1 : -1);
    }


    public static bool InRange(int value, int min, int max)
    {
        return value >= min && value < max;
    }
    public int Clamp(int v)
    {
        return Mathf.Clamp(v, min, max);
    }
    public RangedInt GetClamped(int min, int max)
    {
        return new RangedInt(
            Mathf.Clamp(this.min, min, max),
            Mathf.Clamp(this.max, min, max)
        );
    }
    public RangedInt GetClamped(RangedInt range)
    {
        return new RangedInt(
            range.Clamp(min),
            range.Clamp(max)
        );
    }
}

public class BoxValue<T>
{
    public BoxValue(T value) { this.value = value; }
    public T value;
}

public class PolarVector2
{
    public PolarVector2() { }
    public PolarVector2(float angle, float length) { this.angle = angle; this.length = length; }
    public PolarVector2(Vector2 vector)
    {
        this.angle = Vector2.SignedAngle(Vector2.up, vector);
        this.length = vector.magnitude;
    }
    public float angle;
    public float length;

    public Vector2 GetVector()
    {
        return Quaternion.Euler(0, 0, angle) * Vector2.up * length;
    }
    public static PolarVector2 MoveTowards(PolarVector2 current, PolarVector2 target,
        float maxDistanceDeltaAngle, float maxDistanceDeltaLenght)
    {
        return new PolarVector2(
            Mathf.MoveTowardsAngle(current.angle, target.angle, maxDistanceDeltaAngle),
            Mathf.MoveTowardsAngle(current.length, target.length, maxDistanceDeltaLenght));
    }

    public static implicit operator Vector2(PolarVector2 vector)
    {
        return vector.GetVector();
    }

}

[Serializable]
public class FloatShake
{
    public FloatShake(float v = 0)
    {
        Reset(v);
    }

    // damping of shake offset
    public float offsetDamping;
    // damping of shake velocity
    public float velocityDamping;
    // strength of shake impulse
    public float impulse;
    public Timer tImpulse = new Timer(float.PositiveInfinity);

    // value represented by the property
    public float value { get; private set; }
    // offset caused by shake
    public float offset { get; private set; }
    // shake velocity 
    public float velocity { get; private set; }

    // reset shake and set value to given parameter
    public void Reset(float initialValue)
    {
        value = initialValue;
        offset = 0;
        velocity = 0;
    }
    public void ResetValue(float initialValue)
    {
        value = initialValue;
    }

    // add shake impulse
    public void AddImpulse(float impulse)
    {
        velocity += impulse;
    }

    // add shake impulse with random sign
    public void AddRandomImpulse(float maxImpulse)
    {
        float f = Random.value * 2 - 1;
        velocity += f * maxImpulse;
    }

    // should be called every certain time
    public void UpdateImpulse()
    {
        if (tImpulse.IsReadyRestart())
        {
            AddRandomImpulse(impulse);
        }
    }

    private void UpdateShake()
    {
        velocity *= velocityDamping;
        offset *= offsetDamping;

        offset += velocity;
    }

    public bool UpdateTowards(float towards, float speed)
    {
        float currentValue = value - offset;
        UpdateShake();

        value = Mathf.MoveTowards(currentValue, towards, speed * Time.deltaTime) + offset;
        return Mathf.Approximately(value - offset, towards);
    }
    public bool UpdateTowardsLerp(float towards, float speed)
    {
        float currentValue = value - offset;
        UpdateShake();

        value = Mathf.Lerp(currentValue, towards, speed * Time.deltaTime) + offset;
        return Mathf.Approximately(value - offset, towards);
    }

    public bool UpdateTowardsAngle(float towards, float speed)
    {
        float currentValue = value - offset;
        UpdateShake();

        value = Mathf.MoveTowardsAngle(currentValue, towards, speed * Time.deltaTime) + offset;
        return Mathf.Approximately(value - offset, towards);
    }
    public bool UpdateTowardsAngleLerp(float towards, float speed)
    {
        float currentValue = value - offset;
        UpdateShake();

        value = Mathf.LerpAngle(currentValue, towards, speed * Time.deltaTime) + offset;
        return Mathf.Approximately(value - offset, towards);
    }
}


public static class FloatExtensions
{
    public static float Sq(this float f)
    {
        return f * f;
    }
    public static float Sqrt(this float f)
    {
        return Mathf.Sqrt(f);
    }

    public static float Abs(this float f)
    {
        return Mathf.Abs(f);
    }

    public static float DistanceTo(this float f, float b)
    {
        return (f - b).Abs();
    }
}

public static class VectorExtensions
{
    public static PolarVector2 GetPolarVector(this Vector2 vector)
    {
        return new PolarVector2(vector);
    }

    public static Vector2 To2D(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }

    public static Vector3 To3D(this Vector2 vector)
    {
        return new Vector3(vector.x, 0, vector.y);
    }

    public static Vector3 ToPlane(this Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    public static Vector2 RotateAroundPivot(this Vector2 point, Vector2 pivot, float angle)
    {
        var dir = point - pivot;
        dir = Quaternion.Euler(0, 0, angle) * dir;
        point = dir + pivot;
        return point;
    }

    public static bool Approximately(this Vector3 vA, Vector3 vB)
    {
        return Mathf.Approximately(vA.x, vB.x)
            && Mathf.Approximately(vA.y, vB.y)
            && Mathf.Approximately(vA.z, vB.z);
    }

    public static bool Approximately(this Vector2 vA, Vector2 vB)
    {
        return Mathf.Approximately(vA.x, vB.x)
            && Mathf.Approximately(vA.y, vB.y);
    }
}

public static class CollectionExtensions
{

    public static List<T> Clone<T>(this List<T> arr)
    {
        return new List<T>(arr);
    }
    public static T[] Clone<T>(this T[] arr)
    {
        var a = new T[arr.Length];
        for(int i = 0; i < arr.Length; ++i)
        {
            a[i] = arr[i];
        }
        return a;
    }

    public static T GetRandomElement<T>(this List<T> arr)
    {
        if (arr == null || arr.Count == 0) return default(T);
        var rand = UnityEngine.Random.Range(0, arr.Count);
        return arr[rand];
    }

    public static T GetRandomElement<T>(this T[] arr)
    {
        if (arr == null || arr.Length == 0) return default(T);
        var rand = Random.Range(0, arr.Length);
        return arr[rand];
    }

    public static T GetRandomElement<T>(this List<T> arr, T _default) where T : class
    {
        if (arr == null || arr.Count == 0) return _default;
        var rand = UnityEngine.Random.Range(0, arr.Count);
        return arr[rand];
    }

    public static T GetRandomElement<T>(this T[] arr, T _default) where T : class
    {
        if (arr == null || arr.Length == 0) return _default;
        var rand = UnityEngine.Random.Range(0, arr.Length);
        return arr[rand];
    }

    public static bool IsEmpty<T>(this T[] c)
    {
        return c.Length == 0;
    }

    public static bool IsEmpty<T>(this List<T> c)
    {
        return c.Count == 0;
    }

    public static bool IsEmpty(this ICollection c)
    {
        return c.Count == 0;
    }


    public static T Last<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
            return default(T);

        return list[list.Count - 1];
    }
    public static T Last<T>(this List<T> list, T _default)
    {
        if (list == null || list.Count == 0)
            return _default;

        return list[list.Count - 1];
    }

    public static T Last<T>(this T[] list)
    {
        if (list == null || list.Length == 0)
            return default(T);

        return list[list.Length - 1];
    }
    public static T Last<T>(this T[] list, T _default)
    {
        if (list == null || list.Length == 0)
            return _default;

        return list[list.Length - 1];
    }

    public static T Pop<T>(this List<T> list)
    {
        var last = list.Last();
        list.Remove(last);
        return last;
    }
    public static T Pop<T>(this List<T> list, T _default)
    {
        var last = list.Last(_default);
        list.Remove(last);
        return last;
    }
}

public static class GeneralExtensions
{
    public static void SetLayerRecursively(this GameObject go, int layer)
    {
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layer;
        }
    }
    public static void SetLayerRecursively(this Transform transform, int layer)
    {
        SetLayerRecursively(transform.gameObject, layer);
    }

    public static bool NotNulls<T>(this T[] arr) where T : class
    {
        return NotNulls(arr);
    }

    public static IEnumerator FadeTo(this SpriteRenderer sprite, float desiredAlpha, float transitTime)
    {
        yield return null;
        float timeElapsed = 0;
        float startAlpha = sprite.color.a;
        while (timeElapsed <= transitTime)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.Lerp(startAlpha, desiredAlpha, timeElapsed / transitTime));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }



    public static void DestroyAllChildren<T>(this Transform transform) where T : UnityEngine.Component
    {
        var list = transform.GetComponentsInChildren<T>();
        for (int i = list.Length - 1; i >= 0; i--)
        {
            GameObject.Destroy(list[i].gameObject);
        }
    }

    public static void DestroyAllChildren(this Transform transform)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }
    }


    public static bool TrueForAll<T>(this Predicate<T> predicate, params T[] objs)
    {
        bool succ = true;

        foreach (var o in objs)
        {
            succ &= predicate(o);
        }
        return succ;
    }

    public static bool TrueForAny<T>(this Predicate<T> predicate, params T[] objs)
    {
        bool succ = false;

        foreach (var o in objs)
        {
            succ |= predicate(o);
        }
        return succ;
    }
}

// Debug drawing helpers
public static class DebugUtilities
{
    public static void DrawRectangle(Rect rect)
    {
        var lt = new Vector2(rect.xMin, rect.yMax);
        var lb = new Vector2(rect.xMin, rect.yMin);
        var rt = new Vector2(rect.xMax, rect.yMax);
        var rb = new Vector2(rect.xMax, rect.yMin);

        Gizmos.DrawLine(lt, lb);
        Gizmos.DrawLine(lb, rb);
        Gizmos.DrawLine(rb, rt);
        Gizmos.DrawLine(rt, lt);
    }

    public static void DrawDebug(this Rect r, Color c)
    {
        var tl = new Vector3(r.xMin, r.yMin);
        var tr = new Vector3(r.xMax, r.yMin);
        var bl = new Vector3(r.xMin, r.yMax);
        var br = new Vector3(r.xMax, r.yMax);

        Debug.DrawLine(tl, tr, c);
        Debug.DrawLine(tr, br, c);
        Debug.DrawLine(br, bl, c);
        Debug.DrawLine(bl, tl, c);
    }

    public static void DrawCross2D(Vector3 position, float width, Color color)
    {
        var mid = (Vector2)position;
        var l = mid + Vector2.left * width * .5f;
        var r = mid + Vector2.right * width * .5f;
        var t = mid + Vector2.up * width * .5f;
        var b = mid + Vector2.down * width * .5f;

        Debug.DrawLine(l, r, color);
        Debug.DrawLine(t, b, color);
    }
    public static void DrawCross2D(Vector3 position, float width)
    {
        DrawCross2D(position, width, Color.green);
    }

    public static void DrawCross(Vector3 position, float width, Color color)
    {
        var mid = position;
        var l = mid + Vector3.left * width * .5f;
        var r = mid + Vector3.right * width * .5f;
        var t = mid + Vector3.forward * width * .5f;
        var b = mid + Vector3.back * width * .5f;

        Debug.DrawLine(l, r, color);
        Debug.DrawLine(t, b, color);
    }
    public static void DrawCross(Vector3 position, float width)
    {
        DrawCross(position, width, Color.green);
    }
}
