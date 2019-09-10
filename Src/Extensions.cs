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

    public static ItemRarity GetRarityWeighted(this Random random, ItemRarity maxRarity = ItemRarity.Ancient)
    {
        int totalWeight = 0;

        for (int i = 0; i <= (int)maxRarity; i++)
            totalWeight += 1000 / (int)Math.Pow(1 + i, 4);

        int roll = random.Next(0, totalWeight + 1);

        for (int i = 0; i <= (int)maxRarity; i++)
        {
            if(roll < 1000 / (int)Math.Pow(1 + i, 4))
                return (ItemRarity)i;

            roll -= 1000 / (int)Math.Pow(1 + i, 4);
        }

        return ItemRarity.Common;
    }
}
