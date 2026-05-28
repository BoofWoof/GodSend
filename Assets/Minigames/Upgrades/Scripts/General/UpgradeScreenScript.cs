using UnityEngine;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine.UI;
using System.Collections;

public class UpgradeScreenScript : MonoBehaviour
{
    public Sprite NotificationSprite;

    public Minigame AssociatedMinigame;
    public Dictionary<Minigame, string> MinigameToString = new Dictionary<Minigame, string>()
    {
        {Minigame.Visions, "vision"},
        {Minigame.Shoppr, "shoppr"}
    };

    public AudioSource UpgradeAudio;

    public List<UpgradesAbstract> UpgradeClones;
    public List<GameObject> UpgradeObjects;

    public GameObject UpgradeItemPrefab;

    public RectTransform ContentHolder;

    [HideInInspector] public int DisplayedUpgrades = 0;

    public delegate void UpgradeBoughtDelegate(UpgradesAbstract upgrade);
    public static UpgradeBoughtDelegate UpgradeBoughtEvent;

    public static Dictionary<Minigame, UpgradeScreenScript> upgradeScreenScripts = new Dictionary<Minigame, UpgradeScreenScript>();

    public GameObject ProgressToUnlockUpgradesText;

    public static bool WaitToOpen = false;

    public List<string> PreboughtUpgradeIDs = new List<string>();

    public bool StartActive = false;

    private bool EnableShopprFilter;
    private ShopprTags ShopprFilter;
    private bool EnableVisionFilter;
    private VisionTags VisionFilter;

    public void Awake()
    {
        upgradeScreenScripts[AssociatedMinigame] = this;
        Lua.RegisterFunction("UpgradeWait", null, SymbolExtensions.GetMethodInfo(() => EnableWaitTrigger()));

        gameObject.SetActive(StartActive);
    }

    public void PreBuyUpgrades(List<string> UpgradeIDList)
    {
        PreboughtUpgradeIDs = UpgradeIDList;
        CheckForPrebought();
    }

    private void CheckForPrebought()
    {
        foreach (UpgradesAbstract upgrade in UpgradeClones)
        {
            if (upgrade.UpgradeBought) continue;
            if (PreboughtUpgradeIDs.Contains(upgrade.UpgradeID)) upgrade.LoadBuy();
        }
    }

    public bool UpgradeAffordable()
    {
        foreach(UpgradesAbstract upgrade in UpgradeClones)
        {
            if (upgrade.CanBuy()) return true;
        }
        return false;
    }

    public void AddNewUpgrades(List<UpgradesAbstract> newUpgrades, bool showNotification = true)
    {
        if(showNotification) NotificationMenuScript.SetNotification(MinigameToString[AssociatedMinigame], NotificationSprite);

        List<UpgradesAbstract> newUpgradeClones = new List<UpgradesAbstract>();
        foreach (UpgradesAbstract upgrade in newUpgrades)
        {
            newUpgradeClones.Add(Instantiate(upgrade));
        }
        foreach (UpgradesAbstract upgrade in newUpgradeClones)
        {
            if (upgrade.AutoBuy) upgrade.Buy(true);
        }

        //Find which index to add to.
        for(int i = newUpgradeClones.Count - 1; i >= 0; i--)
        {
            UpgradesAbstract newUpgrade = newUpgradeClones[i];
            int newPriority = newUpgrade.Prioirty;
            int insertionIndex = 0;
            foreach (UpgradesAbstract upgrade in UpgradeClones)
            {
                if (newPriority <= upgrade.Prioirty) break;
                insertionIndex++;
            }
            UpgradeClones.Insert(insertionIndex, newUpgrade);
        }

        CheckForPrebought();

        if(gameObject.activeInHierarchy) Refresh();
    }

    public void OnEnable()
    {
        if(WaitToOpen) Sequencer.Message("FinishedSpeaking"); // (DialogueManager.dialogueUI as AbstractDialogueUI).OnContinueConversation();
        UpgradeBoughtEvent += UpgradeAudioPlay;
        UpgradeBoughtEvent += RecordUpgradeBought;
        Refresh();

        NotificationMenuScript.ReleaseNotification(MinigameToString[AssociatedMinigame]);
    }
    public void OnDisable()
    {
        UpgradeBoughtEvent -= UpgradeAudioPlay;
        UpgradeBoughtEvent -= RecordUpgradeBought;

    }

    public static void EnableWaitTrigger()
    {
        WaitToOpen = true;
    }

    public void UpgradeAudioPlay(UpgradesAbstract upgrade)
    {
        if (upgrade.AssociatedMinigame != AssociatedMinigame) return;
        UpgradeAudio.Play();
    }

    public void RecordUpgradeBought(UpgradesAbstract upgrade)
    {
        if (upgrade.AssociatedMinigame != AssociatedMinigame) return;
        string newID = upgrade.UpgradeID;
        if(!PreboughtUpgradeIDs.Contains(newID)) PreboughtUpgradeIDs.Add(newID);
    }

    public void FullGenerate()
    {
        foreach (UpgradesAbstract upgrade in UpgradeClones)
        {
            if (upgrade.UpgradeBought) continue;
            AddUpgrade(upgrade);
        }
        ProgressToUnlockUpgradesText?.SetActive(DisplayedUpgrades == 0);

        StartCoroutine(RebuildLayout());
        //LayoutRebuilder.ForceRebuildLayoutImmediate(ContentHolder);
    }
    public void BoughtGenerate()
    {
        foreach (UpgradesAbstract upgrade in UpgradeClones)
        {
            if (!upgrade.UpgradeBought) continue;
            AddUpgrade(upgrade);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(ContentHolder);
        StartCoroutine(RebuildLayout());
    }

    public void Clear()
    {
        foreach (Transform upgradeObject in ContentHolder.transform)
        {
            Destroy(upgradeObject.gameObject);
        }
        DisplayedUpgrades = 0;
    }

    public void Refresh()
    {
        Clear();
        FullGenerate();
    }
    public void BoughtRefresh()
    {
        Clear();
        BoughtGenerate();
    }

    public void AddUpgrade(UpgradesAbstract newUpgrade)
    {
        if (EnableVisionFilter && !newUpgrade.VTags.Contains(VisionFilter)) return;
        if (EnableShopprFilter && !newUpgrade.STags.Contains(ShopprFilter)) return;

        DisplayedUpgrades++;

        GameObject newUpgradeObject = Instantiate(UpgradeItemPrefab, ContentHolder);

        UpgradeItemListScript ulScript = newUpgradeObject.GetComponent<UpgradeItemListScript>();
        if(ulScript == null) ulScript = newUpgradeObject.GetComponentInChildren<UpgradeItemListScript>();
        ulScript.SetSource(this);
        if (newUpgrade.UpgradesGroup == null || newUpgrade.UpgradesGroup.Count == 0)
        {
            newUpgrade.AssociatedMinigame = AssociatedMinigame;
            ulScript.SetUpgrade(newUpgrade);
        } else
        {
            foreach (UpgradesAbstract upgrade in newUpgrade.UpgradesGroup)
            {
                upgrade.AssociatedMinigame = AssociatedMinigame;
            }
            ulScript.AssociatedUpgrade = newUpgrade;
            ulScript.SetUpgrade(newUpgrade.UpgradesGroup);
        }

        RectTransform rect = newUpgradeObject.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    public IEnumerator RebuildLayout()
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(ContentHolder);
    }

    public void SetVisionsFilter(int enumIdx)
    {
        VisionTags newTag = (VisionTags)enumIdx;
        EnableVisionFilter = true;
        VisionFilter = newTag;
        Refresh();
    }

    public void SetShopprFilter(int enumIdx)
    {
        ShopprTags newTag = (ShopprTags)enumIdx;
        EnableShopprFilter = true;
        ShopprFilter = newTag;
        Refresh();
    }

    public void ResetFilters()
    {
        EnableShopprFilter = false;
        EnableVisionFilter = false;
        Refresh();
    }
}
