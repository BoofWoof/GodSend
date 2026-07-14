using Steamworks;
using TMPro;
using UnityEngine;

public class AngyRockScript : MonoBehaviour
{
    public TMP_Text ClockText;
    public TMP_Text ScoreText;
    public TMP_Text HighScoreText;
    public TMP_Text GlobalText;

    private static float StartingTime;
    public static int Score = 0;
    public static int HighScore = 0;
    public static int SessionTotal = 0;
    public static bool ActiveRun = false;

    public static float BonusSaveTime = 0f;

    public float MaxTime = 60f * 5f;

    public void Start()
    {
        ScoreText.text = "<b>Score</b>: " + Score.ToString();
        HighScoreText.text = "<b>Highscore</b>: " + HighScore.ToString();

        if (ActiveRun)
        {
            UpdateTimer();
        }

        GlobalText.text = "<b>Officewide Praise</b>: ???";
        UpdateOfficeWide();
    }

    public void Update()
    {
        if (!ActiveRun) return;
        UpdateTimer();
    }

    public void UpdateTimer()
    {
        float timePassed = TimePassed();
        ClockText.text = System.TimeSpan.FromSeconds(MaxTime - timePassed).ToString("m\\:ss");

        if (MaxTime < timePassed)
        {
            ActiveRun = false;
            ResetScore();
        }
    }

    public void Praise()
    {
        if(ActiveRun == true)
        {
            if (TimePassed() > 60)
            {
                IncreaseScore();
            }
            return;
        }
        ActiveRun = true;
        IncreaseScore();
    }

    public static float TimePassed()
    {
        return (Time.time - StartingTime) + BonusSaveTime;
    }

    private void IncreaseScore()
    {
        OverworldAngryRockScript.PlayHappyBaa();
        OverworldAngryRockScript.Reset();

        SessionTotal++;

        BonusSaveTime = 0f;

        StartingTime = Time.time;
        Score += 1;
        if (Score > HighScore)
        {
            HighScore = Score;
            HighScoreText.text = "<b>Highscore</b>: " + HighScore.ToString();
        }
        ScoreText.text = "<b>Score</b>: " + Score.ToString();

        SteamManager.UpdateIntStat("AriesRockGlobal", + SteamManager.GetIntStat("AriesRockGlobal") + 1, false);
        SteamManager.UpdateIntStat("AriesRock", Score);

        if(Score >= 10)
        {
            SteamManager.TriggerSteamAchievement("This Rocks!");
        }

        if (SteamManager.Initialized)
        {
            UpdateOfficeWide();
        }
        else
        {
            GlobalText.text = "";
        }
    }

    private void UpdateOfficeWide()
    {
        if (!SteamManager.Initialized) return;

        CallResult<GlobalStatsReceived_t> m_GlobalStatsCallResult;

        m_GlobalStatsCallResult = CallResult<GlobalStatsReceived_t>.Create(OnGlobalStatsRecieved);

        SteamAPICall_t handle = SteamUserStats.RequestGlobalStats(0);

        m_GlobalStatsCallResult.Set(handle);
        Debug.Log("Requested global stats from Steam...");
    }

    public void OnGlobalStatsRecieved(GlobalStatsReceived_t pCallback, bool bIOFailure)
    {
        if (pCallback.m_eResult == EResult.k_EResultOK)
        {
            long globalTotal = 0;

            // Step 3: Fetch using the correct 64-bit GetGlobalStat function
            if (SteamUserStats.GetGlobalStat("AriesRockGlobal", out globalTotal))
            {
                Debug.Log($"Success! Global aggregated total is: {globalTotal}");
                GlobalText.text = $"<b>Officewide Praise</b>: {globalTotal + SessionTotal}";
            }
            else
            {
                Debug.LogError("Failed to retrieve the specific global stat.");
            }
        }
        else
        {
            Debug.LogError($"Global stats request failed with result: {pCallback.m_eResult}");
        }

    }

    public static void ResetStartingTime()
    {
        StartingTime = Time.time;
    }

    public void ResetScore()
    {
        Score = 0;
        ScoreText.text = "<b>Score</b>: " + Score.ToString();
        ClockText.text = "0:00";

        BonusSaveTime = 0f;
    }
}
