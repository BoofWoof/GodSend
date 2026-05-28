using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpgradeItemListScript : MonoBehaviour
{
    public GameObject DestructionRoot;

    public UpgradesAbstract AssociatedUpgrade;
    public List<UpgradesAbstract> AssociatedUpgrades;
    public UpgradeScreenScript SourceScreen;

    private List<UpgradeItemScript> ItemList = new List<UpgradeItemScript>();

    public GameObject UpgradeOptionPrefab;

    public Color BaseColor;
    public Color SpecialColor;
    public Color MultiColor;

    public float Duration = 1f;

    public static List<UpgradeItemListScript> MultiListUpgrades = new List<UpgradeItemListScript>();

    public UnityEvent OnPurchaseTrigger;

    public GameObject TopTitle;

    public void SetSource(UpgradeScreenScript sourceScreen)
    {
        SourceScreen = sourceScreen;
    }
    public void SetUpgrade(UpgradesAbstract associatedUpgrade)
    {
        AssociatedUpgrade = associatedUpgrade;
        GenerateSingleUpgrade();
    }
    public void SetUpgrade(List<UpgradesAbstract> associatedUpgrade)
    {
        AssociatedUpgrades = associatedUpgrade;
        GenerateMultiUpgrade();
    }

    public UpgradeItemScript AddUpgradeToList(UpgradesAbstract targetUpgrade)
    {
        GameObject newOption = Instantiate(UpgradeOptionPrefab);
        newOption.transform.SetParent(transform);
        newOption.transform.SetAsFirstSibling();
        newOption.transform.localPosition = Vector3.zero;
        newOption.transform.localRotation = Quaternion.identity;
        newOption.transform.localScale = Vector3.one;

        UpgradeItemScript uiScript = newOption.GetComponent<UpgradeItemScript>();
        uiScript.AssociatedList = this;
        uiScript.SetUpgrade(Instantiate(targetUpgrade));

        ItemList.Add(uiScript);

        return uiScript;
    }

    public void GenerateSingleUpgrade()
    {
        ItemList.Clear();
        UpgradeItemScript uiScript = AddUpgradeToList(AssociatedUpgrade);
        if (AssociatedUpgrade.GoldenUpgrade)
        {
            GetComponent<Image>().color = SpecialColor;
            SetTopTitle("Recommended");
        }
        else
        {
            RemoveTopTitle();
        }
    }

    public void DestroySelf()
    {
        if (DestructionRoot)
        {
            Destroy(DestructionRoot);
            return;
        }
        Destroy(gameObject);
    }

    public void GenerateMultiUpgrade()
    {
        ItemList.Clear();
        MultiListUpgrades.RemoveAll(item => item == null || item.AssociatedUpgrade.UpgradeID == AssociatedUpgrade.UpgradeID);
        if (AssociatedUpgrades.Count == 0)
        {
            DestroySelf();
            return;
        }
        else if (AssociatedUpgrades.Count == 1)
        {
            AssociatedUpgrade = AssociatedUpgrades[0];
            GenerateSingleUpgrade();
            return;
        }


        SetTopTitle("PICK ONE");
        foreach(UpgradesAbstract targetUpgrade in AssociatedUpgrades)
        {
            if (SourceScreen.PreboughtUpgradeIDs.Contains(targetUpgrade.UpgradeID)) continue;
            UpgradeItemScript uiScript = AddUpgradeToList(targetUpgrade);
        }
        GetComponent<Image>().color = MultiColor;
        MultiListUpgrades.Add(this);

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }

    public void OnDisable()
    {
        MultiListUpgrades.Remove(this);
    }
    public void OnDestroy()
    {
        MultiListUpgrades.Remove(this);
        if(ItemList.Count > 1) RefreshMultiList();
    }

    public void RefreshMultiList()
    {
        List<UpgradeItemListScript> clone = new List<UpgradeItemListScript>(MultiListUpgrades);
        foreach (UpgradeItemListScript itemList in clone)
        {
            itemList.ClearList();
            itemList.GenerateMultiUpgrade();
        }
    }

    public void ClearList()
    {
        foreach (UpgradeItemScript child in ItemList)
        {
            if (child == null) continue;
            Destroy(child.gameObject);
        }
        ItemList.Clear();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)SourceScreen.transform);
    }


    public void OnPurchase()
    {
        AssociatedUpgrade.AddToPurchasedList();
        AssociatedUpgrade.UpgradeBought = true;
        RemoveTopTitle();

        foreach (UpgradeItemScript targetUpgradeItem in ItemList)
        {
            targetUpgradeItem.ForceDisablePurchases();
        }

        OnPurchaseTrigger?.Invoke();

        MultiListUpgrades.RemoveAll(item => item == null || item.AssociatedUpgrade.UpgradeID == AssociatedUpgrade.UpgradeID);
    }
    public void RemoveTopTitle()
    {
        if (TopTitle == null) return;
        TopTitle.SetActive(false);
    }

    public void SetTopTitle(string TopText)
    {
        if (TopTitle == null) return;
        TopTitle.GetComponentInChildren<TMP_Text>().text = TopText;
    }
}
