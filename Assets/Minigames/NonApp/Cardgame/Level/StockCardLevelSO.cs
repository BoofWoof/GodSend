using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StockCardLevelSO", menuName = "Cards/StockCardLevelSO")]
public class StockCardLevelSO : ScriptableObject
{
    public int WinsNeeded;
    public int LossesAllowed;

    public int DefaultSlots = 7;

    public PhaseData[] Phases;

    [Serializable]
    public class PhaseData
    {
        public int GoalDelta = 0;
        public bool GreaterThanGoal = true;
        public CardSO[] CardPhase;
    }

    [ContextMenu("GenerateLevelFramework")]
    public void GenerateFramework()
    {
        int MaxRounds = WinsNeeded + LossesAllowed - 1;

        if (MaxRounds <= 0) return;

        Phases = new PhaseData[MaxRounds];
        
        for (int i = 0; i < Phases.Length; i++)
        {
            Phases[i] = new PhaseData();

            Phases[i].CardPhase = new CardSO[DefaultSlots];
        }
    }
}
