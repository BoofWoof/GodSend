using PixelCrushers.DialogueSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OCItemScript : MonoBehaviour
{
    private OCSO AssignedOC;

    public Image PFP;
    public TMP_Text Title;
    public TMP_Text Description;
    public TMP_Text Availability;
    public Button StartButton;

    public Color AvailableColor;
    public Color NotAvailableColor;

    public delegate void OnPurchaseDelegate(OCSO thisOC);
    public OnPurchaseDelegate OnPurchase;

    public void SubmitConversation()
    {
        if (!UpdateAvailability()) return;
        OnPurchase?.Invoke(AssignedOC);
        Destroy(gameObject);
    }

    public void AssignOCSO(OCSO newOCSO)
    {
        AssignedOC = newOCSO;
        UpdateData();
    }

    public void UpdateData()
    {
        PFP.sprite = DialogueManager.masterDatabase.GetActor(AssignedOC.AssociatedActor).spritePortrait;
        Title.text = AssignedOC.OCSName;
        Description.text = AssignedOC.OCSDescription;
    }

    public bool UpdateAvailability()
    {
        OCSO.OCAvailability availability = AssignedOC.CheckAvailability();

        switch(availability)
        {
            case OCSO.OCAvailability.Available:
                {
                    StartButton.interactable = true;
                    Availability.color = AvailableColor;
                    Availability.text = "AVAILABLE";
                    return true;
                }
            case OCSO.OCAvailability.DangerActive:
                {
                    StartButton.interactable = false;
                    Availability.color = NotAvailableColor;
                    Availability.text = "Not Available: Clear Current Danger";
                    return false;
                }
            case OCSO.OCAvailability.EventActive:
                {
                    StartButton.interactable = false;
                    Availability.color = NotAvailableColor;
                    Availability.text = "Not Available: Complete Current Challenge";
                    return false;
                }
            case OCSO.OCAvailability.DialogueActive:
                {
                    StartButton.interactable = false;
                    Availability.color = NotAvailableColor;
                    Availability.text = "Not Available: Finish Current Dialogue";
                    return false;
                }
        }
        return false;
    }
}
