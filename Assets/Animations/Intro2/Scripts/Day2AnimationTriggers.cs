using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Day2AnimationTriggers : MonoBehaviour
{
    public GameObject ContainingObject;
    public Camera TargetCamera;

    public SkyCamFlat SkyCamController;

    private UniversalAdditionalCameraData CameraData;
    private List<Camera> previousStack;

    public void Start()
    {
        if (DayInfo.CurrentDay != 2)
        {
            Destroy(ContainingObject);
            return;
        }

        CameraData = TargetCamera.GetUniversalAdditionalCameraData();

        previousStack = new List<Camera>(CameraData.cameraStack);
        CameraData.cameraStack.Clear();
        CameraData.cameraStack.Add(GetComponent<Camera>());

        SkyCamController.SetTargetCamera(GetComponent<Camera>());
    }

    public void OnAnimationEnd()
    {
        CameraData.cameraStack.Clear();
        foreach (Camera c in previousStack)
        {
            CameraData.cameraStack.Add(c);
        }

        SkyCamController.SetDefaultTargetCamera();

        Destroy(ContainingObject);
    }


}
