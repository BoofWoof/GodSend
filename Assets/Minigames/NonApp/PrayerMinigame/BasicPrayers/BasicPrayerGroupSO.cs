using PixelCrushers.DialogueSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using static FirstTimeSave;

[CreateAssetMenu(fileName = "BasicPrayerGroupSO", menuName = "Prayers/BasicPrayerGroupSO")]
public class BasicPrayerGroupSO : ScriptableObject
{
    public string GroupName;
    public string[] Responses;

    [Header("Prayers")]
    [TextArea]public string AlwaysAvailable;
    [TextArea] public string DayTwoAvailable;
    [TextArea] public string DayThreeAvailable;
    [TextArea] public string DayFourAvailable;
    [TextArea] public string DayFiveAvailable;

    [Serializable]
    public class FirstTied
    {
        public PossibleFirsts FirstToUnlock;
        [TextArea] public string Prayers;
    }
    [Header("Gated Prayers")]
    public FirstTied[] GatedPrayersArray;

    private int CurrentPrayerIdx = -1;
    [SerializeField] private List<PrayerData> PrayerDataList = new();

    public void ResetPrayerIdx()
    {
        CurrentPrayerIdx = 0;
    }

    [ContextMenu("Prepare List")]
    public void PrepareGroup()
    {
        CreatePrayerList();

        FirstTimeSave.OnNewFirst += OnNewFirst;
    }

    [Serializable]
    public struct PrayerData
    {
        public string Prayer;
        public string Author;
        public BasicPrayerGroupSO SourcePrayer;
    }

    public void CreatePrayerList()
    {
        PrayerDataList = new();

        PrayerDataList.AddRange(ConvertTextToPrayerList(AlwaysAvailable));
        if(DayInfo.CurrentDay >= 2) AddPrayers(ConvertTextToPrayerList(DayTwoAvailable));
        if (DayInfo.CurrentDay >= 3) AddPrayers(ConvertTextToPrayerList(DayThreeAvailable));
        if (DayInfo.CurrentDay >= 4) AddPrayers(ConvertTextToPrayerList(DayFourAvailable));
        if (DayInfo.CurrentDay >= 5) AddPrayers(ConvertTextToPrayerList(DayFiveAvailable));

        //Need an unlock tracker.
    }

    public void OnNewFirst(PossibleFirsts newFirst)
    {
        foreach (FirstTied firstGate in GatedPrayersArray)
        {
            if (firstGate.FirstToUnlock == newFirst) AddPrayers(ConvertTextToPrayerList(firstGate.Prayers));
        }
    }

    public void AddPrayers(List<PrayerData> newPrayers)
    {
        PrayerDataList.InsertRange(CurrentPrayerIdx, newPrayers);
    }

    public List<PrayerData> ConvertTextToPrayerList(string text)
    {
        List<PrayerData> prayerDataList = new();

        string[] textLines = text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in textLines)
        {
            string[] split = line.Split(" @");
            PrayerData newPrayerData = (new PrayerData
            {
                Prayer = split[0],
                Author = split.Length > 1 ? split[1] : string.Empty,
                SourcePrayer = this
            });

            prayerDataList.Add(newPrayerData);

        }

        return prayerDataList;
    }

    public int GetPrayerCount()
    {
        return PrayerDataList.Count;
    }

    public PrayerData SamplePrayer()
    {
        CurrentPrayerIdx += 1;
        CurrentPrayerIdx = CurrentPrayerIdx % PrayerDataList.Count;
        return PrayerDataList[CurrentPrayerIdx];
    }
}
