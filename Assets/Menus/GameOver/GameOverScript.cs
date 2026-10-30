using PixelCrushers;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
    public Image Panel;
    public GameObject Text1;
    public GameObject Text2;
    public GameObject Text3;
    public GameObject Buttons;
    public GameObject Explinations;

    public GameObject DeathHexImageObject;
    public Animator DeathHexAnimator;

    public GameObject ThreeDAssets;

    public SaveSystem TargetSaveSystem;

    public float FadePeriod;

    public void Awake()
    {
        ClearScreen();
    }
    public void StartGameOver()
    {
        gameObject.SetActive(true);
        StartCoroutine(GameoverCoroutine());
    }

    public void ClearScreen()
    {
        gameObject.SetActive(false);
        Panel.gameObject.SetActive(false);
        Panel.color = new Color(0, 0, 0, 0);
        Text1.SetActive(false);
        Text2.SetActive(false);
        Text3.SetActive(false);
        Buttons.SetActive(false);
        Explinations.SetActive(false);
        DeathHexImageObject.SetActive(false);
        ThreeDAssets.SetActive(false);

        HoverAscend(false);
        HoverDescend(false);
        ChoiceSelected(false);
        DeathHexAnimator.Play("DeathHex_Idle_Anim", 0, 0.0f);
    }

    IEnumerator GameoverCoroutine()
    {
        Debug.Log("GameoverStarted");

        Panel.gameObject.SetActive(true);

        float timePassed = 0f;

        while (timePassed < FadePeriod)
        {
            timePassed += Time.unscaledDeltaTime;
            Debug.Log(timePassed);
            Panel.color = new Color(0, 0, 0, timePassed/FadePeriod);
            yield return null;
        }
        Panel.color = new Color(0, 0, 0, 1);

        yield return new WaitForSecondsRealtime(2f);
        Text1.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        Text2.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        Text3.SetActive(true);
        ThreeDAssets.SetActive(true);
        DeathHexImageObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        Buttons.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);
        Explinations.SetActive(true);
    }

    public void ResumeGame()
    {
        ClearScreen();
        OverworldBehavior.BroadcastBehaviors("A", "instant_judge");
        CrossfadeScript.ResumeMusic();
        MusicSelectorScript.RevertOverworldSong();

        ActiveBroadcast.BroadcastActivation("PrayerRegret");

        DefenseStats.DamageCity(35f);
        AnnouncementScript.StartAnnouncement("Your lands are in ruin. Your life was nearly lost.");
        AnnouncementScript.StartAnnouncement("The ram still waits...");

        PrayerScript.instance.ForcePrayerReferesh();

        ActiveBroadcast.BroadcastActivation("ResetDisaster");
        ActiveBroadcast.BroadcastActivation("ResetDisaster1");
    }

    public void HoverAscend(bool hovered)
    {
        DeathHexAnimator.SetBool("AscendHover", hovered);
    }
    public void HoverDescend(bool hovered)
    {
        DeathHexAnimator.SetBool("DescendHover", hovered);
    }
    public void ChoiceSelected(bool selected)
    {
        DeathHexAnimator.SetBool("Selected", selected);
    }

    public void Ascend()
    {
        StartCoroutine(AscendEnumerator());
    }

    private IEnumerator AscendEnumerator()
    {
        DeathHexAnimator.Play("DeathHex_ConfirmAscend_Anim", 0, 0.0f);
        yield return new WaitForSecondsRealtime(3f);
        ResumeGame();
    }

    public void Descend()
    {
        StartCoroutine(DescendEnumerator());
    }
    private IEnumerator DescendEnumerator()
    {
        DeathHexAnimator.Play("DeathHex_ConfirmDescend_Anim", 0, 0.0f);
        yield return new WaitForSecondsRealtime(3f);
        TargetSaveSystem.LoadSceneAtSpawnpoint("MainMenu");
    }
}
