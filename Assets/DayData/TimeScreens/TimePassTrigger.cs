using PixelCrushers;
using System;

public class TimePassTrigger : Saver
{
    public string Title;
    public string TextTime;

    public float Hour;

    public bool Triggered = false;
    public TimePassingScript TargetTimePassScript;

    public int TriggerDay = -1;

    [Serializable]
    public class TimePassTriggerSave
    {
        public bool Triggered = false;
    }

    public override string RecordData()
    {
        TimePassTriggerSave saveData = new TimePassTriggerSave();
        saveData.Triggered = Triggered;

        return SaveSystem.Serialize(saveData);
    }

    public override void ApplyData(string s)
    {
        TimePassTriggerSave saveData = SaveSystem.Deserialize<TimePassTriggerSave>(s);
        if (saveData == null) return;

        Triggered = saveData.Triggered;
    }

    public void Trigger()
    {
        if (Triggered) return;
        if (TriggerDay != DayInfo.CurrentDay) return;
        TargetTimePassScript.StartDisplay(this);
    }
}
