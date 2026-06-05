using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisionTimerScript : MonoBehaviour
{
    [Serializable]
    public class CountdownTriggers
    {
        public float TriggerTime;
        public bool Triggered;
        public string VoicePath;
    }

    public List<string> PauseVoiceLines = new List<string>();
    public int PauseIndex = 0;

    public List<CountdownTriggers> TimerTriggers = new List<CountdownTriggers>();

    public float TimeRemaining = 60 * 5;
    public TMP_Text TimeText;

    public bool TimerCanStart = true;
    public bool TimerStarted = false;

    public GameObject UpgradePanel;
    public GameObject FocusPanel;
    public GameObject FocusPanel2;

    private bool LastPaused = false;

    public void Awake()
    {
        gameObject.SetActive(false);
    }

    public void AllowTimerStart()
    {
        TimerCanStart = true;
    }

    public void StartTimer(float Time)
    {
        if (DayInfo.CurrentDay != 0 && DayInfo.CurrentDay != 2) return;
        if (!TimerCanStart) return;
        if (TimerStarted) return;
        gameObject.SetActive(true);
        TimeRemaining = Time;
        TimerStarted = true;
    }

    public void Update()
    {
        if(!TimerStarted) return;
        if (UpgradePanel.activeSelf && !GameStateMonitor.isSpeakingSourceActive() && !LastPaused)
        {
            CharacterSpeechScript.BroadcastSpeechAttempt("RadioMilo", PauseVoiceLines[PauseIndex]);

            PauseIndex++;
            PauseIndex %= PauseVoiceLines.Count;
        }

        LastPaused = false;
        if (UpgradePanel.activeSelf || FocusPanel.activeSelf || FocusPanel2.activeSelf)
        {
            TimeText.text = "PAUSED";
            LastPaused = true;
            return;
        } 
        TimeRemaining -= Time.deltaTime;
        TimeText.text = TimeSpan.FromSeconds(TimeRemaining).ToString("m\\:ss");

    }
}
