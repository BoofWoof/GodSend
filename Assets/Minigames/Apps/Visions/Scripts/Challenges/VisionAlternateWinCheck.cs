using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VisionAlternateWinCheck : MonoBehaviour
{
    [Serializable]
    public class AltWinGroupData
    {
        public string WinningGroupName;
        public VisionEmptyGroup checkTarget;
        [TextArea] public string NewEndText;
        public UnityEvent OnWinEvents;
    }
    public List<AltWinGroupData> AltWinOptions;

    public VisionChallengeScript TargetChallenge;
    public void CheckEmptyGroup()
    {
        foreach(AltWinGroupData altWinGroupData in AltWinOptions)
        {
            if(altWinGroupData.WinningGroupName.ToLower() == altWinGroupData.checkTarget.AcceptedGroupName.ToLower())
            {
                TargetChallenge.ChangeExitText(altWinGroupData.NewEndText);
                altWinGroupData.OnWinEvents?.Invoke();
                return;
            }
        }
    }
}
