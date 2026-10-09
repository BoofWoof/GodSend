using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeckSO", menuName = "Cards/DeckSO")]
public class DeckSO : ScriptableObject
{
    public List<CardSO> CardsInDeck;

    public void ShuffleDeck()
    {
        CardsInDeck.Shuffle();
    }

    public GameObject DrawCard()
    {
        if (CardsInDeck.Count <= 0) return default;

        GameObject drawnCard = CardMakerScript.instance.MakeCard(CardsInDeck[0]);
        CardsInDeck.RemoveAt(0);

        return drawnCard;
    }
}
