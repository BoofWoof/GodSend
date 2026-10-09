using System.Collections.Generic;
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
    public static void Shuffle<T>(this List<T> list)
    {
        int count = list.Count;
        for (int i = 0; i < count - 1; i++)
        {
            // Pick a random index from i to count - 1
            int randomIndex = Random.Range(i, count);

            // Swap list[i] with list[randomIndex]
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
