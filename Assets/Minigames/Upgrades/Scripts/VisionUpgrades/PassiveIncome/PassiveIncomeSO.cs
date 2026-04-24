using UnityEngine;

[CreateAssetMenu(fileName = "PassiveIncomeSO", menuName = "Upgrades/PassiveIncome/PassiveIncomeSO")]
public class PassiveIncomeSO : UpgradesAbstract
{
    public override void OnBuy()
    {
        PassiveIncomeScript.StartPassiveIncome();
    }
}
