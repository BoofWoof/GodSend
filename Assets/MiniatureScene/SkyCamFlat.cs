using UnityEngine;

public class SkyCamFlat : MonoBehaviour
{
    public Camera SourceAngleCamera;
    private Camera ReplacementAngleCamera;

    public GameObject SkyCamera;

    public float yRotOffset;
    private Vector3 startingSourcePosition;
    private Vector3 startingTargetPosition;

    public void Start()
    {
        SetDefaultTargetCamera();
        startingTargetPosition = SkyCamera.transform.localPosition;
    }

    void LateUpdate()
    {
        Quaternion currentRotation = SkyCamera.transform.parent.rotation;
        Quaternion cameraTurn = Quaternion.Euler(0, yRotOffset, 0);
        Quaternion reverseCameraTurn = Quaternion.Euler(0, -yRotOffset, 0);

        SkyCamera.transform.rotation = cameraTurn * currentRotation * ReplacementAngleCamera.transform.rotation;

        Vector3 cameraShift = (startingSourcePosition - ReplacementAngleCamera.transform.position) / 10000f;
        Debug.Log(cameraShift);
        SkyCamera.transform.localPosition = startingTargetPosition + reverseCameraTurn * cameraShift;
    }

    public void SetDefaultTargetCamera()
    {
        SetTargetCamera(SourceAngleCamera);
    }

    public void SetTargetCamera(Camera targetCamera)
    {
        startingSourcePosition = targetCamera.transform.position;
        ReplacementAngleCamera = targetCamera;
    }
}
