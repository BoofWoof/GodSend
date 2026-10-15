using UnityEngine;
using UnityEngine.Events;

public class CardStockLevelHolderScript : MonoBehaviour
{
    public StockCardLevelSO CardLevelSO;

    public CardStockManager TargetGameManager;

    public UnityEvent OnStart;

    public void StartLevel()
    {
        TargetGameManager.StartLevel(CardLevelSO);
    }
}
