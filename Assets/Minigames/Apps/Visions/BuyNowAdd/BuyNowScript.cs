using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyNowScript : MonoBehaviour
{
    public Image UpgradeImage;
    public TMP_Text DescriptionText;

    public void UpdateDescriptions(UpgradesAbstract targetUpgrade)
    {
        UpgradeImage.sprite = targetUpgrade.UpgradeIcon;
        DescriptionText.text = targetUpgrade.AdText;
    }
}
