using UnityEngine;

[CreateAssetMenu(fileName = "ComfortingWordsSO", menuName = "Upgrades/Meditation/ComfortingWords")]
public class ComfortingWordsSO : UpgradesAbstract
{
    public float AngerReductionIncrease;
    public override void OnBuy(bool load)
    {
        PrayerScript.instance.AngerReduction += AngerReductionIncrease;
    }
}
