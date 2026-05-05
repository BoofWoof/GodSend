using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class DayInfo
{
    public static int CurrentDay = 0;
    public static bool DayEndEnabled = false;
}

public class DaytaScript : MonoBehaviour
{
    private static DaytaScript instance; 

    public static bool SkipStart = false;
    public bool SkipStartInit = false;

    public bool ExteriorDaySet = false;
    public static bool ExternalSkipStart = false;

    public int DayInit = 0;

    public AudioSource AudioBoom;

    public Image TitleCard;
    public TMP_Text TitleText;

    public void Awake()
    {
        instance = this;
        if(!ExteriorDaySet) DayInfo.CurrentDay = DayInit;
        SkipStart = SkipStartInit;

        Physics.gravity = Vector3.down * 9.8f;

        MenuTrigger.Reset();
    }

    public void AllowDayEnd()
    {
        DayInfo.DayEndEnabled = true;
    }

    public void Start()
    {
        if (SkipStart || ExternalSkipStart)
        {
            EnableCharacter();
            StartDay();
        }
    }
    public static void StaticStartDay()
    {
        instance.StartDay();
    }
    public void StartDay()
    {
        if (DayInfo.CurrentDay == 0)
        {
            EnableCharacter();
            OverworldPositionScript.GoTo("A", 0);
        }
        if (DayInfo.CurrentDay == 1 && !SkipStart && !ExternalSkipStart)
        {
            MusicSelectorScript.SetOverworldSong(5, true); //Instantly switch;
            CrossfadeScript.ResumeMusic();
            CrossfadeScript.SetLowpassOn(true, true);
            StartCoroutine(StartDayOne());
        }
        if (DayInfo.CurrentDay == 2 && !SkipStart && !ExternalSkipStart)
        {
            StartCoroutine(StartDayTwo());
            CrossfadeScript.PauseMusic();
        }
    }

    public IEnumerator StartDayTwo()
    {
        TeleportPointScript.TeleportPlayerTo("Day2IntroPoint");

        PlayerCam.EnableCameraMovement = false;

        MessageQueue.addDialogue("D2Intro");

        OverworldPositionScript.GoTo("A", 12);

        CrossfadeScript.PauseMusic();

        yield return new WaitForSeconds(0.01f);

        CrossfadeScript.PauseMusic();

        yield return new WaitForSeconds(0);

        EnableCharacter();
    }

    public IEnumerator StartDayOne()
    {
        PlayerCam.EnableCameraMovement = false;

        CharacterSpeechScript.BroadcastForceGesture("MacroAries", "BannEnterPuff");

        TitleCard.gameObject.SetActive(true);
        AudioBoom.Play();

        yield return new WaitForSeconds(2);
        OverworldPositionScript.GoTo("A", 6);

        TitleText.gameObject.SetActive(true);
        AudioBoom.Play();

        yield return new WaitForSeconds(3);

        Destroy(TitleText.gameObject);
        Destroy(TitleCard.gameObject);

        MessageQueue.addDialogue("Day1Intro");

        EnableCharacter();
    }

    public void EnableCharacter()
    {
        PlayerCam.EnableCameraMovement = true;
        InputManager.GameStart();
    }
}
