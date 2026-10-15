using Coffee.UIExtensions;
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

    private bool ExhaustedSlot = false;

    public UIParticle PosParticles;
    public UIParticle NegParticles;
    public UIParticle NeuParticles;

    public TemporaryCardStorageScript PlayerCardStorage;
    public TemporaryCardStorageScript EnemyCardStorage;

    public bool isAtLimit()
    {
        return GetActiveCardCount() >= CardLimit;
    }

    public int GetActiveCardCount()
    {
        int activeCardCount = 0;
        foreach (GameCardScript card in _CardStack)
        {
            if (card.gameObject.activeSelf) activeCardCount++;
        }
        return activeCardCount;
    }

    public bool AddCardToStack(GameObject newCard, bool instantly = false)
    {
        if (isAtLimit()) return false;

        newCard.transform.SetParent(transform);
        newCard.transform.localPosition = Vector3.zero;
        newCard.transform.localRotation = Quaternion.identity;
        newCard.transform.localScale = Vector3.one;

        GameCardScript gameCardScript = newCard.GetComponent<GameCardScript>();

        CardStack.Add(gameCardScript);
        gameCardScript.CurrentSlot = this;

        if (PlaySlot)
        {
            gameCardScript.SetSettled(instantly);
            gameCardScript.SetPlaced(true);
            OnCardPlay?.Invoke(gameCardScript);

            if (!instantly)
            {
                if (gameCardScript.PlayerCard)
                {
                    PlayerCardStorage.StoreCard(newCard);
                }
                else
                {
                    EnemyCardStorage.StoreCard(newCard);
                }
            }
        }

        gameCardScript.ExhaustionUpdate(ExhaustedSlot);

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

    public GameCardScript GetActiveCard()
    {
        if(CardStack.Count <= 0) return null;
        for (int i = CardStack.Count - 1; i >= 0; i--)
        {
            if (CardStack[i].cardIsActive()) return CardStack[i];
        }

        return null;
    }

    public void SetExhausted(bool exhausted)
    {
        ExhaustedSlot = exhausted;

        foreach (GameCardScript card in CardStack)
        {
            card.ExhaustionUpdate(ExhaustedSlot);
        }
    }

    public void SettleCard(GameObject settleCard)
    {

        settleCard.GetComponent<GameCardScript>().SetSettled(true);
        settleCard.transform.SetParent(transform);
        settleCard.transform.localPosition = Vector3.zero;
        settleCard.SetActive(true);
    }

    public void LaunchParticles()
    {
        GameCardScript activeCard = GetActiveCard();

        if (activeCard == null) return;

        float changeValue = activeCard.GetOngoingValue();

        if(changeValue > 0)
        {
            ParticleHelper(PosParticles);
        } else if (changeValue < 0)
        {
            ParticleHelper(NegParticles);
        }
        else
        {
            ParticleHelper(NeuParticles);
        }
    }

    private void ParticleHelper(UIParticle uiPrefab)
    {
        GameObject newUIParticle = Instantiate(uiPrefab.gameObject);

        newUIParticle.transform.SetParent(uiPrefab.transform.parent);

        newUIParticle.transform.localPosition = Vector3.zero;
        newUIParticle.transform.localRotation = Quaternion.identity;
        newUIParticle.transform.localScale = uiPrefab.transform.localScale;

        newUIParticle.transform.SetParent(CardStockManager.instance.ParticleLayer, true);

        newUIParticle.GetComponentInChildren<UIParticle>().Play();
    }
}
