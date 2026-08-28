using PixelCrushers.DialogueSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "OCSO", menuName = "Dialogue/OCSO")]
public class OCSO : ScriptableObject
{
    public enum OCAvailability
    {
        Available,
        DangerActive,
        DialogueActive,
        EventActive
    }

    public string UniqueID; //Add this to unique ID generator;
    public string OCSName;
    [TextArea] public string OCSDescription;
    [ActorPopup] public string AssociatedActor;

    [Header("Dialogue Data")]
    public string OCSDialogueName;

    [Header("When Can It Run")]
    public bool CanRunDuringEvents;

    public bool Day0Available = true;
    public bool Day1Available = true;
    public bool Day2Available = true;
    public bool Day3Available = true;
    public bool Day4Available = true;
    public bool Day5Available = true;

    public bool ShowOC()
    {
        if (!Day0Available && DayInfo.CurrentDay == 0) return false;
        if (!Day1Available && DayInfo.CurrentDay == 1) return false;
        if (!Day2Available && DayInfo.CurrentDay == 2) return false;
        if (!Day3Available && DayInfo.CurrentDay == 3) return false;
        if (!Day4Available && DayInfo.CurrentDay == 4) return false;
        if (!Day5Available && DayInfo.CurrentDay == 5) return false;
        return true;
    }

    public OCAvailability CheckAvailability()
    {
        if (ConversationManagerScript.ConversationOngoing || MessageQueue.GetQueueLength() > 0) return OCAvailability.DialogueActive;
        if (GameStateMonitor.DangerActive) return OCAvailability.DangerActive;
        if (GameStateMonitor.ChallengeActive && !CanRunDuringEvents) return OCAvailability.EventActive;

        return OCAvailability.Available;
    }
}
