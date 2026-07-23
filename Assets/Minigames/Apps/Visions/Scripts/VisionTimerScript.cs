using PixelCrushers.DialogueSystem;
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
        public string AltSpeaker;
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

    public bool Complete = false;

    public void Awake()
    {
        gameObject.SetActive(false);
    }

    public void AllowTimerStart()
    {
        TimerCanStart = true;
    }

    public void OnEnable()
    {
        UpgradesAbstract.OnUpgradeBought += WinCheck;
    }

    public void OnDisable()
    {
        UpgradesAbstract.OnUpgradeBought -= WinCheck;
    }

    public void StartTimer(float Time)
    {
        if (Complete) return;
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

        CheckCountdownTriggers();

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

        LoseCheck();
    }

    public void WinCheck(string UpgradeID)
    {
        if (UpgradeID != "78a90b1238549aa4d9e7d5ab7092e3c7") return;
        DialogueLua.SetVariable("D2TimeChallengeWin", true);
        MessageQueue.addDialogue("D2TimerChallengeComplete");

        Complete = true;
        gameObject.SetActive(false);
    }

    public void LoseCheck()
    {
        if (TimeRemaining > 0) return;
        DialogueLua.SetVariable("D2TimeChallengeWin", false);
        MessageQueue.addDialogue("D2TimerChallengeComplete");

        Complete = true;
        gameObject.SetActive(false);
    }

    public void CheckCountdownTriggers()
    {
        foreach(CountdownTriggers trigger in TimerTriggers)
        {
            if (trigger.Triggered) continue;
            if (TimeRemaining > trigger.TriggerTime) return;
            trigger.Triggered = true;

            if(string.IsNullOrEmpty(trigger.AltSpeaker))
                CharacterSpeechScript.BroadcastSpeechAttempt("MacroAlesssandro", trigger.VoicePath);
            else
                CharacterSpeechScript.BroadcastSpeechAttempt(trigger.AltSpeaker, trigger.VoicePath);
        }
    }
}
