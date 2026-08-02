using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    public class SequencerCommandWalkTo : SequencerCommand
    {
        public void Awake()
        {
            GetSubject(1).GetComponentInChildren<OverworldPositionScript>().StartWalkTo(GetParameterAsInt(0));
            Stop();
        }

    }

}
