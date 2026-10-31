using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotScript : MonoBehaviour
{
    public Toggle ToggleDisplay;
    public int SlotID;

    public bool Autosave;
    public bool CompletedFile;

    public bool NewSaveSlot = false;

    public TMP_Text Description;

    public void Start()
    {
        Reset();
        UpdateSaveSlot();
    }

    public void UpdateSaveSlot()
    {
        SetSaveSlot(SlotID);
    }

    public void OnClick()
    {
        foreach(SaveSlotScript saveslot in SaveMenuScript.instance.MenuOwnedSaveSlots)
        {
            saveslot.Reset();
        }
        ToggleDisplay.isOn = true;

        SaveMenuScript.SelectedSlot = this;
        SaveMenuScript.NewSaveSelected = NewSaveSlot;
        SaveMenuScript.instance.SelectedSaveSlot = SlotID;
    }

    public void SetSaveSlot(int SlotNumber)
    {
        SlotID = SlotNumber;

        SaveMenuScript.SaveMetadata metaData = SaveMenuScript.GetMetaData(SlotNumber);
        if (metaData == null)
        {
            Description.text =
                $"{GetSlotName()}\n" +
                $"No Save";

            NewSaveSlot = true;

            RebuildLayouts();
            return;
        }
        NewSaveSlot = false;
        CompletedFile = metaData.DayComplete;

        if (metaData.DayComplete)
        {
            Description.text =
                $"{GetSlotName()}\n" +
                $"<b>DAY COMPLETED</b>";

            RebuildLayouts();
            return;
        }

        Description.text =
            $"{GetSlotName()}\n" +
            $"Time Played: {System.TimeSpan.FromSeconds(metaData.TimePlayed).ToString("h\\:mm\\:ss")}\n" +
            $"Credits: {GetAssociatedDayValue(metaData)}";

        RebuildLayouts();
    }

    private string GetSlotName()
    {
        if (Autosave) return "AUTOSAVE";

        return $"SLOT {SlotID}";
    }
    public string GetAssociatedDayValue(SaveMenuScript.SaveMetadata metaData)
    {
        if(SaveMenuScript.SelectedSaveDay == 1)
        {
            return metaData.Credits.ToString();
        }
        return "Debug Day";
    }
    public void RebuildLayouts()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(
                (RectTransform)Description.transform
            );

        LayoutRebuilder.ForceRebuildLayoutImmediate(
                (RectTransform)Description.transform.parent
            );

        LayoutRebuilder.ForceRebuildLayoutImmediate(
                (RectTransform)Description.transform.parent.parent
            );
    }

    public void Reset()
    {
        ToggleDisplay.isOn = false;
    }
}
