using TMPro;
using UnityEngine;

public class SozaWMScript : MonoBehaviour
{
    public TMP_Text Title;
    public TMP_Text Description;

    private string DefaultTitle;
    private string DefaultDescription;

    public UIAnimatedFillScript MinistryPopularity;
    public UIAnimatedFillScript OperationalEfficiency;
    public UIAnimatedFillScript PopulationRemaining;

    public void Start()
    {
        DefaultTitle = Title.text;
        DefaultDescription = Description.text;
    }

    public void SetToDefault()
    {
        Title.text = DefaultTitle;
        Description.text = DefaultDescription;
        MinistryPopularity.SetNewFillTarget(0f);
        OperationalEfficiency.SetNewFillTarget(0f);
        PopulationRemaining.SetNewFillTarget(0f);
    }

    public void SetWMData(SozaWMDataSO newData)
    {
        Title.text = newData.Title;
        Description.text = newData.Description;
        MinistryPopularity.SetNewFillTarget(newData.MinistryPopularity);
        OperationalEfficiency.SetNewFillTarget(newData.OperationalEfficiency);
        PopulationRemaining.SetNewFillTarget(newData.PopulationRemaining);
    }
}
