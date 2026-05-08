using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct RotationLimits
{
    public float NegHor;
    public float PosHor;
    public float NegVer;
    public float PosVer;

    public RotationLimits(float NegHor, float PosHor, float NegVer, float PosVer)
    {
        this.NegHor = NegHor;
        this.PosHor = PosHor;
        this.NegVer = NegVer;
        this.PosVer = PosVer;
    }
}


public class LookScript : MonoBehaviour
{
    public float HeadLookWeight = 1f;

    public Transform EyeL;
    public Transform TargetEyeL;
    public Transform EyeR;
    public Transform TargetEyeR;
    public Transform Head;
    public Transform TargetHead;

    public bool FlipX = false;
    public bool FlipY = false;
    public bool FlipZ = false;

    public bool Aries = true;

    public float HeadYBias = -5f;

    [Header("Target")]
    public Transform Target;
    public Transform DistractionTarget = null;

    private RotationLimits LEyeRotLimits = new RotationLimits(-70f, 10f, -10f, 5f);
    private RotationLimits REyeRotLimits = new RotationLimits(-10f, 70f, -10f, 5f);
    public RotationLimits HeadRotLimits = new RotationLimits(-30f, 50f, -15f, 35f);


    [Header("Speed")]
    public float lookSpeed = 15f;
    private Quaternion initialHeadLocalRotation;
    private Quaternion initialLEyeLocalRotation;
    private Quaternion initialREyeLocalRotation;

    public float NearLookAwayDistance = 0.5f;
    public float LookAwayValue = 45f;

    public float MaxDistrationWait = 60f;
    public float MinDistrationWait = 30f;

    public List<Transform> DistractionPoints;
    private bool Distracted = false;
    public bool IsAries = true;
    public bool ForceDistracted = false;
    public static Transform ExternalDistractionPoint;
    public static Transform PrevExternalDistractionPoint;


    private void Awake()
    {
        if (TargetEyeL != null)
        {
            TargetEyeL.transform.parent = EyeL;
            initialLEyeLocalRotation = EyeL.localRotation;
        }
        if (TargetEyeR != null)
        {
            TargetEyeR.transform.parent = EyeR;
            initialREyeLocalRotation = EyeR.localRotation;
        }
        if (TargetHead != null) {
            TargetHead.transform.parent = Head;
            initialHeadLocalRotation = Head.localRotation;
        } 


        StartCoroutine(OccassionalDistraction());
    }

    public IEnumerator OccassionalDistraction()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(MinDistrationWait, MaxDistrationWait));
            int distractionIndex = UnityEngine.Random.Range(0, DistractionPoints.Count);
            Distracted = true;
            DistractionTarget = DistractionPoints[distractionIndex];
            if (ExternalDistractionPoint != null) DistractionTarget = ExternalDistractionPoint;
            yield return new WaitForSeconds(UnityEngine.Random.Range(3, 5));
            if (!ForceDistracted) DistractionTarget = null;
            yield return new WaitForSeconds(2f);
            if (!ForceDistracted) Distracted = false;
        }
    }
    public IEnumerator ForceDistraction()
    {
        ForceDistracted = true;
        yield return new WaitForSeconds(0.3f);
        Distracted = true;
        DistractionTarget = ExternalDistractionPoint;
        yield return new WaitForSeconds(UnityEngine.Random.Range(8, 12));
        DistractionTarget = null;
        yield return new WaitForSeconds(2f);
        Distracted = false;
        ForceDistracted = false;
    }

    void LateUpdate()
    {
        if (Target == null) return;
        if (PrevExternalDistractionPoint != ExternalDistractionPoint)
        {
            PrevExternalDistractionPoint = ExternalDistractionPoint;
            if(ExternalDistractionPoint != null) StartCoroutine(ForceDistraction());
        }

        if (Head != null) LookAt(Head, HeadYBias, HeadRotLimits, initialHeadLocalRotation, true);
        if (EyeL != null) LookAt(EyeL, 0f, LEyeRotLimits, initialLEyeLocalRotation);
        if (EyeR != null) LookAt(EyeR, 0f, REyeRotLimits, initialREyeLocalRotation);
    }

    public void SetLookWeight(float newWeight)
    {
        HeadLookWeight = newWeight;
    }

    void LookAt(Transform sourceTransform, float yOffset, RotationLimits rotLimits, Quaternion initialRotation, bool LookAwayCheck = false)
    {
        Transform selectedTarget = Target;
        float selectedLookSpeed = lookSpeed;
        if (Distracted && ((PrayerScript.instance.JudgementActive && !PrayerScript.instance.JudgementFocus) || !IsAries)) {
            if(DistractionTarget != null) selectedTarget = DistractionTarget;
            selectedLookSpeed = 2f;
        } 

        // Direction from bone to target in world space
        Vector3 directionToTarget = sourceTransform.position - selectedTarget.position;
        if (FlipX) directionToTarget.x = -directionToTarget.x;
        if (FlipY) directionToTarget.y = -directionToTarget.y;
        if (FlipZ) directionToTarget.z = -directionToTarget.z;

        // Convert target direction into the bone's local space
        Vector3 localDirection = sourceTransform.parent.InverseTransformDirection(directionToTarget.normalized);

        // Convert direction to angles
        float yaw = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;
        float pitch = -Mathf.Asin(localDirection.y) * Mathf.Rad2Deg;

        // Clamp angles
        yaw = HeadLookWeight * Mathf.Clamp(yaw, rotLimits.NegHor, rotLimits.PosHor);
        pitch = HeadLookWeight * Mathf.Clamp(pitch, rotLimits.NegVer, rotLimits.PosVer);

        float lookAwayYaw = 0f;
        if (LookAwayCheck)
        {
            float distance = Vector3.Distance(sourceTransform.position, selectedTarget.position);
            if (distance < NearLookAwayDistance)
            {
                float progress = 1 - (distance / NearLookAwayDistance);
                lookAwayYaw = progress * LookAwayValue;
            }
        }

        // Create rotation from the clamped angles
        Quaternion targetLocalRotation = Quaternion.Euler(pitch, yaw + lookAwayYaw, 0f);

        // Apply rotation relative to original pose
        Quaternion finalRotation = Quaternion.Euler(yOffset, 0, 0) * initialRotation * targetLocalRotation;


        sourceTransform.localRotation = Quaternion.Slerp(sourceTransform.localRotation, finalRotation, Time.deltaTime * selectedLookSpeed);
    }
}
