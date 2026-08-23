using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AppScript : MonoBehaviour
{
    private static string DefaultApp = "AppMenu";

    public Sprite AssociatedIcon;

    public Transform ActiveTarget;
    public Transform InactiveTarget;

    public string AppName;
    public static string ActiveAppName = "";

    public GameObject AppRoot;
    public AppScript PreviousApp;

    public bool HideOnStart = true;
    public bool Active = true;
    public static AppScript ActiveApp;

    public delegate void HideApp();
    public event HideApp OnHideApp;
    public delegate void ShowApp();
    public event ShowApp OnShowApp;

    public UnityEvent OnActivateApp;
    public UnityEvent OnDeactivateApp;
    public UnityEvent OnRaiseRefreshApp;

    public static Dictionary<string, AppScript> AppsDict = new();

    public void Awake()
    {
        ActiveAppName = "";
        OnHideApp = null;
        OnShowApp = null;
    }
    public void Start()
    {
        if (HideOnStart)
        {
            Hide();
            RegisterInputActions();
        } else
        {
            AppAnimator.instance.CurrentDisplayedApp = this;
            Show();
        }
    }

    virtual public void OnEnable()
    {
        PhonePositionScript.PhoneToggled += TryRaiseRefresh;

        Debug.Log(AppName);
        AppsDict.Add(AppName, this);
    }

    virtual public void OnDisable()
    {
        PhonePositionScript.PhoneToggled -= TryRaiseRefresh;
        AppsDict.Remove(AppName);
    }

    public void OnDestroy()
    {
        InputManager.PlayerInputs.Phone.AppReturn.performed -= HidePressed;
        OnHideApp = null;
        OnShowApp = null;
    }

    public void RegisterInputActions()
    {
        InputManager.PlayerInputs.Phone.AppReturn.performed += HidePressed;
    }

    public void HidePressed(InputAction.CallbackContext c)
    {
        if (AppAnimator.instance.TransitionActive || !Active || AppName == DefaultApp) return;

        foreach(string name in AppsDict.Keys)
        {
            Debug.Log(name);
        }

        if (PreviousApp == null) PreviousApp = AppsDict[DefaultApp];

        if (!Input.GetMouseButton(0)) AppAnimator.instance.SwitchToAppStart(PreviousApp, -Vector3.up * 900f);
    }

    public static void Swap(AppScript newApp)
    {
        if (newApp == null) return;
        //newApp.PreviousApp = ActiveApp;
        AppAnimator.instance.SwitchToAppStart(newApp, Vector3.up * 900f);
        //newApp.Show(AppRoot);
        //Hide(false);
    }

    public void Show()
    {
        AppRoot.transform.SetParent(ActiveTarget);
        AppRoot.transform.localPosition = Vector3.zero;
        AppRoot.transform.localRotation = Quaternion.identity;
        AppRoot.transform.SetAsFirstSibling();

        ShowTriggers();
    }
    public void ShowTriggers()
    {
        Active = true;
        ActiveApp = this;

        OnActivateApp?.Invoke();
        OnShowApp?.Invoke();

        ActiveAppName = AppName;
    }

    public void Hide()
    {
        AppRoot.transform.SetParent(InactiveTarget);
        AppRoot.transform.localPosition = Vector3.zero;
        AppRoot.transform.localRotation = Quaternion.identity;

        HideTriggers();
    }

    public void HideTriggers()
    {
        Active = false;

        OnDeactivateApp?.Invoke();
        OnHideApp?.Invoke();
    }

    public void TryRaiseRefresh(bool raised)
    {
        if (raised && Active)
        {
            OnRaiseRefreshApp?.Invoke();
        }
    }

    public static bool CheckIfActive(string checkName)
    {
        return (checkName.ToLower() == ActiveAppName.ToLower() && PhonePositionScript.raised);
    }
}
