using UnityEngine;

public class GeneralLoadInTriggers : MonoBehaviour
{
    public void Awake()
    {
        GameStateMonitor.ReleaseForceEvent();
        GameStateMonitor.ActivePrayer = false;
        GameStateMonitor.DangerActive = false;

        GameStateMonitor.OnEventChange = null;
        GameStateMonitor.PrevEventActive = false;
    }
}
