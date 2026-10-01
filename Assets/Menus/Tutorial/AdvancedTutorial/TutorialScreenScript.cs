using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialScreenScript : UniqueIDScript
{
    public bool ShowOnlyOnce = false;
    public bool Shown = false;

    public bool TriggerOnAllTrigger = true;

    public UnityEvent ONShow;
}
