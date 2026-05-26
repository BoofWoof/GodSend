using System.Collections.Generic;
using UnityEngine;

public class TurretManagerScript : MonoBehaviour
{
    public static TurretManagerScript Instance;

    public List<TurretScript> Turrets;

    public int ActiveTurrets = 2;


    public void Awake()
    {
        Instance = this;
        UpdateTurretStates();
    }

    public static void IncreaseTurretCountStatic()
    {
        Instance.IncreaseTurretCount();
    }

    public void IncreaseTurretCount()
    {
        ActiveTurrets++;
        UpdateTurretStates();
    }

    public void UpdateTurretStates()
    {
        int turretCount = 0;

        foreach (TurretScript t in Turrets)
        {
            turretCount++;

            if(turretCount <= ActiveTurrets) t.gameObject.SetActive(true);
            else t.gameObject.SetActive(false);
        }
    }
}
