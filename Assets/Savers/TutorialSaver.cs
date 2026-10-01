using PixelCrushers;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSaver : Saver
{

    [Serializable]
    public class AllTutorialSaveData
    {
        public List<TutorialSaveData> TutorialManagerSaveData = new();
        public List<TutorialSaveData> TutorialGroupSaveData = new();
        public List<TutorialSaveData> TutorialItemSaveData = new();
    }

    [Serializable]
    public class TutorialSaveData
    {
        public string AssociatedID = "";
        public bool Unlocked = false;
        public bool Shown = false;
    }

    public override string RecordData()
    {
        AllTutorialSaveData newSaveData = new AllTutorialSaveData();

        TutorialScreenScript[] tutorialScreenScripts = FindObjectsByType<TutorialScreenScript>(
            FindObjectsInactive.Include, 
            FindObjectsSortMode.None
            );

        foreach (TutorialScreenScript tutorialScreenScript in tutorialScreenScripts)
        {
            TutorialSaveData newSaveDataPart = new TutorialSaveData
            {
                AssociatedID = tutorialScreenScript.UniqueIDValue,
                Unlocked = true,
                Shown = tutorialScreenScript.Shown
            };
            newSaveData.TutorialItemSaveData.Add(newSaveDataPart);
        }

        TutorialGroupScript[] tutorialGroupScripts = FindObjectsByType<TutorialGroupScript>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
            );

        foreach (TutorialGroupScript tutorialGroupScript in tutorialGroupScripts)
        {
            TutorialSaveData newSaveDataPart = new TutorialSaveData
            {
                AssociatedID = tutorialGroupScript.UniqueIDValue,
                Unlocked = tutorialGroupScript.Unlocked,
                Shown = tutorialGroupScript.Shown
            };
            newSaveData.TutorialGroupSaveData.Add(newSaveDataPart);
        }

        NewTutorialScript[] newTutorialScripts = FindObjectsByType<NewTutorialScript>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (NewTutorialScript tutorialManagerScript in newTutorialScripts)
        {
            TutorialSaveData newSaveDataPart = new TutorialSaveData
            {
                AssociatedID = tutorialManagerScript.UniqueIDValue,
                Unlocked = true,
                Shown = tutorialManagerScript.FirstTimeShown
            };
            newSaveData.TutorialManagerSaveData.Add(newSaveDataPart);
        }

        return SaveSystem.Serialize(newSaveData);
    }

    public override void ApplyData(string s)
    {
        AllTutorialSaveData loadedData = SaveSystem.Deserialize<AllTutorialSaveData>(s);

        foreach(TutorialSaveData saveData in loadedData.TutorialItemSaveData)
        {
            TutorialScreenScript targetScreenScript = (TutorialScreenScript)UniqueIDScript.GetScriptByID(saveData.AssociatedID);
            if (targetScreenScript == null) continue;
            targetScreenScript.Shown = saveData.Shown;
            if (targetScreenScript.TriggerOnAllTrigger && targetScreenScript.Shown) targetScreenScript.ONShow?.Invoke();
        }

        foreach (TutorialSaveData saveData in loadedData.TutorialGroupSaveData)
        {
            TutorialGroupScript targetGroupScript = (TutorialGroupScript)UniqueIDScript.GetScriptByID(saveData.AssociatedID);
            if (targetGroupScript == null) continue;
            targetGroupScript.Shown = saveData.Shown;
            targetGroupScript.Unlocked = saveData.Unlocked;
            if (targetGroupScript.TriggerOnLoad && targetGroupScript.Shown) targetGroupScript.OnFirstClose?.Invoke();
        }

        foreach (TutorialSaveData saveData in loadedData.TutorialManagerSaveData)
        {
            NewTutorialScript targetManagerScript = (NewTutorialScript)UniqueIDScript.GetScriptByID(saveData.AssociatedID);
            if (targetManagerScript == null) continue;
            targetManagerScript.FirstTimeShown = saveData.Shown;
        }
    }
}
