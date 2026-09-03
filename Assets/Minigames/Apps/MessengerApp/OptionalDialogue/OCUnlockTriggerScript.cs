using PixelCrushers.DialogueSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OCUnlockTriggerScript : MonoBehaviour
{
    [Serializable]
    public class ConditionalEventData
    {
        public string BoolName;
        public bool VariableMustBeTrue = true;
        public UnityEvent ConditionalEvent;
    }

    public OCSO OCToRelease;
    private bool Released = false;

    public UnityEvent OnDialogueCompletion;
    public bool AutomaticallyRelease;

    public List<ConditionalEventData> ConditionalEvents;

    public virtual void OnEnable()
    {
        ConversationManagerScript.OnConversationEndEvent += OnConversationEnd;
    }

    public void Start()
    {
        if (AutomaticallyRelease) Release();
    }

    public virtual void OnDisable()
    {
        ConversationManagerScript.OnConversationEndEvent -= OnConversationEnd;
    }
    public void Release()
    {
        if (Released) return;
        OCManager.instance.AddOC(OCToRelease);
    }

    public void OnConversationEnd(string conversationEnd)
    {
        if(conversationEnd == OCToRelease.OCSDialogueName)
        {
            OnDialogueCompletion?.Invoke();

            foreach (ConditionalEventData conditionalEventData in ConditionalEvents)
            {
                bool trigger = DialogueLua.GetVariable(conditionalEventData.BoolName).asBool;
                if(trigger == conditionalEventData.VariableMustBeTrue)
                {
                    conditionalEventData.ConditionalEvent?.Invoke();
                }
            }
        }
    }
}
