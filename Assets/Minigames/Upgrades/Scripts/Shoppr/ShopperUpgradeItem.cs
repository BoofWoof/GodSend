using TMPro;
using UnityEngine.UI;

public class ShopperUpgradeItem : UpgradeItemScript
{
    public TMP_Text PriceShadow;
    public Image ImageShadow;


    override public void UpdateUI()
    {
        base.UpdateUI();
        PriceShadow.text = CostText.text;
        ImageShadow.sprite = AssociatedUpgrade.UpgradeIcon;
    }
}
