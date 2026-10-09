using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CardSO", menuName = "Cards/CardSO")]
public class CardSO : ScriptableObject
{
    public enum CardTypeEnum
    {
        OngoingStrategy,
        StrategyModifier,
        ImmediateAction
    }
    public enum CardTargetEnum
    {
        TargetsSlot,
        TargetsCard,
        NoTarget
    }

    public string UniqueID;
    public string CardName;
    public Sprite CardArt;
    public int TimeCost;
    [TextArea] public string Description;

    public CardTypeEnum CardType; //Decides When On Activate Triggers
    public CardTargetEnum TargetType; //Decides What You Can Target

    public int CardStrength = 1; //Decides How Effective A Card Is (Has Different Meaning For Different Cards.

    [Header("Keywords")]
    public bool Required = false;

    virtual public void OnActivate()
    {

    }
}
