using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ThreatSpawnerScript : MonoBehaviour
{
    public static ThreatSpawnerScript Instance;
    public static bool Spawning;

    public ADTextController textController;
    public Image TimerBar1;
    public Image TimerBar2;
    public float WaitPeriod = 6f;

    public void Awake()
    {
        Instance = this;
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
        if(FallingThreatScript.isEnemiesRemaining())
        {
            Debug.Log("Remaining Enemies");
            return false;
        }
        Debug.Log("Clear");
        return true;
    }
}
