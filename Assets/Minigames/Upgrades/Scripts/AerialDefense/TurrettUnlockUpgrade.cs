using PixelCrushers.DialogueSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "TurrettUnlockUpgrade", menuName = "Upgrades/AerialDefense/TurrettUnlockUpgrade")]
public class TurrettUnlockUpgrade : UpgradesAbstract
{
    public string DialogueName;
    public override void OnBuy()
    {
        Debug.Log("Lol, jk no turret 4 you.");
    }
}
