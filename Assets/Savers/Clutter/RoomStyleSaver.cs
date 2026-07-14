using PixelCrushers;
using System;
using UnityEngine;
using static AriesRockSaver;

public class RoomStyleSaver : Saver
{
    [Serializable]
    public class RoomStyleSaveData
    {
        public int CurrentRoom;
    }
    public override string RecordData()
    {
        RoomStyleSaveData saveData = new RoomStyleSaveData();

        saveData.CurrentRoom = WallpaperChangerScript.instance.GetWallpaperID();

        return SaveSystem.Serialize(saveData);
    }
    public override void ApplyData(string s)
    {
        RoomStyleSaveData saveData = SaveSystem.Deserialize<RoomStyleSaveData>(s);

        if (saveData == null) return;

        WallpaperChangerScript.instance.SetWallpaperTexture(saveData.CurrentRoom);
    }
}
