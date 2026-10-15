using UnityEngine;

[CreateAssetMenu(fileName = "CardSO", menuName = "Cards/OngoingStrategy")]
public class OngoingCardSO : CardSO
{

    public int GraphChangeStrength;
    override public void OnActivate()
    {

    }

    public int CalculateGraphChange()
    {
        return GraphChangeStrength;
    }
}
