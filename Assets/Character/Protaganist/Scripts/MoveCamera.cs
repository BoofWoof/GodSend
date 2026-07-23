using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;

    public AnimationCurve amplitudeCurve;
    private Vector3 cameraShakeOffset;
    public ParticleSystem dustGenerator;

    public static MoveCamera moveCamera;
    private static float ImpactRumble;

    private static float TargetWordRumble { get; set; }
    private static float CurrentWordRumble { get; set; }

    private static float TargetMaintainRumble { get; set; }
    private static float CurrentMaintainRumble { get; set; }

    private static float MovementRumble { get; set; }

    public static float TotalRumble { get; set; }

    public float falloffRate = 1f;

    public AudioSource earthquakeSoundSource;
    public AudioClip[] earthquakeSoundOptions;
    public AudioSource rumbleSoundSource;

    public bool heldObject = false;
    public float MovementDecayRate = 2f;
    public AudioSource HandEmptyRumble;
    public AudioSource HeldRumble;
    private AudioSource TargetAudioRumbleSource;

    public static float VibrationIntensity = 1f;

    private void Start()
    {
        TargetWordRumble = 0;
        CurrentWordRumble = 0;
        TargetMaintainRumble = 0;
        CurrentMaintainRumble = 0;
        MovementRumble = 0;
        TargetAudioRumbleSource = HandEmptyRumble;

        moveCamera = this;
    }
    void Update()
    {
        if (TargetWordRumble > CurrentWordRumble) CurrentWordRumble = TargetWordRumble;
        CurrentWordRumble = CurrentWordRumble * 1 / (1 + Time.deltaTime * 10f);

        if (TargetMaintainRumble >= CurrentMaintainRumble) CurrentMaintainRumble = TargetMaintainRumble;
        else CurrentMaintainRumble = Mathf.MoveTowards(CurrentMaintainRumble, TargetMaintainRumble, Time.deltaTime * falloffRate);

        MovementRumble = MovementRumble * Mathf.Exp(-MovementDecayRate * Time.deltaTime);
        if (MovementRumble > 0.2f) MovementRumble = 0.2f;

        TotalRumble = Time.timeScale * VibrationIntensity * (ImpactRumble + CurrentMaintainRumble + CurrentWordRumble + (MovementRumble/5f));
        if (PhonePositionScript.raised) TotalRumble = 0;
        transform.position = cameraPosition.position + TotalRumble * Random.insideUnitSphere;

        float MaintainRumbles = CurrentMaintainRumble + CurrentWordRumble;
        rumbleSoundSource.volume = MaintainRumbles * 10f;
        rumbleSoundSource.pitch = 1 + MaintainRumbles * 2f;

        float movementRumbleAudio = MovementRumble * 3f;
        if (heldObject) movementRumbleAudio += 0.4f;
        TargetAudioRumbleSource.volume = movementRumbleAudio;

        TargetWordRumble = 0;
    }

    public void SetToHeld(bool held)
    {
        if (held == heldObject) return;

        heldObject = held;

        TargetAudioRumbleSource.Pause();
        TargetAudioRumbleSource.volume = 0f;

        if (held)
            TargetAudioRumbleSource = HeldRumble;
        else
            TargetAudioRumbleSource = HandEmptyRumble;

        TargetAudioRumbleSource.Play();
        TargetAudioRumbleSource.volume = MovementRumble * 3f;

    }
    public void AddMovementToRumble(float additionalMovemet)
    {
        float movementAmount = additionalMovemet;
        if (heldObject) movementAmount *= 3f;
        MovementRumble += additionalMovemet;
    }

    public static void SetVibrationIntensity(float newIntensity)
    {
        VibrationIntensity = newIntensity;
    }

    public void TestShake(float durationSec)
    {
        ImpactShakeScreen(durationSec, 1);
    }
    public static void SetMaintainRumble(float rumbleQuantity)
    {
        TargetMaintainRumble = rumbleQuantity;
    }
    public static void SetWordRumble(float rumbleQuantity)
    {
        if (TargetWordRumble < rumbleQuantity)
        {
            TargetWordRumble = rumbleQuantity;
        }
    }
    public void ImpactShakeScreen(float durationSec, float shakeAmplitude = 1)
    {
        moveCamera.StartCoroutine(moveCamera.AddScreenShake(durationSec, shakeAmplitude));
    }

    public IEnumerator AddScreenShake(float durationSec, float shakeAmplitude = 1)
    {
        dustGenerator.Play();
        earthquakeSoundSource.volume = shakeAmplitude;
        AudioClip randomClip = earthquakeSoundOptions[Random.Range(0, earthquakeSoundOptions.Length)];

        earthquakeSoundSource.clip = randomClip;
        earthquakeSoundSource.Play();

        float timePassedSec = 0;
        while (timePassedSec < durationSec)
        {
            yield return null;
            timePassedSec += Time.deltaTime;
            ImpactRumble = shakeAmplitude * amplitudeCurve.Evaluate(timePassedSec / durationSec);
        }

        ImpactRumble = 0;

        StartCoroutine(AudioFadeOut.FadeOut(earthquakeSoundSource, 2.5f));
    }
}
