using UnityEngine;

public class ToggleActive : MonoBehaviour
{

    public bool StartActive = true;

    public GameObject targetGameObject = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (targetGameObject == null) targetGameObject = gameObject;
        targetGameObject.SetActive(StartActive);
    }

    public void ToggleState()
    {
        targetGameObject.SetActive(!gameObject.activeSelf);
    }
}
