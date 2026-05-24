using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopprHoverDescription : MonoBehaviour
{
    public static ShopprHoverDescription Instance;

    public Image ObjectImage;
    public Image ShadowImage;

    public TMP_Text Title;

    public TMP_Text Price;

    public TMP_Text StockCount;

    public TMP_Text Description;

    public void Awake()
    {
        Instance = this;
    }

    public void Clear()
    {
        ObjectImage.gameObject.SetActive(false);
        ShadowImage.gameObject.SetActive(false);

        Title.text = "";
        Price.text = "";
        StockCount.text = "";
        Description.text = "Hover any object for more info.";
    }

    public void UpdateData(UpgradeItemScript data){
        UpgradesAbstract upgrade = data.AssociatedUpgrade;

        ObjectImage.gameObject.SetActive(true);
        ObjectImage.sprite = upgrade.UpgradeIcon;
        ShadowImage.gameObject.SetActive(true);
        ShadowImage.sprite = upgrade.UpgradeIcon;

        Title.text = upgrade.UpgradeName;
        Price.text = upgrade.CostToText();
        StockCount.text = "Stock: 1/1";
        Description.text = upgrade.UpgradeMechanicDescription +
            "\n\n" +
            $"<i>({upgrade.UpgradeDescription})";
    }
}
