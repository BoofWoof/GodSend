using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrayerStatueScript : MonoBehaviour
{
    public GameObject PrayerScreen;
    public GameObject Glow;

    public float LowHeight = 0f;
    public float HighHeight = 1f;
    private float CurrentHeight = 0f;

    public float RaisePeriod = 1f;

    private Renderer renderer;
    private MaterialPropertyBlock block;
    private MaterialPropertyBlock backupBlock;

    private Coroutine coroutine;

    public static List<PrayerStatueScript> AllStatues = new();

    public void Start()
    {
        renderer = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();
        backupBlock = new MaterialPropertyBlock();

        renderer.GetPropertyBlock(backupBlock, 3);

        PrayerScreen.SetActive(false);
        Glow.SetActive(false);
        renderer.GetPropertyBlock(block, 3);
        block.SetColor("_EmissionColor", Color.black); // Example: Change color
        renderer.SetPropertyBlock(block, 3);
    }

    public void OnEnable()
    {
        AllStatues.Add(this);
    }

    public void OnDisable()
    {
        AllStatues.Remove(this);
    }

    public static void SetAllScrens(bool on)
    {
        foreach (PrayerStatueScript statue in AllStatues)
        {
            statue.PrayerScreen.SetActive(on);
        }
    }

    public void StatueOn()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(TurnOnStatue());
    }

    IEnumerator TurnOnStatue()
    {
        float timePassedSec = 0f;
        float startHeight = CurrentHeight;
        while (timePassedSec < RaisePeriod)
        {
            timePassedSec += Time.deltaTime;
            CurrentHeight = Mathf.Lerp(startHeight, HighHeight, timePassedSec/RaisePeriod);
            transform.localPosition = Vector3.up * CurrentHeight;
            PrayerScreen.transform.localPosition = Vector3.up * CurrentHeight;
            yield return null;
        }

        PrayerScreen.SetActive(true);
        Glow.SetActive(true);
        renderer.SetPropertyBlock(backupBlock, 3);
    }

    public void StatueOff()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(TurnOffStatue());
    }
    IEnumerator TurnOffStatue()
    {
        PrayerScreen.SetActive(false);
        Glow.SetActive(false);
        renderer.GetPropertyBlock(block, 3);
        block.SetColor("_EmissionColor", Color.black); // Example: Change color
        renderer.SetPropertyBlock(block, 3);

        float timePassedSec = 0f;
        float startHeight = CurrentHeight;
        while (timePassedSec < RaisePeriod)
        {
            timePassedSec += Time.deltaTime;
            CurrentHeight = Mathf.Lerp(startHeight, LowHeight, timePassedSec / RaisePeriod);
            transform.localPosition = Vector3.up * CurrentHeight;
            PrayerScreen.transform.localPosition = Vector3.up * CurrentHeight;
            yield return null;
        }
    }

}
