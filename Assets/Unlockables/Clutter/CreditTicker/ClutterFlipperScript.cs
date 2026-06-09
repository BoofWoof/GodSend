using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ClutterFlipperScript : MonoBehaviour
{
    private int DisplayNumber = 0;
    private int _TargetNumber = 0;
    public int TargetNumber
    {
        get
        {
            return _TargetNumber;
        }
        private set
        {
            _TargetNumber = value % 10;
        }
    }
    private bool AnimatingFlicker = false;

    public float TickoverPeriod = 0.1f;

    public Transform FlipBone;

    public TMP_Text FrontTop;
    public TMP_Text FrontBottom;
    public TMP_Text BackTop;
    public TMP_Text BackBottom;

    public UnityEvent OnFlip;

    public void Awake()
    {
        QuickSetValue(0);
    }
    public void OnDisable()
    {
        AnimatingFlicker = false;
    }

    public void QuickSetValue(int setNumber)
    {
        TargetNumber = setNumber;
        DisplayNumber = 0;

        FrontTop.text = TargetNumber.ToString();
        FrontBottom.text = TargetNumber.ToString();
        BackTop.text = TargetNumber.ToString();
        BackBottom.text = TargetNumber.ToString();
    }

    public void SetValue(int setNumber)
    {
        TargetNumber = setNumber;

        if (TargetNumber == DisplayNumber) return;
        if (AnimatingFlicker) return;

        StartCoroutine(NumberSwap());
    }

    public IEnumerator NumberSwap()
    {
        AnimatingFlicker = true;

        Debug.Log($"{name}: {DisplayNumber}/{TargetNumber}");

        while(TargetNumber != DisplayNumber)
        {
            OnFlip?.Invoke();

            DisplayNumber = (DisplayNumber + 1) % 10;
            BackTop.text = DisplayNumber.ToString();
            BackBottom.text = DisplayNumber.ToString();
            float timePassed = 0f;
            while (timePassed <= TickoverPeriod)
            {
                timePassed += Time.deltaTime;
                float progress = timePassed / TickoverPeriod;
                FlipBone.localRotation = Quaternion.Euler(90f, Mathf.Lerp(0, -180, progress), 0f);
                yield return null;
            }
            FlipBone.localRotation = Quaternion.Euler(90f, 0, 0f);
            FrontTop.text = DisplayNumber.ToString();
            FrontBottom.text = DisplayNumber.ToString();
        }

        AnimatingFlicker = false;
    }

    public IEnumerator CountTest()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            SetValue(TargetNumber + 1);
        }
    }
}
