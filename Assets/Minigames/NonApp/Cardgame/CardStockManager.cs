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
    public void Awake()
    {
        instance = this;
        Deck = Instantiate(StartingDeck);
        Deck.ShuffleDeck();
        TurnOff();
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
}
