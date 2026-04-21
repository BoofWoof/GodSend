using PixelCrushers;
using System;

public class HealthSaver : Saver
{
    [Serializable]
    public class HealthSaveData
    {
        public float Health;
    }
    public override string RecordData()
    {
        HealthSaveData newSaveData = new HealthSaveData()
        {
            Health = DefenseStats.CityEfficiencyHealth
        };
        return SaveSystem.Serialize(newSaveData);
    }
    public override void ApplyData(string s)
    {
        HealthSaveData saveData = SaveSystem.Deserialize<HealthSaveData>(s);

        if (saveData == null) return;

        DefenseStats.SetCityHealth(saveData.Health);
    }
}
