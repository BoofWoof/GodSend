using UnityEngine;
using UnityEngine.Events;

public class VisionPieceCountTrigger : MonoBehaviour
{
    public int TriggerValue = 1;
    private bool Triggered = false;
    public UnityEvent OnPieceTrigger;

    void Update()
    {
        //This is a very lazy implementation, but it works and it only exists for the challenge, so whatever.

        if (Triggered) return;

        int triggerCount = 0;

        foreach (PieceHolderScript piece in TurkPuzzleScript.puzzlePiece)
        {
            if (piece.EverReleased) triggerCount++;
        }

        if(triggerCount >= TriggerValue)
        {
            Debug.Log("Piece Count Trigger");
            Triggered = true;
            OnPieceTrigger?.Invoke();
        }
    }
}
