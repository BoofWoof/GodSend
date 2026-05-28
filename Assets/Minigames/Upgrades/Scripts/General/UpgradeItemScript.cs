using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemScript : MonoBehaviour
{
    public UpgradesAbstract AssociatedUpgrade;
    public UpgradeItemListScript AssociatedList;

    public TextMeshProUGUI NameText;
    public TextMeshProUGUI DescriptionText;
    public TextMeshProUGUI CostText;
    public Image UpgradeImage;

    public Button BuyButton;

    public UpgradeScreenScript SourceScreen;

    private bool DisablePurchases = false;

    virtual public void UpdateUI()
    {
        NameText.text = AssociatedUpgrade.UpgradeName;
        if(DescriptionText != null) DescriptionText.text = AssociatedUpgrade.UpgradeDescription;
        CostText.text = AssociatedUpgrade.CostToText();
        UpgradeImage.sprite = AssociatedUpgrade.UpgradeIcon;

        BuyButton.GetComponent<Image>().color = new Color(0.35f, 0.3f, 0.3f);
    }
    public void SetSource(UpgradeScreenScript sourceScreen)
    {
        SourceScreen = sourceScreen;
    }

    public void SetUpgrade(UpgradesAbstract associatedUpgrade)
    {
        AssociatedUpgrade = associatedUpgrade;
        UpdateUI();
    }

    public void ForceDisablePurchases()
    {
        DisablePurchases = true;
        BuyButton.GetComponent<Image>().color = new Color(0.35f, 0.3f, 0.3f);
        BuyButton.interactable = false;
    }

    public void OnEnable()
    {
        if (SourceScreen != null) SourceScreen.UpgradeObjects.Add(gameObject);
        StartCoroutine(AffordCheck());
    }

    public void OnDisable()
    {
        if(SourceScreen != null) SourceScreen.UpgradeObjects.Remove(gameObject);
        StopAllCoroutines();
    }

    public IEnumerator AffordCheck()
    {
        while (true)
        {
            if(AssociatedUpgrade == null)
            {
                yield return new WaitForSeconds(0.2f);
                continue;
            }

            if (AssociatedUpgrade.CanBuy() && !DisablePurchases) BuyButton.GetComponent<Image>().color = new Color(1f, 1f, 1f);
            else BuyButton.GetComponent<Image>().color = new Color(0.35f, 0.3f, 0.3f);
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void Buy()
    {
        if (AssociatedUpgrade.UpgradeBought) return;
        if (DisablePurchases || !AssociatedUpgrade.Buy())
        {
            AnnouncementScript.StartAnnouncement("You can't afford this upgrade. Go do more puzzles!");
            return;
        }
        AssociatedList.OnPurchase();
    }

}