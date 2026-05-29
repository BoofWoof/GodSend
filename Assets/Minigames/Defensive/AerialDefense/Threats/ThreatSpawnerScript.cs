using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ThreatSpawnerScript : MonoBehaviour
{
    public static bool Spawning;

    public ADTextController textController;
    public Image TimerBar1;
    public Image TimerBar2;
    public float WaitPeriod = 6f;

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

        for (int i = 0; i < levelData.LevelWaves.Count; i++)
        {
            ADWaveInfoSO waveInfo = Instantiate(levelData.LevelWaves[i]);

            textController.TurnOn(waveInfo, levelData, i+1);

            float timePassed = 0;
            while(timePassed < WaitPeriod)
            {
                timePassed += Time.deltaTime;
                float progress = timePassed / WaitPeriod;

                TimerBar1.fillAmount = 1f - progress;
                TimerBar2.fillAmount = 1f - progress;
                yield return null;
            }

            textController.TurnOff();

            while (waveInfo.GetContinueSpawn())
            {
                yield return new WaitForSeconds(waveInfo.GetWait());
                waveInfo.SpawnWave(transform);
            }
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
