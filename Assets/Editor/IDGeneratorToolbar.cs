using System;
using UnityEditor;
using UnityEngine;

public class IDGeneratorToolbar
{
    // This creates the button in the top toolbar
    [MenuItem("Tools/Generate Missing IDs")]
    public static void GenerateIDs()
    {
        SpecialPrayerSetSO[] targets = Resources.FindObjectsOfTypeAll<SpecialPrayerSetSO>();
        int count = 0;
        foreach (SpecialPrayerSetSO target in targets)
        {
            if (string.IsNullOrEmpty(target.ID))
            {
                Undo.RecordObject(target, "Generate Unique ID");

                target.ID = Guid.NewGuid().ToString();

                EditorUtility.SetDirty(target);
                count++;
            }
        }

        UpgradesAbstract[] upgradeTargets = Resources.FindObjectsOfTypeAll<UpgradesAbstract>();
        foreach (UpgradesAbstract target in upgradeTargets)
        {
            if (string.IsNullOrEmpty(target.UpgradeID))
            {
                Undo.RecordObject(target, "Generate Unique ID");

                target.UpgradeID = Guid.NewGuid().ToString();

                EditorUtility.SetDirty(target);
                count++;
            }
        }

        EventAbstract[] eventTargets = Resources.FindObjectsOfTypeAll<EventAbstract>();
        foreach (EventAbstract target in eventTargets)
        {
            if (string.IsNullOrEmpty(target.EventID))
            {
                Undo.RecordObject(target, "Generate Unique ID");

                target.EventID = Guid.NewGuid().ToString();

                EditorUtility.SetDirty(target);
                count++;
            }
        }

        Debug.Log($"ID Generation Complete. Assigned {count} new IDs.");
    }
}
