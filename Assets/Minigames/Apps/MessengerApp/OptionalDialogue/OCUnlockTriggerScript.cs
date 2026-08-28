using UnityEngine;

public class OCUnlockTriggerScript : MonoBehaviour
{
    public OCSO OCToRelease;
    private bool Released = false;
    public void Release()
    {
        if (Released) return;
        OCManager.instance.AddOC(OCToRelease);
    }
}
