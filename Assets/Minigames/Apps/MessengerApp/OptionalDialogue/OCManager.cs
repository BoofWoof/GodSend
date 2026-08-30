using NUnit.Framework;
using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OCManager : MonoBehaviour
{
    public static OCManager instance;

    public List<OCSO> AvailableOC = new();
    public List<string> UsedUpOC = new();

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

    public void OnPurchase(OCSO purchasedOCSD)
    {
        Debug.Log($"Starting Optional Dialogue {purchasedOCSD.OCSDialogueName}");
        ConversationManagerScript.instance.StartDialogue(purchasedOCSD.OCSDialogueName);

        UsedUpOC.Add(purchasedOCSD.UniqueID);

        LocalCharacterInfo targetSpeaker = new LocalCharacterInfo().FromName(purchasedOCSD.AssociatedActor);
        ContactsScript.instance.CheckContacts(targetSpeaker);
        ContactsScript.instance.SwapToCharacterMessanger(targetSpeaker);

        gameObject.SetActive(false);
    }

    public void AddOC(OCSO newOC)
    {
        if(!AvailableOC.Contains(newOC)) AvailableOC.Add(newOC);
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

        foreach (OCSO OC in  AvailableOC)
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
