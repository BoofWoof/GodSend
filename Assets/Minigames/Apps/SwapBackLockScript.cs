using UnityEngine;
using UnityEngine.Events;

public class SwapBackLockScript : MonoBehaviour
{
    public UnityEvent ReplacedEvent;

    public void ActivateSwapBackFreeze(bool subscribe)
    {
        SharedAppData.BackLock = true;
        if(subscribe) SharedAppData.ReplacementBack.AddListener(TriggerReplacementFunction);
    }

    public void ReleaseSwapBackFreeze()
    {
        SharedAppData.BackLock = false;
        SharedAppData.ReplacementBack.RemoveListener(TriggerReplacementFunction);
    }

    public void TriggerReplacementFunction()
    {
        ReplacedEvent?.Invoke();
    }
}
