using System.Collections.Generic;
using UnityEngine;

public class UniqueIDScript : MonoBehaviour
{
    public string UniqueIDValue;

    private static Dictionary<string, UniqueIDScript> UniqueIDDict = new();

    public static UniqueIDScript GetScriptByID(string targetID)
    {
        if (UniqueIDDict.Count == 0) RefreshDictionary();

        if (!UniqueIDDict.ContainsKey(targetID)) return null;

        return UniqueIDDict[targetID];
    }

    public static void RefreshDictionary()
    {
        UniqueIDScript[] uniqueIDScripts = FindObjectsByType<UniqueIDScript>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        UniqueIDDict = new();

        foreach (UniqueIDScript uniqueIDScript in uniqueIDScripts)
        {
            UniqueIDDict.Add(uniqueIDScript.UniqueIDValue, uniqueIDScript);
        }
    }
}
