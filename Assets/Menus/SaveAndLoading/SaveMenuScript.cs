using PixelCrushers;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveMenuScript : MonoBehaviour
{
    public static int SelectedSaveDay = 1;

    public static SaveMenuScript instance;

    private static int _SelectedSaveSlotIdx = -1;
    public int SelectedSaveSlot
    {
        get { return _SelectedSaveSlotIdx; }
        set {
            _SelectedSaveSlotIdx = value; 
            if(value >= 0)
            {
                if (loadButton != null) loadButton.interactable = !NewSaveSelected && !SelectedSlot.CompletedFile;
                if (deleteButton != null) deleteButton.interactable = !NewSaveSelected;
                SaveSelected = true;
            }
        }
    }
    public static SaveSlotScript SelectedSlot;
    public static bool NewSaveSelected = false;
    public bool SaveSelected = false;

    public bool MainMenu = false;

    public bool TrueSaveLoadFalse = true;

    public Transform ContentHolder;

    public GameObject SaveDataWarning;

    public TMP_Text ErrorText;

    [Header("Buttons")]
    public Button saveButton;
    public Button loadButton;
    public Button deleteButton;

    public static bool ForceAllowSaves = false;

    public SaveSlotScript[] MenuOwnedSaveSlots;

    private static bool _CompleteSave = false;

    public Action OnSaveTrigger;

    [Serializable]
    public class SaveMetadata
    {
        public int Slot;
        public int Day;
        public float TimePlayed;
        public double Credits;
        public bool DayComplete = false;
    }

    void Start()
    {
        instance = this;

        MainMenu = SceneManager.GetActiveScene().name == "MainMenu";
        if (MainMenu) Destroy(saveButton.gameObject);

        Reset();
    }

    public static void AutoSave()
    {
        Debug.Log("Autosaving!!!");
        Save(0);
    }

    public void OnEnable()
    {
        SaveSystem.saveEnded += OnSaveEnd;
    }

    public void OnDisable()
    {
        SaveSystem.saveEnded -= OnSaveEnd;
        _CompleteSave = false;
    }

    public void OnDestroy()
    {
        instance = null;
    }

    public void SetCompletitionMenu()
    {
        _CompleteSave = true;
    }
    public void Update()
    {
        if (!SaveSelected)
        {
            if (MainMenu) ErrorText.text = "Select a save slot to load/delete.";
            else ErrorText.text = "Select a save slot to save/load/delete.";
            if (saveButton != null) saveButton.interactable = false;
            return;
        }

        if (MenuOwnedSaveSlots[SelectedSaveSlot].Autosave)
        {
            ErrorText.text = "";
            saveButton.interactable = false;
        }

        if (MainMenu)
        {
            ErrorText.text = "";
            if (saveButton != null) saveButton.interactable = false;
            return;
        }

        if (ForceAllowSaves)
        {
            ErrorText.text = "";
            saveButton.interactable = true;
            return;
        }

        if (GameStateMonitor.isEventActive() || ConversationManagerScript.ConversationOngoing || MessageQueue.GetQueueLength() > 0)
        {
            ErrorText.text = "You must finish your current dialogues before saving.";
            saveButton.interactable = false;
            return;
        }
        else if (!PrayerScript.instance.JudgementActive)
        {
            ErrorText.text = "<b>THE GREAT RAM</b> must be judging you before you can save.";
            saveButton.interactable = false;
            return;
        }
        else
        {
            ErrorText.text = "";
            saveButton.interactable = true;
        }
    }

    public void Reset()
    {
        SelectedSaveSlot = -1;

        if (saveButton != null) saveButton.interactable = false;
        loadButton.interactable = false;

        deleteButton.interactable = false;
        SaveSelected = false;

        //ShowExistingSaveFiles();
    }

    public static bool SaveFileExists()
    {
        for (int i = 0; i < 25; i++)
        {
            if (SaveSystem.HasSavedGameInSlot(i))
            {
                return true;
            }
        }
        return false;
    }

    /*
    public void ShowExistingSaveFiles()
    {
        int EmptySlot = -1;
        for (int i = 0; i < 25; i++)
        {
            if (!SaveSystem.HasSavedGameInSlot(i)) {
                if(EmptySlot < 0) EmptySlot = i;
                continue;
            }

            GameObject newSlot = Instantiate(SlotPrefab, ContentHolder);
            newSlot.transform.localPosition = Vector3.zero;
            newSlot.transform.localRotation = Quaternion.identity;
            newSlot.transform.localScale = Vector3.one;

            newSlot.GetComponent<SaveSlotScript>().SetSaveSlot(i);
        }

        if (EmptySlot >= 0 && TrueSaveLoadFalse)
        {
            GameObject newSlot = Instantiate(SlotPrefab, ContentHolder);
            newSlot.transform.localPosition = Vector3.zero;
            newSlot.transform.localRotation = Quaternion.identity;
            newSlot.transform.localScale = Vector3.one;

            newSlot.GetComponent<SaveSlotScript>().SetNewSaveSlot(EmptySlot);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(
                (RectTransform)ContentHolder.transform
            );
    }
    */

    public void SaveAttempt()
    {
        if (SelectedSlot != null && SelectedSlot.Autosave) return;
        if (NewSaveSelected)
        {
            Save(SelectedSaveSlot);
            return;
        }
        SaveDataWarning.SetActive(true);
    }

    static public void Save(int SaveSlot)
    {
        Debug.Log($"Saving your game for slot: {SaveSlot}");

        SaveMetadata metadata = new SaveMetadata
        {
            Slot = SaveSlot,
            Day = DayInfo.CurrentDay,
            TimePlayed = EndOfDayScript.GetTimePassed(),
            Credits = CurrencyData.Credits,
            DayComplete = _CompleteSave
        };

        string metaDataSerialized = SaveSystem.Serialize(metadata);

        string path = Application.persistentDataPath + "/slot_" + SaveSlot + ".meta";
        System.IO.File.WriteAllText(path, metaDataSerialized);

        SaveSystem.SaveToSlot(SaveSlot);

        if (instance != null) instance.SaveTriggers();
    }

    private void SaveTriggers()
    {
        if (saveButton != null) saveButton.interactable = false;
        deleteButton.interactable = false;

        UpdateAllSaveSlots();

        OnSaveTrigger?.Invoke();

        SaveSelected = false;
    }

    public void UpdateAllSaveSlots()
    {
        foreach (SaveSlotScript saveSlot in MenuOwnedSaveSlots)
        {
            saveSlot.UpdateSaveSlot();
            saveSlot.Reset();
        }
    }

    public void OnSaveEnd()
    {
        Reset();
    }


    public static SaveMetadata GetSelectedMetaData()
    {
        return GetMetaData(instance.SelectedSaveSlot);
    }

    public static SaveMetadata GetMetaData(int SlotIdx)
    {
        string path = Application.persistentDataPath + "/slot_" + SlotIdx + ".meta";

        if (!System.IO.File.Exists(path))
            return null;

        string metaDataSerialized = System.IO.File.ReadAllText(path);

        SaveMetadata metadata = SaveSystem.Deserialize<SaveMetadata>(metaDataSerialized);

        return metadata;
    }
    public void LoadGame()
    {
        Physics.gravity = Vector3.down * 9.8f;
        DaytaScript.ExternalSkipStart = true;
        DayInfo.SetDay(GetSelectedMetaData().Day);
        SaveSystem.LoadFromSlot(SelectedSaveSlot);
    }

    public void DeleteSave()
    {
        if (!SaveSystem.HasSavedGameInSlot(SelectedSaveSlot)) return;
        SaveSystem.DeleteSavedGameInSlot(SelectedSaveSlot);

        string path = Application.persistentDataPath + "/slot_" + SelectedSaveSlot + ".meta";
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }

        Reset();
        UpdateAllSaveSlots();
    }

    public static void ToggleForceAllowSaves()
    {
        ForceAllowSaves = !ForceAllowSaves;
    }
}
