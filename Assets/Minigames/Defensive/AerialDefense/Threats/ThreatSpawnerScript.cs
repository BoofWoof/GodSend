using System.Collections;
using UnityEngine;


public class ThreatSpawnerScript : MonoBehaviour
{
    public static bool Spawning;

    public void Awake()
    {
        Spawning = false;
    }

    public void StartWave(AerialDefenseLevelData levelData)
    {

        StartCoroutine(RunWave(levelData));
    }

    public IEnumerator RunWave(AerialDefenseLevelData levelData)
    {
        Spawning = true;

        ADWaveInfoSO waveInfo = Instantiate(levelData.LevelWaves[AerialDefenseScript.Instance.CurrentWave]);

        while (waveInfo.GetContinueSpawn())
        {
            yield return new WaitForSeconds(waveInfo.GetWait());
            waveInfo.SpawnWave(transform);
        }

        Spawning = false;
    }

    public bool WaveClearCheck()
    {
        if (Spawning)
        {
            Debug.Log("Still Spawning");
            return false;
        }
        foreach(Transform child in transform)
        {
            if (child.gameObject != null)
            {
                Debug.Log("Remaining Enemies");
                return false;
            }
        }
        Debug.Log("Clear");
        return true;
    }
}
