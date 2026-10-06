using UnityEngine;

public class TriggerStatement : MonoBehaviour
{
    public string AudioPath;

    public void TriggerDialogueStatement()
    {
        CharacterSpeechScript.BroadcastSpeechAttempt(null, AudioPath);
    }
}
