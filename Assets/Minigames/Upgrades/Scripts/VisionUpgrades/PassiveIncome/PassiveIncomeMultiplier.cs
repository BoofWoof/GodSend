using UnityEngine;

[CreateAssetMenu(fileName = "PassiveIncomeMultiplierSO", menuName = "Upgrades/PassiveIncome/PassiveIncomeMultiplierSO")]
public class PassiveIncomeMultiplier : UpgradesAbstract
{
    public override void OnBuy()
    {
        PassiveIncomeScript.ActivateTriggerMultiplier = true;
    }
}
