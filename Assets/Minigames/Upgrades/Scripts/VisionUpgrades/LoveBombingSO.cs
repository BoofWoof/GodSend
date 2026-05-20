using UnityEngine;

[CreateAssetMenu(fileName = "LoveBombing", menuName = "Upgrades/LoveBombing")]
public class LoveBombingSO : UpgradesAbstract
{
    [Header("PuzzleDifficultyReward")]
    public int CompletionDifficulty;
    public int PrayerRequired = 5;

    private int PrayersSubmitted = 0;

    public override void OnBuy()
    {
        PrayerScript.PrayerSubmitted += OnPrayerSubmission;
    }

    public void OnPrayerSubmission(bool goodPrayer)
    {
        PrayersSubmitted++;

        if (PrayersSubmitted >= PrayerRequired)
        {
            float reward = TurkPuzzleScript.instance.QuickCalculateReward(CompletionDifficulty);
            CurrencyData.Credits += reward;

            AnnouncementScript.StartAnnouncement($"Love bombing has earned you {reward.ToString("G2")} credits.");
            Debug.Log($"Love bombing: {reward}");

            PrayersSubmitted = 0;
        }
    }
}