using UnityEngine;

[CreateAssetMenu(fileName = "AchievementAbstractSO", menuName = "Scriptable Objects/AchievementAbstractSO")]
public abstract class AchievementAbstractSO : ScriptableObject
{
    public string Title;
    public string Objective;
    public string Flavor;
    public string ButtonText;

    public bool AnnounceUnlock = true;
    public bool FirstCompletionCheck = true;

    public string OnBuyAnnouncement;

    public BroadcastStruct ActivationData;

    public abstract bool CheckCompletionCriteria();

    public virtual string ProgressText()
    {
        return "";
    }

}
