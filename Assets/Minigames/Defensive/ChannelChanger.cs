using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChannelChanger : MonoBehaviour
{
    public Animator ScreenAnimator;
    public bool ScreenRaised = true;

    public Animator MaskAnimator;

    public Image MinigameMask;
    public Coroutine RevealCoroutine;

    public GameObject AerialDefense;
    public GameObject PurityDefense;

    public AudioSource ExtensionAudio;

    public static ChannelChanger ActiveChannelChanger;

    public static ChannelChanger instance;

    public void Awake()
    {
        instance = this;

        GameStateMonitor.DangerActive = false;
    }

    public void Start()
    {
        ActiveChannelChanger = this;
        LockSwitch(false);
    }

    public void LockSwitch(bool PhoneUnlock = true)
    {
        if (GameStateMonitor.DangerActive) return;
        if (PhoneUnlock) PhonePositionScript.UnlockPhone();

        AerialDefense.SetActive(false);
        PurityDefense.SetActive(false);

        ScreenPositionChange(true);
    }

    public void AerialSwitch()
    {
        PhonePositionScript.LockPhoneDown();

        AerialDefense.SetActive(true);
        PurityDefense.SetActive(false);

        ScreenPositionChange(false);
    }

    public void PuritySwitch()
    {
        PhonePositionScript.LockPhoneDown();

        AerialDefense.SetActive(false);
        PurityDefense.SetActive(true);

        ScreenPositionChange(false);
    }

    public void ScreenPositionChange(bool raise = true)
    {
        if ((ScreenRaised && raise) || (!ScreenRaised && !raise)) return;
        ScreenRaised = raise;

        if (raise)
        {
            ScreenAnimator.Play("ContractUp");
            ExtensionAudio.Play();

            MaskAnimator.Play("MaskCollapse");
            if (RevealCoroutine != null) StopCoroutine(RevealCoroutine);
            return;
        }
        ScreenAnimator.Play("ExpandDown");
        ExtensionAudio.Play();

        RevealCoroutine = StartCoroutine(TurnOnScreen());
    }

    public IEnumerator TurnOnScreen()
    {
        yield return new WaitForSeconds(1.37f);

        MaskAnimator.Play("MaskExpand");
    }
}
