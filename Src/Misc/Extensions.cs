using UnityEngine;

using System;
using System.Collections.Generic;

public static class Extensions
{
    /// <summary>
    /// Clamps lerp range, assumes v is in 0 .. 1 range.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float ClampLerp(this float v, float min, float max)
    {
        return max + v * min;
    }

    public static T Random<T>(this T[] array)
    {
        return array[Synched.Next(0, array.Length)];
    }
    public static T RandomWeighted<T>(this T[] array, params int[] weights)
    {
        UnityEngine.Debug.Assert(array.Length == weights.Length, "RandomWeighted(): object array length does not match weights array length!");

        int sum = 0;

        for (int i = 0; i < weights.Length; i++)
            sum += weights[i];

        int roll = Synched.Next(0, sum);

        for (int i = 0; i < array.Length; i++)
        {
            if (roll < weights[i])
                return array[i];

            roll -= weights[i];
        }

        return default;
    }
    public static T Last<T>(this T[] array)
    {
        return array[array.Length - 1];
    }
    public static int IndexOf<T>(this T[] array, T item)
    {
        return Array.IndexOf(array, item);
    }
    public static int WrapStep(this int i, int min, int max, bool direction = true)
    {
        i += direction ? 1 : -1;

        if (i < min)
            i = max;
        else if (i > max)
            i = min;

        return i;
    }

    public static T Random<T>(this List<T> list)
    {
        return list[Synched.Next(0, list.Count)];
    }
    public static T RandomWeighted<T>(this List<T> list, params int[] weights)
    {
        UnityEngine.Debug.Assert(list.Count == weights.Length, "RandomWeighted(): object list length does not match weights array length!");

        int sum = 0;

        for (int i = 0; i < weights.Length; i++)
            sum += weights[i];

        int roll = Synched.Next(0, sum);
    
        for (int i = 0; i < list.Count; i++)
        {
            if (roll < weights[i])
                return list[i];

            roll -= weights[i];
        }

        return default;
    }
    public static T Last<T>(this List<T> list)
    {
        return list[list.Count - 1];
    }
    public static void RemoveLast<T>(this List<T> list)
    {
        list.RemoveAt(list.Count - 1);
    }
    public static void RemoveFirst<T>(this List<T> list)
    {
        list.RemoveAt(0);
    }

    public static float Smoothstep(this float value)
    {
        return value * value * value * value;
    }
    public static float CubicEaseOut(this float value)
    {
        return 1f + ((value -= 1f) * value * value);
    }

    public static Transform FindChildByName(this Transform root, string name)
    {
        Transform bone = null;

        if (root.name == name)
            return root;
        else
        {
            for (int i = 0; i < root.childCount; i++)
            {
                bone = root.GetChild(i).FindChildByName(name);

                if (bone != null)
                    return bone;
            }
        }

        return null;
    }
}