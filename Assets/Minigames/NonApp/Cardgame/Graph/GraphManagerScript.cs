using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GraphManagerScript : MonoBehaviour
{
    public int StartingValue = 10;
    private List<int> GraphValues = new();


    public PlaySlotGenerator SlotData;

    public RectTransform GraphArea;

    private List<GameObject> GraphLines = new();

    public Color PositiveColor = Color.green;
    public Color NegativeColor = Color.red;
    public Color NeutralColor = Color.blue;
    public Color GoalColor = Color.yellow;

    public int GoalValue = 15;
    public bool GreaterThanGoal = true;

    public TMP_Text WinText;
    private int TotalWins = 0;
    private int Wins = 0;
    public TMP_Text LossText;
    private int TotalLosses = 0;
    private int Losses = 0;

    public TMP_Text TimeText;
    private int CardGameTime = 0;
    private int TotalCardGameTime = 0;
    private static bool GraphTimeIsProgressing = false;

    public RectTransform TimeRect;

    private int RoundCount = 0;
    public static Action<int> OnTimeEnd;
    public static Action<int> OnRoundEnd;

    public void Start()
    {
        for (int i = GraphArea.childCount - 1; i >= 0; i--)
        {
            Destroy(GraphArea.GetChild(i).gameObject);
        }
        GraphLines.Clear();

        ResetTimer();
    }

    public void ResetTimer()
    {
        TimeRect.sizeDelta = new Vector2(0, TimeRect.sizeDelta.y);
    }

    public bool CheckForWin()
    {
        if (GreaterThanGoal)
        {
            if (GraphValues[GraphValues.Count - 1] >= GoalValue)
            {
                return true;
            }
        } else
        {
            if (GraphValues[GraphValues.Count - 1] <= GoalValue)
            {
                return true;
            }
        }
        return false;
    }

    public void IncreaseWins()
    {
        TotalWins++;
        Wins++;
        WinText.text = Wins.ToString();
    }

    public void IncreaseLosses()
    {
        TotalLosses++;
        Losses++;
        LossText.text = Losses.ToString();
    }

    public void GenerateNewGoal()
    {
        //Placeholder
        GoalValue = GoalValue + 7;
        GreaterThanGoal = true;
    }

    public void OnCardPlay(GameCardScript playedCard)
    {
        if (!playedCard.PlayerCard) return;

        Debug.Log("Test Card Played");

        TotalCardGameTime += playedCard.CardData.TimeCost;
        UpdateTimeText();

        StartCoroutine(PassTime(playedCard.CardData.TimeCost));
    }

    private IEnumerator PassTime (int time)
    {
        GraphTimeIsProgressing = true;

        for (int i = 0; i < time; i++)
        {
            CardGameTime++;


            float timePassed = 0f;
            float transitionPeriod = 0.4f;
            float xSpacing = CalculateXSpacing();
            float startXPos = (CardGameTime - 1) * xSpacing;
            float endingXPos = (CardGameTime) * xSpacing;
            bool graphUpdated = false;
            while (timePassed < transitionPeriod)
            {
                timePassed += Time.deltaTime;
                float progress = timePassed / transitionPeriod;
                TimeRect.sizeDelta = new Vector2(Mathf.Lerp(startXPos, endingXPos, progress), TimeRect.sizeDelta.y);

                if(timePassed > transitionPeriod / 2f && !graphUpdated)
                {
                    graphUpdated = true;

                    CardSlot slot = SlotData.GetSlot(CardGameTime - 1);
                    slot.SetExhausted(true);
                    slot.LaunchParticles();
                    UpdateGraph(CardGameTime);
                }
                yield return null;
            }

            if (CardGameTime >= GraphValues.Count - 1)
            {
                RoundCount++;
                OnRoundEnd?.Invoke(RoundCount);

                CardGameTime = 0;
                StartingValue = GraphValues[GraphValues.Count - 1];
                for (int j = 0; j < SlotData.GetSlotCount(); j++)
                {
                    SlotData.GetSlot(j).SetExhausted(false);
                }
                if (CheckForWin()) IncreaseWins();
                else IncreaseLosses();

                GenerateNewGoal();

                UpdateGraph(CardGameTime);

                ResetTimer();

                yield return new WaitForSeconds(0.5f);
            }
        }

        OnTimeEnd?.Invoke(CardGameTime);

        UpdateGraph(CardGameTime);

        CardStockManager.instance.CleanupParticleLayer();

        GraphTimeIsProgressing = false;
    }


    public void UpdateTimeText()
    {
        TimeText.text = $"Time: {TotalCardGameTime}";
    }

    public void AddCardSlot(CardSlot slot)
    {
        Debug.Log("CardPlayed");
        slot.OnCardPlay += OnCardPlay;
    }

    public void UpdateGraph(int updateStartIndex)
    {
        List<int> newGraphValues = new();

        int currentValue = StartingValue;
        newGraphValues.Add(currentValue);

        GameObject separator = SlotData.GetSeparator(0);
        separator.GetComponentInChildren<TMP_Text>().text = currentValue.ToString();

        for (int i = 1; i < updateStartIndex + 1; i++)
        {
            currentValue = GraphValues[i];
            newGraphValues.Add(currentValue);

            separator = SlotData.GetSeparator(i);
            separator.GetComponentInChildren<TMP_Text>().text = currentValue.ToString();
        }

        for (int i = updateStartIndex; i < SlotData.GetSlotCount(); i++)
        {
            GameCardScript activeCard = SlotData.GetSlot(i).GetActiveCard();
            if (activeCard != null)
            {
                OngoingCardSO ongoingStrategy = activeCard.CardData as OngoingCardSO;
                currentValue += ongoingStrategy.CalculateGraphChange();
            }

            newGraphValues.Add(currentValue);

            separator = SlotData.GetSeparator(i + 1);
            separator.GetComponentInChildren<TMP_Text>().text = currentValue.ToString();
        }


        GraphValues = newGraphValues;

        CreateLines();
    }
    private void CreateLines()
    {
        ClearGraphLines();

        int maxValue = GraphValues.Max();
        int minValue = GraphValues.Min();

        if(maxValue < GoalValue) maxValue = GoalValue;
        if(minValue > GoalValue) minValue = GoalValue;

        int range = maxValue - minValue;
        if (range < 5) range = 5;
        int maxY = (int)Math.Ceiling(maxValue + 0.05f * range);
        int minY = (int)Math.Floor(minValue - 0.05f * range);
        int finalRange = maxY - minY;

        float xSpacing = CalculateXSpacing();
        float minXPos = -GraphArea.sizeDelta.x / 2f;

        float ySpacing = GraphArea.sizeDelta.y / finalRange;
        float minYPos = -GraphArea.sizeDelta.y / 2f;

        for (int i = 1; i < GraphValues.Count; i++)
        {
            int valueA = GraphValues[i - 1];
            int valueB = GraphValues[i];

            Vector2 pointA = new Vector2(minXPos + xSpacing * (i-1), minYPos + (valueA - minY) * ySpacing);
            Vector2 pointB = new Vector2(minXPos + xSpacing * (i), minYPos + (valueB - minY) * ySpacing);

            Color lineColor = Color.black;
            if (pointA.y < pointB.y)
            {
                lineColor = PositiveColor;
            }
            else if (pointA.y > pointB.y)
            {
                lineColor = NegativeColor;
            }
            else
            {
                lineColor = NeutralColor;
            }

            float alpha = i > CardGameTime ? 0.1f : 1f; 

            DrawLine(pointA, pointB, lineColor, alpha);
        }

        Vector2 goalA = new Vector2(minXPos, minYPos + (GoalValue - minY) * ySpacing);
        Vector2 goalB = new Vector2(-minXPos, minYPos + (GoalValue - minY) * ySpacing);
        DrawLine(goalA, goalB, GoalColor);
    }
    public void DrawLine(Vector2 pointA, Vector2 pointB, Color lineColor, float alpha = 1)
    {
        GameObject lineGO = new GameObject("Line", typeof(Image));
        lineGO.transform.SetParent(GraphArea);

        Image image = lineGO.GetComponent<Image>();
        image.color = new Color(lineColor.r, lineColor.g, lineColor.b, alpha);

        RectTransform rectTransform = lineGO.GetComponent<RectTransform>();

        Vector2 dir = (pointB - pointA).normalized;
        float distance = Vector2.Distance(pointA, pointB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 15f);
        rectTransform.localPosition = pointA + dir * distance * 0.5f;

        rectTransform.localScale = Vector3.one;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rectTransform.localEulerAngles = new Vector3(0, 0, angle);

        GraphLines.Add(lineGO);

    }
    public void ClearGraphLines()
    {
        for(int i = GraphLines.Count - 1; i >= 0; i--) {
            Destroy(GraphLines[i]);
        }
        GraphLines.Clear();
    }

    public static bool IsGraphAvailable()
    {
        return GraphTimeIsProgressing;
    }

    private float CalculateXSpacing()
    {

        return GraphArea.sizeDelta.x / (GraphValues.Count - 1f);
    }
}
