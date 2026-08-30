using UnityEngine;
using UnityEngine.Events;

public class OCUnlockTriggerScript : MonoBehaviour
{
    public OCSO OCToRelease;
    private bool Released = false;

    public UnityEvent OnDialogueCompletion;
    public bool AutomaticallyRelease;

    public void OnEnable()
    {
        ConversationManagerScript.OnConversationEndEvent += OnConversationEnd;
    }

    public void Start()
    {
        if (AutomaticallyRelease) Release();
    }

    public void OnDisable()
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
        }
    }
}
