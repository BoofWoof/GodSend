using UnityEngine;

public class CardMakerScript : MonoBehaviour
{
    public static CardMakerScript instance;
    public GameObject CardPrefab;

    public void Awake()
    {
        instance = this;
    }

    public GameObject MakeCard(CardSO cardData)
    {
        GameObject newCard = Instantiate(CardPrefab);
        GameCardScript cardScript = newCard.GetComponent<GameCardScript>();
        cardScript.SetCardData(cardData);
        return newCard;
    }
}
