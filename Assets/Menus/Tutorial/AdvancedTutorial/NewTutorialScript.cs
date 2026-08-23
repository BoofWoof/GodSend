using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewTutorialScript : MonoBehaviour
{
    public List<TutorialGroupScript> TutorialGroupsList = new();
    private Dictionary<string, TutorialGroupScript> TutorialGroupDict = new();

    private bool Active = false;

    public bool FirstTimeShown = true;

    public Image ViewBarrier;

    public void Awake()
    {
        ViewBarrier.enabled = false;

        foreach (TutorialGroupScript group in TutorialGroupsList)
        {
            TutorialGroupDict[group.GroupName] = group;
        }
    }

    public void UnlockAll()
    {
        foreach (TutorialGroupScript group in TutorialGroupsList)
        {
            group.Unlocked = true;
            group.OnFirstClose?.Invoke();
            group.TriggerAll();
        }
    }

    public void AttemptFirstTimeShow()
    {
        if (!FirstTimeShown) return;
        FirstTimeShown = false;
        ShowAllGroups();
    }

    public void UnlockGroupByName(string name)
    {
        if (!TutorialGroupDict.ContainsKey(name)) return;

        TutorialGroupScript targetGroup = TutorialGroupDict[name];
        targetGroup.Unlocked = true;
        ShowGroup(targetGroup);
    }

    public void ShowGroup(TutorialGroupScript group)
    {
        if (Active) return;
        StartCoroutine(ShowGroupCoroutine(group));
    }
    public IEnumerator ShowGroupCoroutine(TutorialGroupScript group)
    {
        Active = true;
        ViewBarrier.enabled = true;

        group.ShowGroup();

        while (group.Active)
        {
            yield return null;
        }

        ViewBarrier.enabled = false;
        Active = false;
    }

    public void ShowAllGroups()
    {
        if (Active) return;
        StartCoroutine(ShowAllGroupsCoroutine());
    }

    public IEnumerator ShowAllGroupsCoroutine()
    {
        Active = true;
        ViewBarrier.enabled = true;

        foreach (TutorialGroupScript group in TutorialGroupsList)
        {
            if (!group.Unlocked) continue;
            if (group.ShowOnlyOnce && group.Shown) continue;

            group.ShowGroup();

            while (group.Active)
            {
                yield return null;
            }
        }

        ViewBarrier.enabled = false;
        Active = false;
    }
}
