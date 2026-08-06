using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Speedster", menuName = "Upgrades/Challenges/Speedster")]
public class SpeedsterSO : ValueModifierAbstract
{
    public float FastestTime;
    public float MinMultiplier;
    public float MaxMultiplier;

    public Color DisplayColor = Color.green;

    public bool GoFast = true;

    public override string ModifierDescription()
    {
        return "";
    }

    public override void OnBuy(bool load)
    {
        TurkPuzzleScript.secondaryMuliplierListModifier += ListModifier;
    }

    public void ListModifier(ref List<SecondaryMultiplier> referenceValue)
    {
        float multiplier;
        if (GoFast)
        {
            multiplier = CalculateSpeedMultiplier();
        } else
        {
            multiplier = CalculateSlowMultiplier();
        }

        if(multiplier > 1.01f)
        {
            //string hex = DisplayColor.ToHexString().Substring(0, 6);
            if (GoFast)
            {
                referenceValue.Add(
                    new SecondaryMultiplier
                    {
                        multiplier = multiplier,
                        description = "<size=30><color=#" + DisplayColor.ToHexString() + "><b>FALCON SPEED:</size> x</b>" + multiplier.AllSignificantDigits(3) + "</color>"
                    }
                    );
            } else
            {
                referenceValue.Add(
                    new SecondaryMultiplier
                    {
                        multiplier = multiplier,
                        description = "<size=30><color=#" + DisplayColor.ToHexString() + "><b>PENGUIN SPEED:</size> x</b>" + multiplier.AllSignificantDigits(3) + "</color>"
                    }
                    );
            }
        }
    }
    public override void ValueModifier(ref float referenceValue)
    {
    }

    public float CalculateSpeedMultiplier()
    {
        float SlowestTime = TurkPuzzleScript.instance.LevelSets[TurkPuzzleScript.CurrentDifficutly].FalconSpeed;
        float timePassed = Time.time - TurkPuzzleScript.instance.StartingTime;
        float rawMultValue = (SlowestTime - timePassed) / (SlowestTime - FastestTime);
        float clampedMultValue = Mathf.Clamp01(rawMultValue);
        float finalMultiplier = Mathf.Lerp(MinMultiplier, MaxMultiplier, clampedMultValue);

        Debug.Log(finalMultiplier);
        return finalMultiplier;
    }
    public float CalculateSlowMultiplier()
    {
        float timePassed = Time.time - TurkPuzzleScript.instance.StartingTime;
        float finalMultiplier = 1f + timePassed / 240f;

        Debug.Log(finalMultiplier);
        return finalMultiplier;
    }
}
