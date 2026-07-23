using System.Collections.Generic;
using UnityEngine;

public class AnimationTriggers : MonoBehaviour
{
    public OverworldPositionScript overworldPositionScript;
    public Animator animator;

    public HandScript RightHand;
    public HandScript LeftHand;
    public List<HandScript> ExtraPropHolders;

    public LookScript LookControl;
    private float LastHeadLookWeight;
    private float SetToLookWeight;

    private Transform LastHeadTarget;
    private Transform SetToTarget;

    public void SetLookWeight(float newWeight)
    {
        LastHeadLookWeight = LookControl.HeadLookWeight;
        LookControl.HeadLookWeight = newWeight;
        SetToLookWeight = newWeight;
    }
    public void ResumeLookWeight()
    {
        if (SetToLookWeight != LookControl.HeadLookWeight) return;
        LookControl.HeadLookWeight = LastHeadLookWeight;
    }

    public void FocusLeftSpawnedItem()
    {
        LastHeadTarget = LookControl.Target;
        LookControl.Target = LeftHand.SpawnedObject.transform;
        SetToTarget = LeftHand.SpawnedObject.transform;
    }
    public void ResumeLookTarget()
    {
        if (SetToTarget != LookControl.Target) return;
        LookControl.Target = LastHeadTarget;
    }

    public void Impact(float Strength)
    {
        MoveCamera.moveCamera.ImpactShakeScreen(1f, Strength);
    }

    public void Rumble(float Strength)
    {
        MoveCamera.SetMaintainRumble(Strength);
    }

    public void EnableMobility()
    {
        overworldPositionScript.CharacterMobile = true;
    }

    public void HoldCity() //This is now misnamed but annoying to fix. It is now holding anything.
    {
        animator.SetBool("Holding", true);
    }

    public void ReleaseCity() //This is now misnamed but annoying to fix. It is now releasing anything.
    {
        animator.SetBool("Holding", false);
    }

    public void RightHandSpawn(int SpawnID)
    {
        if (RightHand.HoldingObject) return;
        RightHand.SpawnInHand(SpawnID);
    }

    public void RightHandDelete()
    {
        RightHand.DestroyHeldObject();
    }

    public void RightHandRelease(int ReleaseID)
    {
        RightHand.ReleaseHandObject(ReleaseID);
    }

    public void RightHandPickup(string objectName)
    {
        RightHand.PickupHandObject(objectName);
    }

    public void RightHandActivate()
    {
        RightHand.Activate();
    }

    public void LeftHandSpawn(int SpawnID)
    {
        if (LeftHand.HoldingObject) return;
        LeftHand.SpawnInHand(SpawnID);
    }

    public void LeftHandDelete()
    {
        LeftHand.DestroyHeldObject();
    }

    public void LeftHandRelease(int ReleaseID)
    {
        LeftHand.ReleaseHandObject(ReleaseID);
    }

    public void LeftHandPickup(string objectName)
    {
        LeftHand.PickupHandObject(objectName);
    }

    public void LeftHandActivate()
    {
        LeftHand.Activate();
    }

    public void LeftHandRumble()
    {
        LeftHand.TurnOnMovementRumble();
    }
    public void LeftHandStopRumble()
    {
        LeftHand.TurnOffMovementRumble();
    }

    public void LeftHandDropAndActivate(int ReleaseID)
    {
        LeftHand.ReleaseHandObject(ReleaseID);
        LeftHand.Activate();
    }

    public void ExtraSpawn0(int SpawnID)
    {
        if (ExtraPropHolders[0].HoldingObject) return;
        ExtraPropHolders[0].SpawnInHand(SpawnID);
    }

    public void ExtraPickup0(string objectName)
    {
        ExtraPropHolders[0].PickupHandObject(objectName);
    }

    public void BroadcastActivation(string TriggerName)
    {
        ActiveBroadcast.BroadcastActivation(TriggerName);
    }

    public void SetRumbleToHeld()
    {
        MoveCamera.moveCamera.SetToHeld(true);
    }

    public void SetRumbleToRelease()
    {
        MoveCamera.moveCamera.SetToHeld(false);
    }
}
