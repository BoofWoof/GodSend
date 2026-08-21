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
        float waitDivider = 1f;
        for (int i = 0; i < levelData.LevelWaves.Count; i++)
        {
            float modifiedWaitPeriod = WaitPeriod / waitDivider;

            ADWaveInfoSO waveInfo = Instantiate(levelData.LevelWaves[i]);
            FallingThreatScript.WaveSpeedModifier = waveInfo.WaveSpeedModifier;

            if (waveInfo.OnStartBroadcasts.Count > 0)
            {
                ActiveBroadcast.BroadcastActivation(waveInfo.OnStartBroadcasts);
            }

            if (waveInfo.PeekTime > 0)
            {
                ChannelChanger.instance.ScreenPositionChange(true);
                yield return new WaitForSeconds(waveInfo.PeekTime);
                ChannelChanger.instance.ScreenPositionChange(false);
            }

            textController.TurnOn(waveInfo, levelData, i+1);

            float timePassed = 0;
            while(timePassed < modifiedWaitPeriod)
            {
                timePassed += Time.deltaTime;
                float progress = timePassed / modifiedWaitPeriod;

                TimerBar1.fillAmount = 1f - progress;
                TimerBar2.fillAmount = 1f - progress;
                yield return null;
            }
            waitDivider = 2f;

            textController.TurnOff();

            waveInfo.SpawnWave(transform);

            while (!WaveClearCheck())
            {
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(0.2f);

            if (waveInfo.OnEndBroadcasts.Count > 0)
            {
                ActiveBroadcast.BroadcastActivation(waveInfo.OnEndBroadcasts);
            }

            if (waveInfo.PeekExitTime > 0)
            {
                ChannelChanger.instance.ScreenPositionChange(true);
                yield return new WaitForSeconds(waveInfo.PeekExitTime);
                if (i < levelData.LevelWaves.Count - 1) ChannelChanger.instance.ScreenPositionChange(false);
            }
        }

        AerialDefenseScript.Instance.WinOutcomes();
        AerialDefenseScript.Instance.StopWave();
    }

    public bool WaveClearCheck()
    {
        if(FallingThreatScript.isEnemiesRemaining())
        {
            //Debug.Log("Remaining Enemies");
            return false;
        }
        Debug.Log("Clear");
        return true;
    }
}
