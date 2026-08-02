using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    public class SequencerCommandBehavior : SequencerCommand
    {
        public void Awake()
        {
            GetSubject(1).GetComponentInChildren<OverworldBehavior>().ExecuteSelfBehavior(GetParameter(0), 0);
            Stop();
        }

    }

}
