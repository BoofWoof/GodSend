using PixelCrushers.DialogueSystem;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DS;

public class ConversationManagerScript : MonoBehaviour
{
    public static ConversationManagerScript instance;

    public static bool ConversationOngoing = false;
    public static bool isMacroConvo = false;
    public static bool WaitingForEvent = false;

    public static List<string> BannedDialogues = new List<string>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        transform.SetParent(DialogueManager.instance.transform);

        ConversationOngoing = false;
        isMacroConvo = false;
        WaitingForEvent = false;

        BannedDialogues = new List<string>();
    }

    public void ForceNextDialogue()
    {
        StartCoroutine(DelayedSkip());
    }
    public IEnumerator DelayedSkip()
    {
        yield return null;
        yield return null;
        Sequencer.Message("FinishedSpeaking");
    }

    void Start()
    {
        Lua.RegisterFunction("QueueDialogue", null, SymbolExtensions.GetMethodInfo(() => MessageQueue.addDialogue("")));
        Lua.RegisterFunction("QueueWaitDialogue", null, SymbolExtensions.GetMethodInfo(() => MessageQueue.addDialogue("", 0)));

        StartCoroutine(WaitForNextConversation());

        transform.parent = DialogueManager.instance.transform;
    }

    public void StartDialogue(string newConversation)
    {
        Debug.Log($"Conversation Starting {newConversation}");

        Conversation newConv = DialogueManager.masterDatabase.GetConversation(newConversation);
        bool allowRepeat = Field.LookupBool(newConv.fields, "AllowRepeat");
        if(!allowRepeat) BannedDialogues.Add(newConversation);

        isMacroConvo = Field.LookupBool(newConv.fields, "IsMacro");

        ConversationOngoing = true;

        DialogueManager.StopConversation();
        DialogueManager.StartConversation(newConversation);

        if (isMacroConvo)
        {
            PrayerScript.StoryMode = true;
            //MusicSelectorScript.SetOverworldSong(3);
        }
        else
        {
            PrayerScript.StoryMode = false;
        }
    }
    public void OnConversationEnd(Transform actor)
    {
        PrayerScript.StoryMode = false;
        ConversationOngoing = false;
        WaitingForEvent = false;

        Debug.Log($"The conversation is now over.");

        Resources.UnloadUnusedAssets();
    }

    public IEnumerator WaitForNextConversation()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (GameStateMonitor.isEventActive()) continue;
            if (!ConversationOngoing && MessageQueue.GetQueueLength() > 0)
            {
                Dialogue nextDialogue = MessageQueue.getNextDialogue();

                if (BannedDialogues.Contains(nextDialogue.dialouge)) continue;

                bool exists = DialogueManager.masterDatabase.GetConversation(nextDialogue.dialouge) != null;
                if (!exists)
                {
                    Debug.LogError($"Dialogue does not exist: {nextDialogue.dialouge}");
                    continue;
                }

                yield return new WaitForSeconds((float)nextDialogue.wait);
                StartDialogue(nextDialogue.dialouge);
            }
        }
    }

}
