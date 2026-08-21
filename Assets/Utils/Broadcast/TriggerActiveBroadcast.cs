using System.Collections.Generic;
using UnityEngine;

public class TriggerActiveBroadcast : MonoBehaviour
{
    public List<BroadcastStruct> BroadcastData = new();


    public void OnTrigger()
    {
        foreach (BroadcastStruct data in BroadcastData)
        {
            ActiveBroadcast.BroadcastActivation(data);
        }
    }
}
