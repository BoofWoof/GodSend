using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

public class ADTextController : MonoBehaviour
{
    public TMP_Text Title;
    public TMP_Text Flavor;
    public TMP_Text Details;


    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    public void TurnOn(ADWaveInfoSO waveInfo, AerialDefenseLevelData levelInfo, int currentWave)
    {
        gameObject.SetActive(true);
        Title.text = waveInfo.WaveName;

        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        string result = textInfo.ToTitleCase(waveInfo.WaveFlavor.ToLower());
        Flavor.text = result;

        Details.text = $"Speed: {waveInfo.WaveSpeedModifier.ToString("F1")}x         Stage: {currentWave}/{levelInfo.LevelWaves.Count}";
    }
}
