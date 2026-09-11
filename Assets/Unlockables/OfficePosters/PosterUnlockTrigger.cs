using UnityEngine;

public class PosterUnlockTrigger : MonoBehaviour
{
    public string PosterName;
    public void TriggerUnlock()
    {
        UnlockablesManager.UnlockPortrait(PosterName);
    }
}
