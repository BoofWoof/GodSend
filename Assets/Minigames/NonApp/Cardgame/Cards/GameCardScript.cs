using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameCardScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public CardSO CardData;

    public TMP_Text Title;
    public TMP_Text TimeCost;
    public Image CardImage;
    public TMP_Text CardType;
    public TMP_Text Description;

    private RectTransform ThisRec;

    private Canvas CurrentCanvas;
    public CardSlot CurrentSlot;

    private bool PickedUp;

    public void Start()
    {
        ThisRec = GetComponent<RectTransform>();
        CurrentCanvas = GetComponentInParent<Canvas>();
        CurrentSlot = GetComponentInParent<CardSlot>();
    }

    public void LateUpdate()
    {
        if (!PickedUp) return;

        Canvas pointedCanvas = CanvasHelper.GetHoveredCanvas(Input.mousePosition);
        if (pointedCanvas != null && pointedCanvas != CurrentCanvas)
        {
            CurrentCanvas = pointedCanvas;
            ParentToCanvasScreen();
        }

        Camera worldCamera = CurrentCanvas.worldCamera != null
            ? CurrentCanvas.worldCamera
            : Camera.main;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            CurrentCanvas.transform as RectTransform,
            Input.mousePosition,
            worldCamera,
            out Vector2 localPoint))
        {
            ThisRec.localPosition = localPoint;
        }
    }

    public void ParentToCanvasScreen()
    {
        CardScreenScript targetScreen = CurrentCanvas.GetComponent<CardScreenScript>();
        if (targetScreen != null)
        {
            transform.SetParent(targetScreen.CardParentTarget, false);
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one * targetScreen.OnScreenScale;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        PickedUp = true;
        Debug.Log($"{CardData.CardName} is picked up.");

        GetComponent<CanvasGroup>().blocksRaycasts = false;

        CurrentSlot.RemoveCardFromStack(gameObject);

        ParentToCanvasScreen();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        PickedUp = false;
        Debug.Log($"{CardData.CardName} is dropped.");

        GetComponent<CanvasGroup>().blocksRaycasts = true;

        if(CardSlot.HoveredSlot != null)
        {
            if (!CardSlot.HoveredSlot.AddCardToStack(gameObject))
            {
                CurrentSlot.AddCardToStack(gameObject);
            }
        } else
        {
            CurrentSlot.AddCardToStack(gameObject);
        }
    }

    public void SetCardData(CardSO cardData)
    {
        CardData = cardData;
        UpdateCardUI();
    }

    [ContextMenu("Update Card")]
    public void UpdateCardUI()
    {
        Title.text = CardData.CardName;
        TimeCost.text = CardData.TimeCost.ToString();
        CardImage.sprite = CardData.CardArt;
        CardType.text = CardData.CardType.ToString();
        Description.text = CardData.Description;
    }
}
