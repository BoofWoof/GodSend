using System.Collections.Generic;
using UnityEngine;

public class CursorStateControl : MonoBehaviour
{
    public static CursorStateControl ActiveCursorController;

    private static List<int> _CursorRequest = new();

    public void RequestCursor(int requesterID)
    {
        Debug.Log("Requesting Cursor");
        if(!_CursorRequest.Contains(requesterID)) _CursorRequest.Add(requesterID);
        ShowMouse();
    }

    public void ReleaseCursor(int requesterID)
    {
        Debug.Log("Releasing Cursor");
        if (_CursorRequest.Contains(requesterID)) _CursorRequest.Remove(requesterID);
        if (_CursorRequest.Count == 0)
        {
            HideMouse();
        }
    }

    public static bool isCursorActive()
    {
        return _CursorRequest.Count > 0;
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

        _CursorRequest = new();
    }

    private void OnDestroy()
    {
        PhonePositionScript.PhoneToggled -= PhoneToggle;
    }

    public void PhoneToggle(bool phoneUp)
    {
        //1337 is just a number to give the phone a unique number.
        if (phoneUp)
        {
            RequestCursor(1337);
        } else
        {
            ReleaseCursor(1337);
        }
    }
}
