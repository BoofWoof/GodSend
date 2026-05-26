using PixelCrushers.DialogueSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "ADTriggerUpgrade", menuName = "Upgrades/DefenseTriggers/ADTriggerUpgrade")]
public class ADTriggerUpgrade : UpgradesAbstract
{
    public override void OnBuy()
    {
        OverworldBehavior.AriesBehavior("soda");
    }
}
