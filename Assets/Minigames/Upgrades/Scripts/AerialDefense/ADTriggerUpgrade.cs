using PixelCrushers.DialogueSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "ADTriggerUpgrade", menuName = "Upgrades/DefenseTriggers/ADTriggerUpgrade")]
public class ADTriggerUpgrade : UpgradesAbstract
{
    public string DialogueName;
    public override void OnBuy()
    {
        Debug.Log("Lol, jk no fight 4 u.");
    }
}
