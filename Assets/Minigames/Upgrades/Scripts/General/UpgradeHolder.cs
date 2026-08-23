using PixelCrushers;
using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UpgradeHolderSaver))]
public class UpgradeHolder : ActiveBroadcast
{
    public Minigame AssociatedMinigame;

    public bool AutoSubmit = false;

    public List<UpgradesAbstract> Upgrades;

    public bool Submitted = false;

    public static Dictionary<Minigame, List<UpgradeHolder>> AllUpgradeHolders = new Dictionary<Minigame, List<UpgradeHolder>>();

    public int Priority;

    public void Start()
    {
        if (!AllUpgradeHolders.ContainsKey(AssociatedMinigame)) AllUpgradeHolders[AssociatedMinigame] = new List<UpgradeHolder>();
        AllUpgradeHolders[AssociatedMinigame].Add(this);
        
        if (AutoSubmit)
        {
            SubmitUpgrades(false);
        } else
        {
            ActivationEvents.AddListener(SubmitUpgrades);
        }
    }

    public static void UnlockAll(Minigame TargetedMinigame)
    {
        foreach (UpgradeHolder holder in AllUpgradeHolders[TargetedMinigame])
        {
            holder.SubmitUpgrades();
        }
    }
    public void SubmitUpgrades()
    {
        SubmitUpgrades(true);
    }

    public void SubmitUpgrades(bool announce)
    {
        if (Submitted) return;
        foreach (UpgradesAbstract upgrade in Upgrades)
        {
            if(upgrade.PrioritySortOverride >= 0)
            {
                upgrade.Prioirty = upgrade.PrioritySortOverride;
            } else
            {
                upgrade.Prioirty = Priority;
            }
        }

        UpgradeScreenScript associatedScreen = UpgradeScreenScript.upgradeScreenScripts[AssociatedMinigame];
        if (announce)
        {
            string appName = associatedScreen.AssociatedApp.AppName;
            string previewText = $"<b>New Upgrades Available:</b>\nHead to <b>{appName}</b> to check them out!";
            AppScript targetApp = associatedScreen.AssociatedApp;

            AppNotificationScript.SetNotification(new AppNotificationScript.NotificationInfo
            {
                SourceApp = targetApp,
                PreviewImage = targetApp.AssociatedIcon,
                PreviewText = previewText,
                AdditionalActions = null
            });
        }

        associatedScreen.AddNewUpgrades(Upgrades, !AutoSubmit);
        Submitted = true;
    }
}
