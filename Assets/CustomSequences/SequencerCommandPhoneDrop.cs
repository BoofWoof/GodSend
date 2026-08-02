using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    public class SequencerCommandPhoneDrop : SequencerCommand
    {
        public void Awake()
        {
            if (PhonePositionScript.raised) PhonePositionScript.instance.ForceTogglePhone();
            Stop();
        }

    }

}
