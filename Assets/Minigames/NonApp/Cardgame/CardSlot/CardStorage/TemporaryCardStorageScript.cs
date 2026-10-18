using UnityEngine;
using UnityEngine.EventSystems;

public class TemporaryCardStorageScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{


    private GameObject HeldCard = null;
    public GameObject WaitGraphic;

    public bool ReleaseOnRoundEnd;
    public bool ReleaseOnSettle;

    public CardSlot AssociatedSlot;

    public void Start()
    {
        UpdateWaitVisibility();

        if (ReleaseOnRoundEnd) CardTimeManager.OnRoundEnd += SendToSlot;
        if (ReleaseOnSettle) CardTimeManager.OnTimeEnd += SendToSlot;
    }

    public void UpdateWaitVisibility()
    {
        gameObject.SetActive(isCardHeld());
        bool AnyActiveChildren = false;
        foreach (Transform child in transform.parent)
        {
            if (child.gameObject.activeSelf)
            {
                AnyActiveChildren = true;
            }
        }
        WaitGraphic.SetActive(AnyActiveChildren);
    }
    public bool isCardHeld()
    {
        return HeldCard != null;
    }

    public void StoreCard(GameObject targetCard)
    {
        targetCard.SetActive(false);
        HeldCard = targetCard;
        HeldCard.GetComponent<CanvasGroup>().interactable = false;
        UpdateWaitVisibility();

        RectTransform rectTransform = targetCard.GetComponent<RectTransform>();
        rectTransform.SetParent(transform);
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localRotation = Quaternion.identity;
    }

    public GameObject RetrieveCard()
    {
        GameObject tempCard = HeldCard;
        HeldCard.GetComponent<CanvasGroup>().interactable = false;
        HeldCard = null;
        UpdateWaitVisibility();
        return tempCard;
    }

    public void SendToSlot(int round)
    {
        if (!isCardHeld()) return;

        AssociatedSlot.SettleCard(HeldCard);
        HeldCard = null;
        UpdateWaitVisibility();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isCardHeld()) return;
        if (ReleaseOnSettle) return;
        HeldCard.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isCardHeld()) return;
        if (ReleaseOnSettle) return;
        HeldCard.SetActive(false);
    }
}
