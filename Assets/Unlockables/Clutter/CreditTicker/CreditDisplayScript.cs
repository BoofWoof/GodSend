using System.Collections;
using UnityEngine;

public class CreditDisplayScript : MonoBehaviour
{
    public ClutterFlipperScript[] DisplayDigits = new ClutterFlipperScript[10];
    public Transform DecimalSlider;
    private Vector3 StartingDecimalSliderPosition;
    private float TargetDecimalZ;

    private bool DecimalMoving = false;

    public void OnEnable()
    {
        StartingDecimalSliderPosition = DecimalSlider.localPosition;
        CurrencyData.CreditUpdate += OnCreditUpdate;
    }
    public void OnDisable()
    {
        CurrencyData.CreditUpdate -= OnCreditUpdate;
        DecimalMoving = false;
    }
    public void OnCreditUpdate(float creditCount)
    {
        int magnitude = (int)Mathf.Floor(Mathf.Log10(creditCount));
        int decimals = 9 - Mathf.Clamp(magnitude, 0, 8);
        string format = $"F{decimals}";
        string creditString = creditCount.ToString(format);

        Debug.Log($"Setting new credit string: {creditString}");

        int idx = 0;
        foreach (char c in creditString)
        {
            if(c == '.')
            {
                TargetDecimalZ = (DisplayDigits[idx - 1].transform.localPosition.z + DisplayDigits[idx].transform.localPosition.z) / 2f;
                if(!DecimalMoving) StartCoroutine(MoveDecimal());
                continue;
            }
            if (!char.IsDigit(c)) continue;
            int value = int.Parse(c.ToString());
            Debug.Log(value);
            DisplayDigits[idx].SetValue(value);
            idx++;
        }
    }
    public IEnumerator MoveDecimal()
    {
        DecimalMoving = true;

        while(DecimalSlider.localPosition.z != TargetDecimalZ)
        {
            Vector3 TargetPosition = new Vector3(StartingDecimalSliderPosition.x, StartingDecimalSliderPosition.y, TargetDecimalZ);
            DecimalSlider.localPosition = Vector3.MoveTowards(DecimalSlider.localPosition, TargetPosition, 0.5f*Time.deltaTime);
            yield return null;

        }

        DecimalMoving = false;
    }
}