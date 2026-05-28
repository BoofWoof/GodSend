using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class FormationDropInfo
{
    public GameObject FormationPrefab;
    public float DelayTillRelease = 0f;
    public float Offest = 0f;
    public bool Flip = false;
    public float FormationSpeedModifier = 1f;
}


[CreateAssetMenu(fileName = "WaveInfoSO", menuName = "AerialDefense/WaveInfoSO")]
public class ADWaveInfoSO : ScriptableObject
{
    public List<FormationDropInfo> DropInfo;
    [HideInInspector] public int CurrentWave = 0;
    public bool BossWave = false;

    public string WaveName = "";
    public string WaveFlavor = "";

    public float WaveSpeedModifier = 1f;

    public void SpawnWave(Transform targetParent)
    {
        if (CurrentWave >= DropInfo.Count)
        {
            Debug.Log("Attempted to spawn invalid weve.");
            return;
        }
        GameObject NewWave = Instantiate(DropInfo[CurrentWave].FormationPrefab, targetParent);
        NewWave.transform.localPosition = Vector3.zero;
        NewWave.transform.localRotation = Quaternion.identity;

        for (int i = NewWave.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = NewWave.transform.GetChild(i);
            child.SetParent(targetParent);
            child.transform.localPosition += Vector3.right * DropInfo[CurrentWave].Offest;
            FallingThreatScript threatScript = child.GetComponent<FallingThreatScript>();
            threatScript.FormationSpeedModifier = DropInfo[CurrentWave].FormationSpeedModifier;
            threatScript.WaveSpeedModifier = WaveSpeedModifier;
        }
        Destroy(NewWave);

        CurrentWave++;
    }

    public bool GetContinueSpawn()
    {
        return CurrentWave < DropInfo.Count;
    }

    public float GetWait()
    {
        if(CurrentWave >= DropInfo.Count)
        {
            Debug.Log("Attempted to grab invalid weit.");
            return 0f;
        }
        return DropInfo[CurrentWave].DelayTillRelease;
    }
}
