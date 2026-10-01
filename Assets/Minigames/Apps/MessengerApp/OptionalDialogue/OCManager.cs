using NUnit.Framework;
using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OCManager : MonoBehaviour
{
    public static OCManager instance;

    public List<OCSO> AvailableOC = new();
    public List<string> UsedUpOC = new();

    public UnityEvent NewDialogueAdded;

    public Transform ContentHolder;

    public GameObject OCItemPrefab;

    private int SecretNumber;

    public void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }


    public void OnEnable()
    {

        RefreshOptions();
    }

    public void OnPurchase(OCSO selectedOCSO)
    {
        Conversation AssociatedConversation = DialogueManager.masterDatabase.GetConversation(selectedOCSO.OCSDialogueName);

        bool isMacroConvo = Field.LookupBool(AssociatedConversation.fields, "IsMacro");

        Debug.Log($"Starting Optional Dialogue {selectedOCSO.OCSDialogueName}");
        ConversationManagerScript.instance.StartDialogue(selectedOCSO.OCSDialogueName);

        UsedUpOC.Add(selectedOCSO.UniqueID);

        if (isMacroConvo)
        {
            PhonePositionScript.instance.ForceTogglePhone();
            return;
        }

        LocalCharacterInfo targetSpeaker = new LocalCharacterInfo().FromName(selectedOCSO.AssociatedActor);
        ContactsScript.instance.CheckContacts(targetSpeaker);
        ContactsScript.instance.SwapToCharacterMessanger(targetSpeaker);

        gameObject.SetActive(false);
    }

    public void AddOC(OCSO newOC)
    {
        if(!AvailableOC.Contains(newOC)) AvailableOC.Add(newOC);

        NewDialogueAdded?.Invoke();

        RefreshOptions();
    }

    public void RefreshOptions()
    {
        ClearOptions();
        GenerateOptions();
    }

    public void ClearOptions()
    {
        foreach (Transform child in ContentHolder)
        {
            Destroy(child.gameObject);
        }
    }

    public void GenerateOptions()
    {
        List<OCSO> UpdatedList = new();

        foreach (OCSO OC in AvailableOC)
        {
            if (UsedUpOC.Contains(OC.UniqueID)) continue;
            UpdatedList.Add(OC);

            GameObject newOCItem = Instantiate(OCItemPrefab);
            Transform ocT = newOCItem.transform;
            ocT.SetParent(ContentHolder);
            ocT.localPosition = Vector3.zero;
            ocT.localRotation = Quaternion.identity;
            ocT.localScale = Vector3.one;

            OCItemScript OCscript = ocT.GetComponent<OCItemScript>();
            OCscript.AssignOCSO(OC);
            OCscript.OnPurchase += OnPurchase;
        }

        AvailableOC = UpdatedList;

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)ContentHolder);
    }

    public void SetSecretNumber(string UpdatedSecretNumber)
    {
        SecretNumber = int.Parse(UpdatedSecretNumber);
    }

    public void SubmitSecretNumber()
    {
        SecretOCTriggerScript.SubmitPhoneNumber(SecretNumber);
    }
}
