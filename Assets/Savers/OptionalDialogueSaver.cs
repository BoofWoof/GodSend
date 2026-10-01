using PixelCrushers;
using System;
using System.Collections;
using System.Collections.Generic;

public class OptionalDialogueSaver : Saver
{
    public OCManager targetOCManager;

    bool hasLoadedFromSave = false;

    [Serializable]
    public class OptionalDialogueSaveData
    {
        public List<string> UsedUpOCList = new();
        public List<string> UnlockedOCList = new();
    }

    public override string RecordData()
    {
        OptionalDialogueSaveData newSaveData = new();
        newSaveData.UsedUpOCList = targetOCManager.UsedUpOC;

        foreach(string dictKey in OCUnlockTriggerScript.OCUnlockDict.Keys)
        {
            if (OCUnlockTriggerScript.OCUnlockDict[dictKey].Released)
            {
                newSaveData.UnlockedOCList.Add(dictKey);
            }
        }

        return SaveSystem.Serialize(newSaveData);
    }

    public override void ApplyData(string s)
    {
        OptionalDialogueSaveData loadedData = SaveSystem.Deserialize<OptionalDialogueSaveData>(s);

        foreach (string dictKey in OCUnlockTriggerScript.OCUnlockDict.Keys)
        {
            if (loadedData.UnlockedOCList.Contains(dictKey)) OCUnlockTriggerScript.OCUnlockDict[dictKey].Release();
            OCUnlockTriggerScript.OCUnlockDict[dictKey].CheckAutoRelease();
        }

        targetOCManager.UsedUpOC = loadedData.UsedUpOCList;

        hasLoadedFromSave = true;
    }

    override public void Start()
    {
        base.Start();

        StartCoroutine(CheckForNoLoad());
    }

    public IEnumerator CheckForNoLoad()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        if (!hasLoadedFromSave)
        {
            foreach (string dictKey in OCUnlockTriggerScript.OCUnlockDict.Keys)
            {
                OCUnlockTriggerScript.OCUnlockDict[dictKey].CheckAutoRelease();
            }
        }
    }
}
