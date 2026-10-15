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

    [Header("Keywords")]
    public bool Required = false;

    virtual public void OnActivate()
    {

    }
}
