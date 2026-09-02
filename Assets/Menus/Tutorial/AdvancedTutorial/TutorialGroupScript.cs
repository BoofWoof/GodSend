using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialGroupScript : MonoBehaviour
{
    public string GroupName;

    public List<TutorialScreenScript> GroupScreens = new();

    public bool Unlocked;
    public bool Shown;
    public bool ShowOnlyOnce;
    public UnityEvent OnFirstClose;

    [HideInInspector]public bool Active = false;
    private bool Waiting = false;

    public void Awake()
    {
        foreach (TutorialScreenScript screen in GroupScreens)
        {
            screen.gameObject.SetActive(false);
        }
    }

    public void TriggerAll()
    {

        foreach (TutorialScreenScript screen in GroupScreens)
        {
            if(screen.TriggerOnAllTrigger) screen.ONShow?.Invoke();
        }
    }

    public void ShowGroup()
    {
        if (Active) return;

        StartCoroutine(ShowGroupCoroutine());
    }

    public IEnumerator ShowGroupCoroutine()
    {
        Active = true;

        foreach (TutorialScreenScript screen in GroupScreens)
        {
            if (screen.ShowOnlyOnce && screen.Shown) continue;

            screen.gameObject.SetActive(true);
            screen.Shown = true;

            screen.ONShow?.Invoke();

            Waiting = true;
            while (Waiting)
            {
                yield return null;
            }

            screen.gameObject.SetActive(false);
        }
        Debug.Log($"Tutorial Group: {GroupName} has been finished.");
        if (!Shown) OnFirstClose?.Invoke();

        Shown = true;
        Active = false;
    }

    public void Continue()
    {
        Waiting = false;
    }
}
