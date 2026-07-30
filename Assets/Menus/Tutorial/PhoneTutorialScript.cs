using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PhoneTutorialScript : MonoBehaviour
{
    [Serializable]
    public class TutorialDayStartTriggers
    {
        public int TargetDay;
        public int StartingTutorialStep;
        public int MaxTutorialStep;
        public bool Skip = false;
    }

    private List<Transform> TutorialScreens;
    private int TutorialStep = 0;
    public bool CompletedTutorial = false;
    public AudioSource Next;

    public int MaxTutorialStep = 999;

    public TutorialDayStartTriggers[] NewDayTriggerArray;

    private void Start()
    {
        StartTutorial();

        foreach (TutorialDayStartTriggers trigger in NewDayTriggerArray)
        {
            if(trigger.TargetDay == DayInfo.CurrentDay)
            {
                SetMaxTutorialSteps(trigger.MaxTutorialStep);

                if (trigger.Skip)
                {
                    TutorialScreens[TutorialStep].gameObject.SetActive(false);
                    CompletedTutorial = true;
                    GetComponent<Image>().enabled = false;
                    break;
                }
                JumpTutorial(trigger.StartingTutorialStep);
                break;
            }
        }
    }

    public void RestartTutorial()
    {
        StartTutorial();
    }

    public void HideTutorial()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.gameObject.SetActive(false);
        }
        GetComponent<Image>().enabled = false;
    }

    public void JumpTutorial(int TargetStep)
    {
        TutorialScreens[TutorialStep].gameObject.SetActive(false);
        TutorialStep = TargetStep;
        TutorialScreens[TutorialStep].gameObject.SetActive(true);
    }

    public void SetMaxTutorialSteps(int NewMax)
    {
        MaxTutorialStep = NewMax;
    }

    public void StartTutorial()
    {
        TutorialStep = 0;

        gameObject.SetActive(true);
        GetComponent<Image>().enabled = true;
        TutorialScreens = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            TutorialScreens.Add(child);
            child.gameObject.SetActive(false);
        }

        if (TutorialScreens.Count <= 0) return;
        TutorialScreens[0].gameObject.SetActive(true);

    }

    public void ProgressTutorial()
    {
        Next.Play();
        if (TutorialScreens.Count <= 0) return;
        TutorialScreens[TutorialStep].gameObject.SetActive(false);
        TutorialStep++;
        if(TutorialStep >= TutorialScreens.Count || TutorialStep >= MaxTutorialStep)
        {
            CompletedTutorial = true;
            GetComponent<Image>().enabled = false;
            return;
        }
        TutorialScreens[TutorialStep].gameObject.SetActive(true);
    }

    public void UnlockNewTutorialStep()
    {
        TutorialStep = MaxTutorialStep;
        GetComponent<Image>().enabled = true;
        MaxTutorialStep++;
        TutorialScreens[TutorialStep].gameObject.SetActive(true);
    }
}
