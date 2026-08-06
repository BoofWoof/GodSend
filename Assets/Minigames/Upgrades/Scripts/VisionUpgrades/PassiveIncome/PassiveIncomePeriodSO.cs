using UnityEngine;

[CreateAssetMenu(fileName = "PassivePeriodSO", menuName = "Upgrades/PassiveIncome/PassivePeriodSO")]
public class PassiveIncomePeriodSO : UpgradesAbstract
{
    public float NewPeriod = 5f;
    public override void OnBuy(bool load)
    {
        PassiveIncomeScript.ImproveIncomePeriod(NewPeriod);
    }
}
