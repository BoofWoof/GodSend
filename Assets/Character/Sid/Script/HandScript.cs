using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class HandScript : MonoBehaviour
{
    public int ObjectIDToSpawn;

    public GameObject SpawnedObject;

    public bool HoldingObject = false;

    private bool RumbleOn = false;
    private Vector3 PreviousPosition;

    [ContextMenu("Spawn In Hand")] // Adds right-click menu option
    public void SpawnInHandInterface()
    {
        SpawnInHand(ObjectIDToSpawn);
    }

    public void SpawnInHand(int ID)
    {
        if (PropManager.instance == null || PropManager.instance.PropList.Length == 0)
        {
            Debug.LogWarning("No possible objects assigned.");
            return;
        }
        if (ObjectIDToSpawn < 0 || ObjectIDToSpawn >= PropManager.instance.PropList.Length)
        {
            Debug.LogWarning("Invalid object ID.");
            return;
        }

        SpawnedObject = Instantiate(PropManager.instance.PropList[ID]);
        SpawnedObject.transform.localScale = Vector3.one;
        SpawnedObject.transform.parent = transform;
        SpawnedObject.transform.localPosition = Vector3.zero;
        SpawnedObject.transform.localRotation = Quaternion.identity;

        HoldingObject = true;

        #if UNITY_EDITOR
                UnityEditor.Undo.RegisterCreatedObjectUndo(SpawnedObject, "Spawn Object In Hand");
        #endif
    }

    public void TurnOnMovementRumble()
    {
        if (RumbleOn) return;
        StartCoroutine(MovementRumbleIEnumerator());
    }
    public void TurnOffMovementRumble()
    {
        StopAllCoroutines();
        RumbleOn = false;
    }

    public IEnumerator MovementRumbleIEnumerator()
    {
        RumbleOn = true;

        PreviousPosition = transform.position;

        while (true)
        {
            yield return null;
            float distance = Vector3.Distance(PreviousPosition, transform.position);

            if (Time.deltaTime == 0) continue;

            float rumbleAdd = (distance / Time.deltaTime) / 100f;

            MoveCamera.moveCamera.AddMovementToRumble(rumbleAdd);
            Debug.Log(rumbleAdd);

            PreviousPosition = transform.position;
        }
    }

    public void Activate()
    {
        SpawnedObject.GetComponent<CarryableObject>().Activate();
    }

    public void DestroyHeldObject()
    {
        Destroy(SpawnedObject);

        HoldingObject = false;
    }

    public void ReleaseHandObject(int releaseIdx)
    {
        SpawnedObject.GetComponent<CarryableObject>().GoTo(releaseIdx);

        HoldingObject = false;
    }

    public void PickupHandObject(string objectName)
    {
        CarryableObject getCarryable = CarryableObject.GetCarryableObject(objectName);
        if (getCarryable == null) {
            Debug.Log($"No carryable object with this name:{objectName}");
            return;
        }
        getCarryable.Release();
        SpawnedObject = getCarryable.gameObject;

        Debug.Log(SpawnedObject);

        SpawnedObject.transform.parent = transform;
        SpawnedObject.transform.localPosition = Vector3.zero;
        SpawnedObject.transform.localRotation = Quaternion.identity;

        HoldingObject = true;
    }
}
