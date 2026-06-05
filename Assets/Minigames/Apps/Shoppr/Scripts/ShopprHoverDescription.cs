using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopprHoverDescription : MonoBehaviour
{
    public static Dictionary<Minigame, ShopprHoverDescription> Instances = new Dictionary<Minigame, ShopprHoverDescription>();

    public Image ObjectImage;
    public Image ShadowImage;

    public TMP_Text Title;

    public TMP_Text Price;

    public TMP_Text StockCount;

    public TMP_Text Description;

    public string DefaultDescription = "Hover any object for more info.";

    public Minigame AssociatedMinigame;

    public List<RectTransform> ToRebuildOnCompletion;

    public void Awake()
    {
        Instances.Add(AssociatedMinigame, this);
    }

    public void OnDestroy()
    {
        Instances.Remove(AssociatedMinigame);
    }

    public void Clear()
    {
        ObjectImage.gameObject.SetActive(false);
        if (ShadowImage != null) ShadowImage.gameObject.SetActive(false);

        Title.text = "";
        Price.text = "";
        StockCount.text = "";
        Description.text = DefaultDescription;
    }

    public void UpdateData(UpgradeItemScript data)
    {
        UpgradesAbstract upgrade = data.AssociatedUpgrade;

        ObjectImage.gameObject.SetActive(true);
        ObjectImage.sprite = upgrade.UpgradeIcon;
        if (ShadowImage != null)
        {
            ShadowImage.gameObject.SetActive(true);
            ShadowImage.sprite = upgrade.UpgradeIcon;
        }

        Title.text = upgrade.UpgradeName;
        Price.text = upgrade.CostToText();
        StockCount.text = "Stock: 1/1";
        Description.text = upgrade.UpgradeMechanicDescription +
            "\n\n" +
            $"<i>({upgrade.UpgradeDescription})";

        foreach(RectTransform rect in ToRebuildOnCompletion)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }
}
