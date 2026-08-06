using PixelCrushers.DialogueSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "GiftUpgrade", menuName = "Upgrades/Relationship/GiftUpgrade")]
public class GiftUpgrade : UpgradesAbstract
{
    public string DialogueName;
    public override void OnBuy(bool load)
    {
        Debug.Log("Lol, jk no gift 4 u.");
    }
}
