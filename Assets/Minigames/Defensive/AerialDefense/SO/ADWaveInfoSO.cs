using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WaveInfoSO", menuName = "AerialDefense/WaveInfoSO")]
public class ADWaveInfoSO : ScriptableObject
{
    public GameObject LevelPrefab;
    [HideInInspector] public int CurrentWave = 0;
    public bool BossWave = false;

    public string WaveName = "";
    public string WaveFlavor = "";

    public float WaveSpeedModifier = 1f;

    public void SpawnWave(Transform targetParent)
    {

        GameObject NewWave = Instantiate(LevelPrefab, targetParent);
        NewWave.transform.localPosition = Vector3.zero;
        NewWave.transform.localRotation = Quaternion.identity;

        CurrentWave++;
    }
}
