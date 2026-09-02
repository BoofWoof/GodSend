using UnityEngine;

public class AppSwapTriggerScript : MonoBehaviour
{
    public string AppName;
    public void Trigger()
    {
        AppScript.Swap(AppScript.AppsDict[AppName]);
    }
}
