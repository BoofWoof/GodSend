using PixelCrushers.DialogueSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "ADTriggerUpgrade", menuName = "Upgrades/DefenseTriggers/ADTriggerUpgrade")]
public class ADTriggerUpgrade : UpgradesAbstract
{
    public string LevelName;
    public override void OnBuy()
    {
        AerialDefenseLevelData.PrepLevelByName(LevelName);

        OverworldBehavior.AriesBehavior("soda");
    }
}
