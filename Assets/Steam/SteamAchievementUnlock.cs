using UnityEngine;

public class SteamAchievementUnlock : MonoBehaviour
{
    public string AchievementName;

    public void UnlockSteamAchievement()
    {
        SteamManager.TriggerSteamAchievement(AchievementName);
    }
}
