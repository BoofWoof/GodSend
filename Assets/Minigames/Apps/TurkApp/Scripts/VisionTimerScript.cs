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

    public List<CountdownTriggers> TimerTriggers = new List<CountdownTriggers>();

    public float TimeRemaining = 60 * 5;
    public TMP_Text TimeText;

    public bool TimerCanStart = true;
    public bool TimerStarted = false;

    public GameObject UpgradePanel;

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
        if (!TimerCanStart) return;
        if (TimerStarted) return;
        gameObject.SetActive(true);
        TimeRemaining = Time;
        TimerStarted = true;
    }

    public void Update()
    {
        if(!TimerStarted) return;
        if (UpgradePanel.activeSelf)
        {
            TimeText.text = "PAUSED";
            return;
        } 
        TimeRemaining -= Time.deltaTime;
        TimeText.text = TimeSpan.FromSeconds(TimeRemaining).ToString("m\\:ss");

    }
}
