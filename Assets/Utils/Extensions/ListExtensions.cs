using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class ListExtensions
{
    public static T RandomlySelectValue<T>(this List<T> input)
    {
        if (input.Count == 0) return default(T);

        int RandomIdx = Random.Range(0, input.Count);
        return input[RandomIdx];
    }
    public static T RandomlySelectValue<T>(this T[] input)
    {
        if (input == null || input.Length == 0) return default(T);

        int RandomIdx = Random.Range(0, input.Length);
        return input[RandomIdx];
    }
}
