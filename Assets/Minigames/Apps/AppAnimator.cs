using System.Collections;
using UnityEngine;

public class AppAnimator : MonoBehaviour
{
    public static AppAnimator instance;

    public Transform SlideInPoint;
    public Transform SlideOutPoint;
    public Transform InactivePoint;

    public bool TransitionActive;

    public AppScript CurrentDisplayedApp;

    public float TransitionPeriod = 0.1f;

    public void Awake()
    {
        instance = this;
        TransitionActive = false;
    }

    public bool SwitchToAppStart(AppScript targetApp, Vector3 offsetDirection)
    {
        if (TransitionActive) return false;
        StartCoroutine(SwitchToApp(targetApp, offsetDirection));
        return true;
    }

    public IEnumerator SwitchToApp(AppScript targetApp, Vector3 offsetDirection)
    {
        targetApp.ShowTriggers();

        Transform targetAppTransform = targetApp.AppRoot.transform;
        Transform displayedAppTransform = CurrentDisplayedApp.AppRoot.transform;

        TransitionActive = true;

        SlideInPoint.localPosition = offsetDirection;
        targetAppTransform.SetParent(SlideInPoint);
        targetAppTransform.localPosition = Vector3.zero;
        targetAppTransform.localRotation = Quaternion.identity;

        SlideOutPoint.localPosition = Vector3.zero;
        displayedAppTransform.SetParent(SlideOutPoint);
        displayedAppTransform.localPosition = Vector3.zero;
        displayedAppTransform.localRotation = Quaternion.identity;

        float timePassed = 0;

        while(timePassed < TransitionPeriod)
        {
            timePassed += Time.deltaTime;
            float progress = timePassed / TransitionPeriod;

            SlideInPoint.localPosition = Vector3.Lerp(offsetDirection, Vector3.zero, progress);
            SlideOutPoint.localPosition = Vector3.Lerp(Vector3.zero, -offsetDirection, progress);

            yield return null;
        }

        SlideInPoint.localPosition = Vector3.zero;

        displayedAppTransform.SetParent(InactivePoint);
        displayedAppTransform.localPosition = Vector3.zero;
        displayedAppTransform.localRotation = Quaternion.identity;

        CurrentDisplayedApp.HideTriggers();

        CurrentDisplayedApp = targetApp;

        TransitionActive = false;
    }
}
