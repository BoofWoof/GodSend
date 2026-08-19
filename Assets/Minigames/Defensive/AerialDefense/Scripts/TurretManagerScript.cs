using System.Collections.Generic;
using UnityEngine;

public class TurretManagerScript : MonoBehaviour
{
    public static TurretManagerScript Instance;

    public List<TurretScript> Turrets;
    public List<TurretScript> LeftTurrets;
    public List<TurretScript> RightTurrets;

    public int ActiveTurrets = 2;

    public AudioSource ErrorSoundSource;

    private int CurrentFireIndex = 0;
    private bool SearchForNextTurret = true;

    public bool AutoFire;

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
    public void Update()
    {
        if (SearchForNextTurret)
        {
            FindNextAvailableTurret();
        }

        if (Input.GetMouseButtonDown(0) || AutoFire)
        {
            if (ADTargetScript.ValidTarget)
            {
                for (int i = 0; i < LeftTurrets.Count; i++)
                {
                    if (LeftTurrets[i].isActiveAndEnabled) LeftTurrets[i].FireBeam();
                }
                FireError();
            }
        }
        if (Input.GetMouseButtonDown(1) || AutoFire)
        {
            if (ADTargetScript.ValidTarget)
            {
                for (int i = 0; i < RightTurrets.Count; i++)
                {
                    if (RightTurrets[i].isActiveAndEnabled) RightTurrets[i].FireBeam();
                }
                FireError();
            }
        }
    }

    public void UpdateFireBeams()
    {
        /*
        for (int i = 0; i < ActiveTurrets; i++)
        {
            Turrets[i].AimBeam.SetActive(i == CurrentFireIndex);
        }
        SearchForNextTurret = false;
        */
    }

    public void ClearFireBeams()
    {
        /*
        for (int i = 0; i < ActiveTurrets; i++)
        {
            Turrets[i].AimBeam.SetActive(false);
        }
        SearchForNextTurret = true;
        */
    }

    public bool FindNextAvailableTurret()
    {
        for (int i = 0; i < ActiveTurrets; i++)
        {
            int turretIdx = (CurrentFireIndex + i) % ActiveTurrets;
            if (Turrets[turretIdx].IsTurretCharged())
            {
                CurrentFireIndex = turretIdx;
                UpdateFireBeams();
                return true;
            }
        }
        ClearFireBeams();
        return false;
    }

    public void FireError()
    {
        ErrorSoundSource.Play();
    }
}
