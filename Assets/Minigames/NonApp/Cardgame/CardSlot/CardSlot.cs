using Coffee.UIExtensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    public int CardLimit = 1; //Set to negative one if no card limit.

    public static CardSlot HoveredSlot;

    public bool PlaySlot = false;

    private bool ExhaustedSlot = false;

    public UIParticle PosParticles;
    public UIParticle NegParticles;
    public UIParticle NeuParticles;

    public TemporaryCardStorageScript PlayerCardStorage;
    public TemporaryCardStorageScript EnemyCardStorage;

    public GameObject CardHighlight;
    private Image _HighlightImage;
    private Color _HighlightColor;

    public static Action<bool, CardSlot> SlotHovered;
    private bool Hovered = false;

    public static Action<CardSlot, GameCardScript> OnCardPlayedInSlot;
    public static Action<CardSlot, GameCardScript> OnCardSettledInSlot;

    private static List<CardSlot> PlaySlotList = new();

    public void Start()
    {
        _HighlightImage = CardHighlight.GetComponent<Image>();
        _HighlightColor = _HighlightImage.color;
        HighlightCard(false, null);

        if (PlaySlot)
        {
            PlaySlotList.Add(this);
        }
    }

    public void OnDestroy()
    {
        if (PlaySlot) PlaySlotList.Remove(this);
    }

    public void OnEnable()
    {
        GameCardScript.CardPickedUp += HighlightCard;
    }

    public void OnDisable()
    {
        GameCardScript.CardPickedUp -= HighlightCard;
    }

    public bool isAtLimit()
    {
        return GetActiveCardCount() >= CardLimit;
    }

    public bool isValidPlaySpace()
    {
        return !isAtLimit();
    }

    public static CardSlot GetSlot(int index)
    {
        if (index >= PlaySlotList.Count) return default;
        return PlaySlotList[index];
    }

    public static int GetSlotCount()
    {
        return PlaySlotList.Count;
    }

    public void HighlightCard(bool showGlow, GameCardScript _)
    {
        if (!PlaySlot && showGlow) return;
        if (CardHighlight == null) return;

        CardHighlight.SetActive(showGlow && isValidPlaySpace());
        _HighlightImage.color = _HighlightColor;
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
        if (!isValidPlaySpace()) return false;

        newCard.transform.SetParent(transform);
        newCard.transform.localPosition = Vector3.zero;
        newCard.transform.localRotation = Quaternion.identity;
        newCard.transform.localScale = Vector3.one;

        GameCardScript gameCardScript = newCard.GetComponent<GameCardScript>();

        CardStack.Add(gameCardScript);
        gameCardScript.CurrentSlot = this;

        if (PlaySlot)
        {
            OnCardPlayedInSlot?.Invoke(this, gameCardScript);
            gameCardScript.SetSettled(false);
            gameCardScript.SetPlaced(true);

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
            } else
            {
                SettleCard(newCard);
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
        Hovered = true;
        SlotHovered?.Invoke(true, this);

        HoveredSlot = this;
        if (_HighlightImage != null) _HighlightImage.color = new Color(1, 1, 1, 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Hovered = false;
        SlotHovered?.Invoke(false, this);

        if (HoveredSlot != this) return;
        HoveredSlot = null;
        if (_HighlightImage != null) _HighlightImage.color = _HighlightColor;
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
    
    public GameCardScript GetPredictionCard()
    {
        if (Hovered && GameCardScript.CurrentHeldCard != null) return GameCardScript.CurrentHeldCard;
        else return GetActiveCard();
    }

    public void AcitvateCard()
    {
        SetExhausted(true);
        LaunchParticles();
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
        GameCardScript gameCardScript = settleCard.GetComponent<GameCardScript>();
        gameCardScript.SetSettled(true);
        settleCard.transform.SetParent(transform);
        settleCard.transform.localPosition = Vector3.zero;
        settleCard.SetActive(true);


        OnCardSettledInSlot?.Invoke(this, gameCardScript);
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
