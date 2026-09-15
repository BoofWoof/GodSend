using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VisionCompletionTrigger : MonoBehaviour
{
    public VisionChallengeScript TargetChallenge;
    public List<TriggerRanges> TriggerList;

    [Serializable]
    public class TriggerRanges
    {
        public Vector2 TriggerRange;
        public UnityEvent TriggerEvents;
    }

    public void Start()
    {
        TargetChallenge.SubscribeToSolutionCheck(PercentDoneCheck);
    }

    public void OnDestroy()
    {
        TargetChallenge.UnSubscribeToSolutionCheck(PercentDoneCheck);
    }

    public void PercentDoneCheck(float percentComplete)
    {
        foreach (TriggerRanges targetTrigger in TriggerList)
        {
            if(targetTrigger.TriggerRange.x <= percentComplete && targetTrigger.TriggerRange.y > percentComplete)
            {
                targetTrigger.TriggerEvents?.Invoke();
            }
        }
    }
}
