using UnityEngine;

public class SpinScript : MonoBehaviour
{
    public float rotationSpeed = 720f;

    public bool RotateX = false;
    public bool RotateY = true;
    public bool RotateZ = false;
    void Update()
    {
        float rotation = rotationSpeed * Time.deltaTime;

        transform.Rotate(RotateX ? rotation : 0f, RotateY ? rotation : 0f, RotateZ ? rotation : 0f, Space.Self);
    }
}
