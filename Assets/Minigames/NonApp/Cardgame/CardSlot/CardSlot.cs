using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private List<GameCardScript> _CardStack = new();
    public List<GameCardScript> CardStack
    {
        get { return _CardStack; }
        set {
            _CardStack = value;
            OnCardStackChange?.Invoke(value);
        }
    }
    public Action<List<GameCardScript>> OnCardStackChange;
    public Action<GameCardScript> OnCardPlay;

    public int CardLimit = 1; //Set to negative one if no card limit.

    public static CardSlot HoveredSlot;

    public bool PlaySlot = false;

    public bool isAtLimit()
    {
        return CardStack.Count >= CardLimit;
    }

    public bool AddCardToStack(GameObject newCard)
    {
        if (isAtLimit()) return false;

        newCard.transform.SetParent(transform);
        newCard.transform.localPosition = Vector3.zero;
        newCard.transform.localRotation = Quaternion.identity;
        newCard.transform.localScale = Vector3.one;

        GameCardScript gameCardScript = newCard.GetComponent<GameCardScript>();

        CardStack.Add(gameCardScript);
        gameCardScript.CurrentSlot = this;

        if (PlaySlot) OnCardPlay?.Invoke(gameCardScript);

        return true;
    }

    public bool RemoveCardFromStack(GameObject newCard)
    {
        GameCardScript gameCardScript = newCard.GetComponent<GameCardScript>();

        if (!CardStack.Contains(gameCardScript)) return false;
        CardStack.Remove(gameCardScript);
        return true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoveredSlot = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (HoveredSlot != this) return;
        HoveredSlot = null;
    }
}
