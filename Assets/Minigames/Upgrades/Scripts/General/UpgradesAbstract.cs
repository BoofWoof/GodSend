using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum Minigame
{
    Visions,
    Stocks,
    Rally,
    Hacking,
    Shoppr
}

public enum VisionTags
{
    Income,
    Survival,
    Tools,
    Decorations
}
public enum ShopprTags
{
    Tribute,
    City,
    Defense,
    Gifts,
    Misc
}

[Serializable]
public class MascotDialogueOverride
{
    public int OverrideDay = -1;
    [TextArea]
    public string ReplacementDialogue;
    public bool triggerToday()
    {
        return OverrideDay == DayInfo.CurrentDay;
    }
}

public abstract class UpgradesAbstract : ScriptableObject
{
    [HideInInspector] public int Prioirty;

    public string UpgradeID;

    //This is only used for upgrade options.
    public List<UpgradesAbstract> UpgradesGroup;

    public string UpgradeName;
    public Sprite UpgradeIcon;

    public int PrioritySortOverride = -1;

    public bool GoldenUpgrade = false;

    public int DayToTrigger = -1;
    public string DialogueToTrigger = "";
    public bool CompleteQuest = false;
    public bool ProgressQuest = false;

    [TextArea]
    public string UpgradeDescription;
    [TextArea]
    public string UpgradeMechanicDescription;
    [TextArea]
    public string MascotDialogue;
    public List<MascotDialogueOverride> OverrideDialogue = new List<MascotDialogueOverride>();

    [Header("Costs")]
    public float Credits;
    public float FlockRenown;
    public float FoundationRenown;
    public float AssscensssionRenown;
    public float RevolutionRenown;

    public string Tags;

    public bool UpgradeBought = false;

    public Minigame AssociatedMinigame;

    public bool AutoBuy = false;

    public bool TriggerOnLoadBuy = true;

    public List<VisionTags> VTags;
    public List<ShopprTags> STags;

    public bool CloseMenuOnBuy = false;

    public bool CanBuy()
    {
        if (UpgradeBought) return false;
        if (AutoBuy) return false;

        if (UpgradesGroup.Count > 0)
        {
            foreach (UpgradesAbstract subUpgrade in UpgradesGroup)
            {
                if (subUpgrade.CanBuy())
                {
                    return true;
                }
            }
            return false;
        }

        if (CurrencyData.Credits < Credits) return false;
        if (CurrencyData.RenownFlock < FlockRenown) return false;
        if (CurrencyData.RenownFoundation < FoundationRenown) return false;
        if (CurrencyData.RenownAscension < AssscensssionRenown) return false;
        if (CurrencyData.RenownRevolution < RevolutionRenown) return false;
        return true;
    }

    public void LoadBuy()
    {
        if (UpgradeBought) return;
        if (TriggerOnLoadBuy) OnBuy();
        UpgradeBought = true;
    }

    public bool Buy(bool forceBuy = false)
    {
        if (UpgradeBought) return false;
        bool canBuy = CanBuy();
        if (!canBuy && !forceBuy) return canBuy;
        UpgradeBought = true;

        CurrencyData.Credits -= Credits;
        CurrencyData.RenownFlock -= FlockRenown;
        CurrencyData.RenownFoundation -= FoundationRenown;
        CurrencyData.RenownAscension -= AssscensssionRenown;
        CurrencyData.RenownRevolution -= RevolutionRenown;

        OnBuy();

        if(AssociatedMinigame == Minigame.Visions) {
            string sayDialogue = MascotDialogue;
            foreach(MascotDialogueOverride mdo in OverrideDialogue)
            {
                if (mdo.triggerToday())
                {
                    sayDialogue = mdo.ReplacementDialogue;
                    break;
                }
            }
            VisionMascotScript.SayText(sayDialogue);
        }

        if(CloseMenuOnBuy) UpgradeScreenScript.upgradeScreenScripts[AssociatedMinigame].gameObject.SetActive(false);

        if (!forceBuy) AddToPurchasedList();

        bool triggerDay = DayToTrigger == DayInfo.CurrentDay;
        if (DialogueToTrigger.Length > 0 && triggerDay) MessageQueue.addDialogue(DialogueToTrigger);
        if (CompleteQuest && triggerDay) QuestManager.CompleteQuest(QuestManager.currentQuest);
        if (ProgressQuest && triggerDay) QuestManager.IncrementQuest();

        return canBuy;
    }

    public void AddToPurchasedList()
    {
        UpgradeScreenScript.UpgradeBoughtEvent?.Invoke(this);
    }

    

    public string CostToText()
    {
        int costTypes = 0;
        string outputText = "";

        if(Credits > 0)
        {
            outputText += "<sprite index=1 tint=1> " + Credits.NumberToString();
            costTypes++;
        }
        if(FlockRenown > 0)
        {
            if (costTypes > 0) outputText += "\n";
            outputText += "<sprite index=0 tint=1> " + FlockRenown.NumberToString();
            costTypes++;
        }
        if(FoundationRenown > 0)
        {
            if (costTypes > 0) outputText += "\n";
            outputText += "<sprite index=5 tint=1> " + FoundationRenown.NumberToString();
            costTypes++;
        }
        if(AssscensssionRenown > 0)
        {
            if (costTypes > 0) outputText += "\n";
            outputText += "<sprite index=2 tint=1> " + AssscensssionRenown.NumberToString();
            costTypes++;
        }
        if(RevolutionRenown > 0)
        {
            if (costTypes > 0) outputText += "\n";
            outputText += "<sprite index=4 tint=1> " + RevolutionRenown.NumberToString();
        }

        if (outputText.Length <= 1) return "<sprite index=1> 0";

        return outputText;
    }

    public abstract void OnBuy();
}
