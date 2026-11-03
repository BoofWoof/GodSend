using UnityEngine;
using UnityEngine.Events;

public class DoorOpenScript : MonoBehaviour
{
    public UnityEvent OnDoorOpen;
    public UnityEvent OnDoorClose;

    public Animator DoorAnimator;

    public void OpenDoor()
    {
        DoorAnimator.SetBool("DoorOpen", true);
        OnDoorOpen?.Invoke();
    }

    public void CloseDoor()
    {
        DoorAnimator.SetBool("DoorOpen", false);
        OnDoorClose?.Invoke();
    }
}
