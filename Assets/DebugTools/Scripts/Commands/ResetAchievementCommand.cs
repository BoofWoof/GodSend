using Steamworks;
using UnityEngine;

namespace DebugTools.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "ResetAchievementCommand", menuName = "DebugCommands/Steam/ResetAchievementCommand")]
    public class ResetAchievementCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            if (!SteamManager.Initialized) return true;

            if (args.Length > 0)
            {
                string achievementID = args[0];

                // Clear the achievement locally
                SteamUserStats.ClearAchievement(achievementID);

                // Push the reset to Steam
                SteamUserStats.StoreStats();
                Debug.Log($"Achievement '{achievementID}' has been reset!");

                return true;
            }

            // The 'true' parameter means it will reset both Achievements AND Stats (like leaderboards/counters)
            SteamUserStats.ResetAllStats(true);

            // Save the wipe to the server
            SteamUserStats.StoreStats();
            Debug.Log("All Steam achievements and stats have been wiped for this user.");

            return true;
        }
    }
}