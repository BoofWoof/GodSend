using UnityEngine;

public class SkyCamFlat : MonoBehaviour
{
    public Camera SourceAngleCamera;

    public GameObject SkyCamera;

    public float yRotOffset;
    private Vector3 startingSourcePosition;
    private Vector3 startingTargetPosition;

    public void Start()
    {
        startingSourcePosition = SourceAngleCamera.transform.position;
        startingTargetPosition = SkyCamera.transform.localPosition;
    }

    void LateUpdate()
    {
        Quaternion currentRotation = SkyCamera.transform.parent.rotation;
        Quaternion cameraTurn = Quaternion.Euler(0, yRotOffset, 0);
        Quaternion reverseCameraTurn = Quaternion.Euler(0, -yRotOffset, 0);

        SkyCamera.transform.rotation = cameraTurn * currentRotation * SourceAngleCamera.transform.rotation;

        Vector3 cameraShift = (startingSourcePosition - SourceAngleCamera.transform.position) / 10000f;
        Debug.Log(cameraShift);
        SkyCamera.transform.localPosition = startingTargetPosition + reverseCameraTurn * cameraShift;
    }
}
