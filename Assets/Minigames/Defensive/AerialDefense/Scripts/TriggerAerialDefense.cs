using UnityEngine;

public class TriggerAerialDefense : MonoBehaviour
{
    public string LevelName;
    public void Trigger()
    {
        AerialDefenseLevelData levelData = AerialDefenseLevelData.LevelDictionary[LevelName];
        levelData.StartLevel();
    }
}
