using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "PuzlMultiSO", menuName = "Upgrades/VM/PuzlMultiSO")]
public class PuzlMultiSO : ValueModifierAbstract
{
    public Color DisplayColor;

    public override string ModifierDescription()
    {
        return "";
    }

    public override void ValueModifier(ref float referenceValue)
    {
        return;
    }

    public override void OnBuy()
    {
        TurkPuzzleScript.secondaryMuliplierListModifier += ListModifier;
    }

    public void ListModifier(ref List<SecondaryMultiplier> referenceValue)
    {
        int totalPuzzles = 0;
        foreach(int solved in TurkPuzzleScript.PuzzlesCompleted.Values)
        {
            totalPuzzles += solved;
        }

        float multiplier = totalPuzzles + 1f;

        if (multiplier > 1.01f)
        {
            //string hex = DisplayColor.ToHexString().Substring(0, 6);
            referenceValue.Add(
                new SecondaryMultiplier
                {
                    multiplier = multiplier,
                    description = "<size=30><color=#" + DisplayColor.ToHexString() + "><b>PUZZLING:</size> x</b>" + multiplier.AllSignificantDigits(3) + "</color>"
                }
                );
        }
    }
}
