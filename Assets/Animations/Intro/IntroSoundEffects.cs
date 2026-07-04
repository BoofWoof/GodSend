using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class IntroSoundEffects : MonoBehaviour
{
    public AudioClip Gate;
    public AudioClip Step;

    private AudioSource ThisAudioSource;

    public UnityEvent OnCompleteEvent;

    //CameraControl
    public Camera TargetCamera;

    public SkyCamFlat SkyCamController;

    private UniversalAdditionalCameraData CameraData;
    private List<Camera> previousStack;


    public void Start()
    {
        ThisAudioSource = GetComponent<AudioSource>();

        if (DaytaScript.SkipStart || DayInfo.CurrentDay != 1)
        {
            DaytaScript.StaticStartDay();
            Destroy(gameObject);
            return;
        }

        CameraData = TargetCamera.GetUniversalAdditionalCameraData();

        previousStack = new List<Camera>(CameraData.cameraStack);
        CameraData.cameraStack.Clear();
        CameraData.cameraStack.Add(GetComponent<Camera>());

        SkyCamController.SetTargetCamera(GetComponent<Camera>());
    }

    public void TakeStep()
    {
        ThisAudioSource.clip = Step;
        ThisAudioSource.Play();
    }
    public void OpenGate()
    {
        ThisAudioSource.clip = Gate;
        ThisAudioSource.Play();
    }

    public void OnCutsceneCompletion()
    {
        OnCompleteEvent?.Invoke();

        CameraData.cameraStack.Clear();
        foreach (Camera c in previousStack)
        {
            CameraData.cameraStack.Add(c);
        }

        SkyCamController.SetDefaultTargetCamera();

        Destroy(gameObject);
    }
}
