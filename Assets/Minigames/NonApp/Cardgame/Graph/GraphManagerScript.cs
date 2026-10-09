using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GraphManagerScript : MonoBehaviour
{
    public List<CardSlot> CardSlots = new();

    public TMP_Text TimeText;

    private int CardGameTime = 0;

    public void Start()
    {
        foreach (CardSlot slot in CardSlots)
        {
            slot.OnCardPlay += OnCardPlay;
        }
    }

    public void OnCardPlay(GameCardScript playedCard)
    {
        Debug.Log("Test Card Played");

        CardGameTime += playedCard.CardData.TimeCost;
        UpdateTimeText();
    }

    public void UpdateTimeText()
    {
        TimeText.text = $"Time:{CardGameTime}";
    }
}
