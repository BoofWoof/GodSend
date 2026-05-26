using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AppScript : MonoBehaviour
{
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

    public void OnEnable()
    {
        PhonePositionScript.PhoneToggled += TryRaiseRefresh;
    }

    public void OnDisable()
    {
        PhonePositionScript.PhoneToggled -= TryRaiseRefresh;
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
        if (AppAnimator.instance.TransitionActive || !Active || PreviousApp == null) return;

        if (!Input.GetMouseButton(0)) AppAnimator.instance.SwitchToAppStart(PreviousApp, -Vector3.up * 900f);
    }

    public static void Swap(AppScript newApp)
    {
        if (newApp == null) return;
        newApp.PreviousApp = ActiveApp;
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
