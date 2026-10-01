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

    public static Dictionary<string, OCUnlockTriggerScript> OCUnlockDict = new();

    public string OCName;

    public OCSO OCToRelease;
    public bool Released = false;

    public UnityEvent OnDialogueCompletion;
    public bool AutomaticallyRelease;

    public List<ConditionalEventData> ConditionalEvents;

    public bool AdvertiseUnlock = false;

    public static void UnlockOCByName(string ocName)
    {
        Debug.Log(ocName);

        string lowerName = ocName.ToLower();

        if (!OCUnlockDict.ContainsKey(lowerName)) return;
        OCUnlockDict[lowerName].Release();
    }

    public virtual void OnEnable()
    {
        if (string.IsNullOrEmpty(OCName)) OCName = name;

        ConversationManagerScript.OnConversationEndEvent += OnConversationEnd;
        if(!string.IsNullOrEmpty(OCName) && !OCUnlockDict.ContainsKey(OCName.ToLower())) OCUnlockDict.Add(OCName.ToLower(), this);
    }

    public void CheckAutoRelease()
    {
        if (AutomaticallyRelease) Release();
    }

    public virtual void OnDisable()
    {
        ConversationManagerScript.OnConversationEndEvent -= OnConversationEnd;
        if (!string.IsNullOrEmpty(OCName)) OCUnlockDict.Remove(OCName.ToLower());
    }
    public void Release()
    {
        if (Released) return;
        Released = true;
        OCManager.instance.AddOC(OCToRelease);

        if (AdvertiseUnlock)
        {
            string appName = "Contact";
            string previewText = $"<b>A New Optional Dialogue Is Available:</b>\nHead to <b>Hex App</b> to check it out!";
            AppScript targetApp = AppScript.AppsDict[appName];

            AppNotificationScript.SetNotification(new AppNotificationScript.NotificationInfo
            {
                SourceApp = targetApp,
                PreviewImage = targetApp.AssociatedIcon,
                PreviewText = previewText,
                AdditionalActions = null
            });
        }
    }

    public void OnConversationEnd(string conversationEnd)
    {
        Debug.Log(conversationEnd);
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
