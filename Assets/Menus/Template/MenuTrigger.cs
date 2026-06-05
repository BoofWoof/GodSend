using UnityEngine;

public class MenuTrigger : MonoBehaviour
{
    private static int MenuOpenCount = 0;
    private static int PartialMenuOpenCount = 0;

    private static float NormalTimescale = 1f;

    private static bool Paused = false;
    public bool PartialPause = false;

    private bool CursorRequestActive = false;

    public static void Reset()
    {
        MenuOpenCount = 0;
        NormalTimescale = 1f;
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
        NormalTimescale = Time.timeScale;
    }
    public void PauseGame()
    {
        Paused = true;

        AudioListener.pause = true;
        Time.timeScale = 0;
        InputManager.AllOff();

        if (!CursorRequestActive)
        {
            CursorRequestActive = true;
            CursorStateControl.ActiveCursorController.RequestCursor();
        }
    }

    public void PartialPauseGame()
    {
        Paused = true;

        AudioListener.pause = false;
        Time.timeScale = NormalTimescale;
        InputManager.AllOff();

        if (!CursorRequestActive)
        {
            CursorRequestActive = true;
            CursorStateControl.ActiveCursorController.RequestCursor();
        }
    }

    public void UnPauseGame()
    {
        Paused = false;

        AudioListener.pause = false;
        Time.timeScale = NormalTimescale;
        InputManager.AllOn();

        if (CursorRequestActive)
        {
            CursorRequestActive = false;
            CursorStateControl.ActiveCursorController.ReleaseCursor();
        }
    }
}