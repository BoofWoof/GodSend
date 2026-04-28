using UnityEngine;

public class MenuTrigger : MonoBehaviour
{
    private static int MenuOpenCount = 0;
    private static int PartialMenuOpenCount = 0;

    private static float NormalTimescale = 1f;
    private static bool ReticlePreviousShown = true;

    private static bool Paused = false;

    public bool PartialPause = false;

    public static void Reset()
    {
        MenuOpenCount = 0;

        NormalTimescale = 1f;
        ReticlePreviousShown = true;
    }

    public void OnEnable()
    {
        if(PartialPause)
        {
            PartialMenuOpenCount++;
            PauseCheck();
            return;
        }
        MenuOpenCount++;
        PauseCheck();
    }

    public void OnDisable()
    {
        if (PartialPause)
        {
            PartialMenuOpenCount--;
            PauseCheck();
            return;
        }
        MenuOpenCount--;
        PauseCheck();
    }

    public void PauseCheck()
    {
        if(!Paused) RecordCurrentGameState();
        if (MenuOpenCount > 0)
        {
            PauseGame();
            return;
        } else if (PartialMenuOpenCount > 0)
        {
            PartialPauseGame();
            return;
        }

        UnPauseGame();
    }

    public static int GetMenuCount()
    {
        return MenuOpenCount;
    }

    public void RecordCurrentGameState()
    {
        ReticlePreviousShown = HudScript.instance.Reticle.activeInHierarchy;
        NormalTimescale = Time.timeScale;
        CursorStateControl.RecordCursorState();
    }
    public void PauseGame()
    {
        Paused = true;

        AudioListener.pause = true;
        CursorStateControl.AllowMouse(true);
        HudScript.instance.ShowReticle(false);
        Time.timeScale = 0;
        InputManager.AllOff();
    }

    public void PartialPauseGame()
    {
        Paused = true;

        AudioListener.pause = false;
        CursorStateControl.AllowMouse(true);
        ReticlePreviousShown = HudScript.instance.Reticle.activeInHierarchy;
        HudScript.instance.ShowReticle(false);
        Time.timeScale = NormalTimescale;
        InputManager.AllOff();
    }

    public void UnPauseGame()
    {
        Paused = false;

        AudioListener.pause = false;
        CursorStateControl.ResumeCursorState();
        HudScript.instance.ShowReticle(!ReticlePreviousShown);
        Time.timeScale = NormalTimescale;
        InputManager.AllOn();
    }
}