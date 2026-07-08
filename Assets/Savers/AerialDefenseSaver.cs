using PixelCrushers;
using System;

public class AerialDefenseSaver : Saver
{
    [Serializable]
    public class AerialSave
    {
        public int ThreatsDestroyed;
    }

    public override string RecordData()
    {
        AerialSave newSaveData = new AerialSave();

        newSaveData.ThreatsDestroyed = AerialDefenseScript.TotalProjectilesDestroyed;

        return SaveSystem.Serialize(newSaveData);
    }

    public override void ApplyData(string s)
    {
        AerialSave saveData = SaveSystem.Deserialize<AerialSave>(s);

        if (saveData == null) return;

        AerialDefenseScript.TotalProjectilesDestroyed = saveData.ThreatsDestroyed;
    }
}
