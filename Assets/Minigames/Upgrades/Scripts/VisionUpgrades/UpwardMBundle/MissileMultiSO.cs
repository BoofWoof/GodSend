using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "MissileMultiSO", menuName = "Upgrades/VM/MissileMultiSO")]
public class MissileMultiSO : ValueModifierAbstract
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
        int threatsDestroyed = AerialDefenseScript.TotalProjectilesDestroyed;

        float multiplier = threatsDestroyed / 15f + 1f;

        if (multiplier > 1.01f)
        {
            //string hex = DisplayColor.ToHexString().Substring(0, 6);
            referenceValue.Add(
                new SecondaryMultiplier
                {
                    multiplier = multiplier,
                    description = "<size=30><color=#" + DisplayColor.ToHexString() + "><b>THREAT DESTRUCTION:</size> x</b>" + multiplier.AllSignificantDigits(3) + "</color>"
                }
                );
        }
    }
}
