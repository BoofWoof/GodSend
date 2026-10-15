using System;
using UnityEngine;

public class CardStockManager : MonoBehaviour
{
    public static CardStockManager instance;

    private static DeckSO _Deck;
    public static DeckSO Deck
    {
        get { return _Deck; }
        set { 
            _Deck = value; 
            OnDeckChange?.Invoke(value);
        }
    }
    public DeckSO StartingDeck;
    public static Action<DeckSO> OnDeckChange;

    public HandManagerScript HandManager;

    [SerializeField] private GameObject CardGameCanvas;
    public static Action<bool> CanvasStateChange;

    public Transform ParticleLayer;

    private StockCardLevelSO _Level;

    public GraphManagerScript GraphManager;

    public PlaySlotGenerator SlotManager;

    public CardMakerScript CardMaker;
    public void Awake()
    {
        instance = this;
        Deck = Instantiate(StartingDeck);
        Deck.ShuffleDeck();
        TurnOff();
    }

    public void StartLevel(StockCardLevelSO targetLevel)
    {
        _Level = targetLevel;

        StockCardLevelSO.PhaseData phaseOne = _Level.Phases[0];

        SlotManager.ClearRow();

        StartPhase(phaseOne, true);


        StockCardLevelSO.PhaseData phaseTwo = _Level.Phases[1];
        StartPhase(phaseTwo, false);
    }

    public void StartPhase(StockCardLevelSO.PhaseData phaseData, bool instantlyPlayCards)
    {
        if(SlotManager.GetSlotCount() != phaseData.CardPhase.Length) SlotManager.GeneratePlayRow(phaseData.CardPhase.Length);

        int cardIndex = 0;
        foreach(CardSO cardData in phaseData.CardPhase)
        {
            if (cardData == null)
            {
                cardIndex++;
                continue;
            }

            GameObject newCard = CardMaker.MakeCard(cardData);
            if(instantlyPlayCards) SlotManager.AddEnemyCard(newCard, cardIndex);
            else SlotManager.AddQueuedEnemyCard(newCard, cardIndex);

            cardIndex++;
        }
    }

    public void TurnOn()
    {
        CardGameCanvas.SetActive(true);
        CanvasStateChange?.Invoke(true);
        PrayerStatueScript.SetAllScrens(false);
        LowerCardScreenScript.SetAllScreens(true);
    }

    public void TurnOff()
    {
        CardGameCanvas.SetActive(false);
        CanvasStateChange?.Invoke(false);
        PrayerStatueScript.SetAllScrens(true);
        LowerCardScreenScript.SetAllScreens(false);
    }

    public void DrawCard(int count = 1)
    {
        for(int i = 0; i < count; i++)
        {
            GameObject DrawnCard = Deck.DrawCard();
            if(DrawnCard != null)
            {
                HandManager.AddCard(DrawnCard);
            }
        }
    }

    public void CleanupParticleLayer()
    {
        for(int i = ParticleLayer.childCount - 1; i >= 0; i--)
        {
            Destroy(ParticleLayer.GetChild(i).gameObject);
        }
    }
}
