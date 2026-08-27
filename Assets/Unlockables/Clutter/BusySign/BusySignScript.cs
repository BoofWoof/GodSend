using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BusySignScript : MonoBehaviour
{
    private enum SignMode
    {
        Leisure = 0,
        Meeting = 90,
        Danger = 180,
        Challenge = 270
    }

    public Transform RotatingSign;
    public MeshRenderer LightRendererSource;

    private bool DangerActive;
    private bool ConversationActive;
    private bool ChallengeActive;

    private SignMode currentMode = SignMode.Leisure;

    private Coroutine rotationCoroutine = null;
    public AnimationCurve rotationCurve;
    public float rotationPeriod = 1.5f;

    public UnityEvent StartDangerEvents;

    public UnityEvent PostDangerEvents;

    public UnityEvent StartRotate;

    private bool WasDanger = false;

    public void OnEnable()
    {
        GameStateMonitor.OnDangerChange += OnDangerChange;
        GameStateMonitor.OnChallengeChange += OnChallengeChange;
        ConversationManagerScript.OnConversationChange += OnConversationChange;
    }

    public void OnDisable()
    {
        GameStateMonitor.OnDangerChange -= OnDangerChange;
        GameStateMonitor.OnChallengeChange -= OnChallengeChange;
        ConversationManagerScript.OnConversationChange -= OnConversationChange;
    }

    private void OnDangerChange(bool dangerBool)
    {
        DangerActive = dangerBool;

        CheckRotationUpdate();
    }

    private void OnConversationChange(bool conversationBool)
    {
        ConversationActive = conversationBool;

        CheckRotationUpdate();
    }

    private void OnChallengeChange(bool challengeBool)
    {
        ChallengeActive = challengeBool;

        CheckRotationUpdate();
    }

    private void CheckRotationUpdate()
    {
        Debug.Log(gameObject.name);
        SignMode newMode = SignMode.Leisure;
        if(ConversationActive) newMode = SignMode.Meeting;
        if(DangerActive) newMode = SignMode.Danger;
        if (ChallengeActive) newMode = SignMode.Challenge;

        if (newMode == currentMode) return;

        if (SignMode.Danger == newMode) WasDanger = true;

        Debug.Log($"Switching sign to mode: {newMode}");

        if(rotationCoroutine != null) StopCoroutine(rotationCoroutine);
        rotationCoroutine = StartCoroutine(RotationAnimation(newMode, currentMode));

        currentMode = newMode;
    }

    private IEnumerator RotationAnimation(SignMode newMode, SignMode prevMode)
    {
        yield return new WaitForSeconds(1f);

        StartRotate?.Invoke();

        if (newMode != SignMode.Danger && WasDanger)
        {
            PostDangerEvents?.Invoke();
            WasDanger = false;
        }

        float targetRotation = (int)newMode;

        float timePassed = 0f;

        float startRotation = RotatingSign.localRotation.eulerAngles.z;

        while (timePassed < rotationPeriod)
        {
            timePassed += Time.deltaTime;
            float progress = timePassed / rotationPeriod;
            float rotationProgress = rotationCurve.Evaluate(progress);

            float rotation = Mathf.LerpUnclamped(startRotation, targetRotation, rotationProgress);

            RotatingSign.localRotation = Quaternion.Euler(0f, 0f, rotation);

            yield return null;
        }
    }
}
