using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimatedFillScript : MonoBehaviour
{
    public Image FillTarget;
    public Gradient FillGradient;

    public float FillRate;
    public Vector2 FillRange;

    private float CurrentFill = 0f;
    private float TargetFill = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateFillAmount();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentFill = Mathf.MoveTowards(CurrentFill, TargetFill, FillRate*Time.deltaTime);
        UpdateFillAmount();
    }

    private void UpdateFillAmount()
    {
        float fillPercentage = (CurrentFill - FillRange.x) / (FillRange.y - FillRange.x);
        FillTarget.fillAmount = fillPercentage;
        FillTarget.color = FillGradient.Evaluate(fillPercentage);
    }

    public void SetNewFillTarget(float targetFill)
    {
        TargetFill = targetFill;
    }
}
