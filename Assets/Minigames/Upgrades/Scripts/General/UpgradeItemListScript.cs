using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemListScript : MonoBehaviour
{
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
        newOption.transform.localPosition = Vector3.zero;
        newOption.transform.localRotation = Quaternion.identity;
        newOption.transform.localScale = Vector3.one;

        UpgradeItemScript uiScript = newOption.GetComponent<UpgradeItemScript>();
        uiScript.AssociatedList = this;
        uiScript.SetUpgrade(Instantiate(targetUpgrade));
        uiScript.SetSource(SourceScreen);

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
            uiScript.SetTopTitle("Recommended");
        }
        else
        {
            uiScript.RemoveTopTitle();
        }
    }

    public void GenerateMultiUpgrade()
    {
        ItemList.Clear();
        MultiListUpgrades.RemoveAll(item => item == null || item.AssociatedUpgrade.UpgradeID == AssociatedUpgrade.UpgradeID);
        if (AssociatedUpgrades.Count == 0)
        {
            Destroy(gameObject);
            return;
        }
        else if (AssociatedUpgrades.Count == 1)
        {
            AssociatedUpgrade = AssociatedUpgrades[0];
            GenerateSingleUpgrade();
            return;
        }

        int itemCount = 0;
        foreach(UpgradesAbstract targetUpgrade in AssociatedUpgrades)
        {
            if (SourceScreen.PreboughtUpgradeIDs.Contains(targetUpgrade.UpgradeID)) continue;
            UpgradeItemScript uiScript = AddUpgradeToList(targetUpgrade);

            if(itemCount == 0)
            {
                uiScript.SetTopTitle("PICK ONE");
            } else
            {
                uiScript.SetTopTitle("OR");
            }
            itemCount++;
        }
        GetComponent<Image>().color = MultiColor;
        MultiListUpgrades.Add(this);
    }

    public void OnDisable()
    {
        MultiListUpgrades.Remove(this);
    }
    public void OnDestroy()
    {
        MultiListUpgrades.Remove(this);
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
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public IEnumerator UpgradeBoughtAnimation()
    {
        AssociatedUpgrade.AddToPurchasedList();
        AssociatedUpgrade.UpgradeBought = true;
        foreach (UpgradeItemScript targetUpgradeItem in ItemList)
        {
            targetUpgradeItem.ForceDisablePurchases();
        }

        Image image = GetComponent<Image>();
        Material runtimeMaterial = Instantiate(image.material);
        image.material = runtimeMaterial;

        float elapsed = 0f;
        while (elapsed < Duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / Duration);
            float progress = Mathf.Lerp(0.3f, 1f, t);
            image.materialForRendering.SetFloat("_Disappear", progress);
            yield return null;
        }

        // Ensure it ends at 1
        image.materialForRendering.SetFloat("_Disappear", 1f);

        MultiListUpgrades.Remove(this);
        RefreshMultiList();
        Destroy(gameObject);
    }
}
