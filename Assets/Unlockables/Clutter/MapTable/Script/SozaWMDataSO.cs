using UnityEngine;

[CreateAssetMenu(fileName = "SozaWMDataSO", menuName = "CustomUI/SozaWMDataSO")]
public class SozaWMDataSO : ScriptableObject
{
    public string Title;
    [TextArea] public string Description;
    public float MinistryPopularity;
    public float OperationalEfficiency;
    public float PopulationRemaining;
}
