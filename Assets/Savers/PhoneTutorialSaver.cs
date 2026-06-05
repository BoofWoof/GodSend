using PixelCrushers;
using System;

public class PhoneTutorialSaver : Saver
{

    [Serializable]
    public class PhoneTutorialSaveData
    {
        public bool Completed = false;
        public int MaxTutorialStep;
    }

    public override string RecordData()
    {
        PhoneTutorialScript pts = GetComponent<PhoneTutorialScript>();
        PhoneTutorialSaveData newSaveData = new PhoneTutorialSaveData()
        {
            Completed = pts.CompletedTutorial,
            MaxTutorialStep = pts.MaxTutorialStep
        };
        return SaveSystem.Serialize(newSaveData);
    }
    public override void ApplyData(string s)
    {
        PhoneTutorialSaveData saveData = SaveSystem.Deserialize<PhoneTutorialSaveData>(s);

        if (saveData == null) return;

        PhoneTutorialScript pts = GetComponent<PhoneTutorialScript>();

        if (saveData.Completed)
        {
            pts.CompletedTutorial = true;
            pts.HideTutorial();
        }
        pts.MaxTutorialStep = saveData.MaxTutorialStep;
    }

}
