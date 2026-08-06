using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "PuzzleRewardUpgrade", menuName = "Upgrades/PuzzleReward")]
public class PuzzleRewardSO : ValueModifierAbstract
{
    [Header("Reward Changes")]
    public float MultiplyReward;
    public Color DisplayColor = Color.green;

    public static float TotalMultiplier = 1f;

    public override string ModifierDescription()
    {
        return UpgradeName + ": x" + MultiplyReward.NumberToString(true);
    }

    public override void ValueModifier(ref float referenceValue)
    {
        return;
    }

    public override void OnBuy(bool load)
    {
        TotalMultiplier *= MultiplyReward;
        if (IsAlreadySubscribed()) return;
        TurkPuzzleScript.secondaryMuliplierListModifier += ListModifier;
    }

    public bool IsAlreadySubscribed()
    {
        if (TurkPuzzleScript.secondaryMuliplierListModifier == null)
            return false;

        foreach (Delegate d in TurkPuzzleScript.secondaryMuliplierListModifier.GetInvocationList())
        {
            if (d.Method.DeclaringType == typeof(PuzzleRewardSO))
            {
                return true;
            }
        }

        return false;
    }

    public void ListModifier(ref List<SecondaryMultiplier> referenceValue)
    {
        float multiplier = TotalMultiplier;

        if (multiplier > 1.01f)
        {
            //string hex = DisplayColor.ToHexString().Substring(0, 6);
            referenceValue.Add(
                new SecondaryMultiplier
                {
                    multiplier = multiplier,
                    description = "<size=30><color=#" + DisplayColor.ToHexString() + "><b>GRANDEUR:</size> x</b>" + multiplier.AllSignificantDigits(3) + "</color>"
                }
                );
        }
    }
}
