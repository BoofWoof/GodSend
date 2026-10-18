using System;
using System.Collections;
using UnityEngine;

public class CardTimeManager : MonoBehaviour
{
    private int CardGameTime = 0;
    private int TotalCardGameTime = 0;
    private static bool GraphTimeIsProgressing = false;

    private int RoundCount = 0;

    public static Action<int> OnTimeTick;
    public static Action<int> OnTimeEnd;
    public static Action<int> OnWinCheck;
    public static Action<int> OnRoundEnd;

    public RectTransform TimeRect;
    public RectTransform TimeRectUnderlay;

    public GameObject ExpectedPosition;

    public int GetCardGameTime()
    {
        return CardGameTime;
    }

    public void Start()
    {
        ResetTimer();
        
    }

    public void OnEnable()
    {
        GameCardScript.CardPickedUp += OnCardPickup;
        CardSlot.OnCardPlayedInSlot += OnCardPlay;
    }

    public void OnDisable()
    {
        GameCardScript.CardPickedUp -= OnCardPickup;
        CardSlot.OnCardPlayedInSlot -= OnCardPlay;
    }

    public void ResetTimer()
    {
        TimeRect.sizeDelta = new Vector2(0, TimeRect.sizeDelta.y);
    }
    public void OnCardPlay(CardSlot playedSlot, GameCardScript playedCard)
    {
        if (!playedCard.PlayerCard) return;

        TotalCardGameTime += playedCard.CardData.TimeCost;

        StartCoroutine(PassTime(playedCard.CardData.TimeCost));
    }

    public void StartPassTime(int time)
    {
        StartCoroutine(PassTime(time));
    }

    private IEnumerator PassTime(int time)
    {
        GraphTimeIsProgressing = true;

        for (int i = 0; i < time; i++)
        {
            CardGameTime++;

            float timePassed = 0f;
            float transitionPeriod = 0.4f;
            float xSpacing = TimeRectUnderlay.EqualSpacingX(7);
            float startXPos = (CardGameTime - 1) * xSpacing;
            float endingXPos = (CardGameTime) * xSpacing;
            bool graphUpdated = false;
            while (timePassed < transitionPeriod)
            {
                timePassed += Time.deltaTime;
                float progress = timePassed / transitionPeriod;
                TimeRect.sizeDelta = new Vector2(Mathf.Lerp(startXPos, endingXPos, progress), TimeRect.sizeDelta.y);

                if (timePassed > transitionPeriod / 2f && !graphUpdated)
                {
                    graphUpdated = true;

                    CardSlot slot = CardSlot.GetSlot(CardGameTime - 1);
                    slot.AcitvateCard();

                    OnTimeTick?.Invoke(CardGameTime);
                }
                yield return null;
            }

            int slotCount = CardSlot.GetSlotCount();
            if (CardGameTime >= slotCount)
            {
                RoundCount++;
                CardGameTime = 0;

                OnWinCheck?.Invoke(RoundCount);
                OnRoundEnd?.Invoke(RoundCount);

                ResetTimer();

                yield return new WaitForSeconds(0.5f);
            }
        }

        OnTimeEnd?.Invoke(CardGameTime);

        //ADD WHEN CARD SETTLES TO GRAPH UPDATE

        CardStockManager.instance.CleanupParticleLayer();

        GraphTimeIsProgressing = false;
    }

    public void OnCardPickup(bool cardPickedUp, GameCardScript gameCard)
    {
        if (ExpectedPosition == null) return;

        int cost = gameCard.CardData.TimeCost;
        float xSpacing = TimeRectUnderlay.EqualSpacingX(7);
        float previewTime = (CardGameTime + cost) % (CardStockManager.instance.SlotManager.GetSlotCount());
        ExpectedPosition.GetComponent<RectTransform>().anchoredPosition = Vector3.right * previewTime * xSpacing;
        ExpectedPosition.SetActive(cardPickedUp);
    }

    public static bool IsTimeManagerAvailable()
    {
        return GraphTimeIsProgressing;
    }
}
