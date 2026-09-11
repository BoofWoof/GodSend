using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public static List<string> UsedDialogues = new(); //These clear on load. Don't rely on save and loading.

    public string VoiceLinePath;
    [ActorPopup] public string TargetActor;

    public void TriggerDialogue()
    {
        if (UsedDialogues.Contains(VoiceLinePath)) return;
        UsedDialogues.Add(VoiceLinePath);

        Debug.Log($"Sending dialogue to {TargetActor} from path {VoiceLinePath}.");

        CharacterSpeechScript.BroadcastSpeechAttempt(TargetActor, VoiceLinePath);
    }
}
