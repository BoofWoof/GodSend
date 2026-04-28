using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class AngyRockScript : MonoBehaviour
{
    public TMP_Text ClockText;
    public TMP_Text ScoreText;
    public TMP_Text HighScoreText;

    private static float StartingTime;
    private static int Score = 0;
    private static int HighScore = 0;
    public static bool ActiveRun = false;

    public float MaxTime = 60f * 5f;

    public void Start()
    {
        ScoreText.text = "<b>Score</b>: " + Score.ToString();
        HighScoreText.text = "<b>Highscore</b>: " + HighScore.ToString();

        if (ActiveRun)
        {
            UpdateTimer();
        }
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
        return Time.time - StartingTime;
    }

    private void IncreaseScore()
    {
        OverworldAngryRockScript.PlayHappyBaa();
        OverworldAngryRockScript.Reset();

        StartingTime = Time.time;
        Score += 1;
        if (Score > HighScore)
        {
            HighScore = Score;
            HighScoreText.text = "<b>Highscore</b>: " + HighScore.ToString();
        }
        ScoreText.text = "<b>Score</b>: " + Score.ToString();
    }

    public void ResetScore()
    {
        Score = 0;
        ScoreText.text = "<b>Score</b>: " + Score.ToString();
        ClockText.text = "0:00";
    }
}
