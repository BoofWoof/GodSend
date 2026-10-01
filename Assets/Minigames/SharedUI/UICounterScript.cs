using TMPro;
using UnityEngine;

public class UICounterScript : MonoBehaviour
{
    public TMP_Text DisplayText;

    public bool DisplayAtZero = false;

    private int CurrentCount = 0;

    public void Start()
    {
        ResetValue();
    }

    public void UpdateText()
    {
        if(!DisplayAtZero) gameObject.SetActive(CurrentCount != 0);

        DisplayText.text = CurrentCount.ToString();
    }

    public void IncreaseValue()
    {
        CurrentCount++;
        UpdateText();
    }

    public void ResetValue()
    {
        CurrentCount = 0;
        UpdateText();
    }
}
