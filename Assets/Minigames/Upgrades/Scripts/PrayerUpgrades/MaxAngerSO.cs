using UnityEngine;

[CreateAssetMenu(fileName = "MaxAngerUpgrade", menuName = "Upgrades/MaxAngerUpgrade")]
public class MaxAngerSO : UpgradesAbstract
{
    [Header("IncreasedMaxAnger")]
    public float MaxAngerIncrease;

    public override void OnBuy(bool load)
    {
        PrayerScript.instance.IncreaseAngerThreshold(MaxAngerIncrease);
    }
}

