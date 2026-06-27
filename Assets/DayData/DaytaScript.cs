using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class DayInfo
{
    public static int CurrentDay { get; private set; }
    public static bool DaySet { get; private set; } = false;

    public static bool DayEndEnabled = false;

    public static void SetDay(int dayvalue)
    {
        CurrentDay = dayvalue;
        DaySet = true;
    }
}

public class DaytaScript : MonoBehaviour
{
    private static DaytaScript instance;

    public bool SkipStartInit = false;
    public static bool SkipStart = false;
    public static bool ExternalSkipStart = false;

    public int DayInit = 0;

    public AudioSource AudioBoom;

    public Image TitleCard;
    public TMP_Text TitleText;

    public void Awake()
    {
        instance = this;

        if (!DayInfo.DaySet)
        {
            DayInfo.SetDay(DayInit);
            SkipStart = SkipStartInit;
        } else
        {
            SkipStart = ExternalSkipStart;
        }

        Physics.gravity = Vector3.down * 9.8f;

        MenuTrigger.Reset();
    }

    public void AllowDayEnd()
    {
        DayInfo.DayEndEnabled = true;
    }

    public void Start()
    {
        if (SkipStart)
        {
            EnableCharacter();
            StartDay();
        }
        if (DayInfo.CurrentDay == 0)
        {
            CharacterActivationScript.DisableCharacter("MacroSid");
            CharacterActivationScript.DisableCharacter("MacroAlesssandro");
        }
        if (DayInfo.CurrentDay == 1 && !SkipStart)
        {
            CharacterActivationScript.DisableCharacter("MacroSid");
            CharacterActivationScript.DisableCharacter("MacroAlesssandro");
        }
        if (DayInfo.CurrentDay == 2 && !SkipStart)
        {
            CharacterActivationScript.DisableCharacter("MacroSid");
            CharacterActivationScript.DisableCharacter("MacroAries");
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
            CrossfadeScript.ResumeMusic();
            OverworldPositionScript.GoTo("A", 0);
        }
        if (DayInfo.CurrentDay == 1 && !SkipStart)
        {
            MusicSelectorScript.SetOverworldSong(5, true); //Instantly switch;
            CrossfadeScript.ResumeMusic();
            CrossfadeScript.SetLowpassOn(true, true);
            StartCoroutine(StartDayOne());
        }
        if (DayInfo.CurrentDay == 2 && !SkipStart)
        {
            StartCoroutine(StartDayTwo());
            CrossfadeScript.PauseMusic();
        }
    }

    public IEnumerator StartDayTwo()
    {
        TeleportPointScript.TeleportPlayerTo("Day2IntroPoint");

        QuestManager.ChangeQuest("Zzz");

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
