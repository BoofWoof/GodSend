using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

public class CursorStateControl : MonoBehaviour
{
    public static CursorStateControl ActiveCursorController;

    private static int _CursorRequest;

    public void RequestCursor()
    {
        _CursorRequest++;
        ShowMouse();
    }

    public void ReleaseCursor()
    {
        _CursorRequest--;
        if(_CursorRequest == 0)
        {
            HideMouse();
        }
    }

    public static bool isCursorActive()
    {
        return _CursorRequest > 0;
    }

    public void ShowMouse()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        HudScript.instance.Reticle.SetActive(false);
    }

    public void HideMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        HudScript.instance.Reticle.SetActive(true);
    }

    public void Awake()
    {
        ActiveCursorController = this;

        HideMouse();

        PhonePositionScript.PhoneToggled += PhoneToggle;

        _CursorRequest = 0;
    }

    private void OnDestroy()
    {
        PhonePositionScript.PhoneToggled -= PhoneToggle;
    }

    public void PhoneToggle(bool phoneUp)
    {
        if (phoneUp)
        {
            RequestCursor();
        } else
        {
            ReleaseCursor();
        }
    }
}
