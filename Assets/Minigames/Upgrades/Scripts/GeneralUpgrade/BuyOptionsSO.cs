using UnityEngine;

[CreateAssetMenu(fileName = "BuyOptions", menuName = "Upgrades/BuyOptions")]
public class BuyOptionsSO : UpgradesAbstract
{
    [Header("Poster Name")]
    public string PosterName;

    public override void OnBuy()
    {

    }
}
