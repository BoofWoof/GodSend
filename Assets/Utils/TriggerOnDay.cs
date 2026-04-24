using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DayTriggers
{
    public bool TriggerDay0 = false;
    public bool TriggerDay1 = false;
    public bool TriggerDay2 = false;
    public bool TriggerDay3 = false;
    public bool TriggerDay4 = false;
    public bool TriggerDay5 = false;

    public UnityEvent Triggers;
}
public class TriggerOnDay : MonoBehaviour
{
    public List<DayTriggers> TriggerList = new List<DayTriggers>();

    public void Start()
    {
        foreach (DayTriggers t in TriggerList)
        {
            if (t.TriggerDay0 && DayInfo.CurrentDay == 0)
            {
                t.Triggers?.Invoke();
            }
            if (t.TriggerDay1 && DayInfo.CurrentDay == 1)
            {
                t.Triggers?.Invoke();
            }
            if (t.TriggerDay2 && DayInfo.CurrentDay == 2)
            {
                t.Triggers?.Invoke();
            }
            if (t.TriggerDay3 && DayInfo.CurrentDay == 3)
            {
                t.Triggers?.Invoke();
            }
            if (t.TriggerDay4 && DayInfo.CurrentDay == 4)
            {
                t.Triggers?.Invoke();
            }
            if (t.TriggerDay5 && DayInfo.CurrentDay == 5)
            {
                t.Triggers?.Invoke();
            }
        }
    }
}
