using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPointScript : MonoBehaviour
{
    public static Dictionary<string, TeleportPointScript> TeleportPoints = new Dictionary<string, TeleportPointScript>();

    public string TeleportPointName;

    public void Awake()
    {
        TeleportPoints.Add(TeleportPointName, this);
    }


    public static void TeleportPlayerTo(string pointName)
    {
        if (!TeleportPoints.ContainsKey(pointName))
        {
            Debug.LogError("Teleport point does not exist.");
            return;
        }
        TeleportPointScript targetPoint = TeleportPoints[pointName];

        PlayerMovment.instance.TeleportPlayer(targetPoint.transform.position);
        PlayerCam.instance.ForceLook(targetPoint.transform.rotation);
    }
}
