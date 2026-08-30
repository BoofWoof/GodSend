using UnityEngine;

public class TriggersMascotDialogue : MonoBehaviour
{
    [TextArea]public string DialogueText;

    private bool Triggered;

    public void Send()
    {
        if (Triggered) return;
        Triggered = true;

        VisionMascotScript.SayText(DialogueText);
    }
}
