using PixelCrushers;
using System;
using UnityEngine;

public class AriesRockSaver : Saver
{
    public OverworldAngryRockScript targetOverworldAngryRockScript;

    [Serializable]
    public class ARockSaveData
    {
        //Overworld Values
        public bool WarningZero = false;
        public bool WarningOne = false;
        public bool WarningTwo = false;
        public bool Death = false;

        //UI Script
        public bool ActiveRun;
        public float SavedTime;
        public int Score;
        public int HighScore;

        public void SetOverwoldData(OverworldAngryRockScript data)
        {
            WarningZero = data.WarningZero;
            WarningOne = data.WarningOne;
            WarningTwo = data.WarningTwo;
            Death = data.Death;
        }

        public void LoadOverworldData(OverworldAngryRockScript data)
        {
            data.WarningZero = WarningZero;
            data.WarningOne = WarningOne;
            data.WarningTwo = WarningTwo;
            data.Death = Death;
        }

        public void SetUIData()
        {
            ActiveRun = AngyRockScript.ActiveRun;
            SavedTime = AngyRockScript.TimePassed();
            Score = AngyRockScript.Score;
            HighScore = AngyRockScript.HighScore;
        }

        public void LoadUIData()
        {
            AngyRockScript.BonusSaveTime = SavedTime;

            AngyRockScript.Score = Score;
            AngyRockScript.HighScore = HighScore;

            AngyRockScript.ActiveRun = ActiveRun;
            if (ActiveRun) AngyRockScript.ResetStartingTime();
        }
    }
    public override string RecordData()
    {
        ARockSaveData saveData = new ARockSaveData();

        saveData.SetOverwoldData(targetOverworldAngryRockScript);
        saveData.SetUIData();

        return SaveSystem.Serialize(saveData);
    }
    public override void ApplyData(string s)
    {
        ARockSaveData saveData = SaveSystem.Deserialize<ARockSaveData>(s);

        if (saveData == null) return;

        saveData.LoadOverworldData(targetOverworldAngryRockScript);
        saveData.LoadUIData();
    }
}
