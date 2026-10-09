using System.Collections.Generic;
using UnityEngine;

public class HandManagerScript : MonoBehaviour
{
    public List<CardSlot> CardSlots;


    public bool AddCard(GameObject newCard)
    {
        //Returns if card was successfully added.
        foreach (CardSlot slot in CardSlots)
        {
            if (!slot.AddCardToStack(newCard)) continue;
            return true;
        }
        return false;
    }
}
