using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GraphManagerScript : MonoBehaviour
{
    public PlaySlotGenerator SlotData;

    public RectTransform GraphArea;

    public int StartingValue = 10;
    private List<int> GraphValues = new();
    private List<GameObject> GraphLines = new();

    private static int AllGraphMin = 9999;
    private static int AllGraphMax = -9999;

    public Color PositiveColor = Color.green;
    public Color NegativeColor = Color.red;
    public Color NeutralColor = Color.blue;
    public Color GoalColor = Color.yellow;

    public bool UsePredictionValuesInstead;
    private int TheoryTime;
    private bool TheoryCardActive;

    private static bool PreparedToUpdate;

    private static List<GraphManagerScript> GraphManagerList = new();

    public void Start()
    {
        for (int i = GraphArea.childCount - 1; i >= 0; i--)
        {
            Destroy(GraphArea.GetChild(i).gameObject);
        }
        GraphLines.Clear();

        GraphManagerList.Add(this);
    }
    public void OnDestroy()
    {
        GraphManagerList.Remove(this);
    }
    public void OnEnable()
    {
        CardSlot.OnCardSettledInSlot += OnCardSettled;
        if (UsePredictionValuesInstead) CardSlot.OnCardPlayedInSlot += OnCardPlayed;
        PlaySlotGenerator.OnFullSlotGeneration += OnNewSlotsGenerated;
        CardTimeManager.OnTimeTick += OnTimeTick;
        if (UsePredictionValuesInstead) GameCardScript.CardHoveredPlaySlot += OnCardHover;
    }

    public void OnDisable()
    {
        CardSlot.OnCardSettledInSlot -= OnCardSettled;
        if (UsePredictionValuesInstead) CardSlot.OnCardPlayedInSlot -= OnCardPlayed;
        PlaySlotGenerator.OnFullSlotGeneration -= OnNewSlotsGenerated;
        CardTimeManager.OnTimeTick -= OnTimeTick;
        if (UsePredictionValuesInstead) GameCardScript.CardHoveredPlaySlot -= OnCardHover;
    }

    public void OnCardHover(bool hovered, GameCardScript card,  CardSlot hoveredSlot)
    {
        if (!hovered)
        {
            ResetTheory();
            return;
        }

        TheoryCardActive = hovered;
        TheoryTime = card.CardData.TimeCost;
        MarkUpdateNeeded();
    }

    public void OnCardPlayed(CardSlot slot, GameCardScript card)
    {
        ResetTheory();
    }

    public void ResetTheory()
    {
        TheoryCardActive = false;
        TheoryTime = 0;
        ClearGraphLines();
    }

    public void OnTimeTick(int currentTime)
    {
        MarkUpdateNeeded();
    }

    public void OnRoundEnd(int roundNumber)
    {
        StartingValue = GraphValues[GraphValues.Count - 1];
        for (int j = 0; j < SlotData.GetSlotCount(); j++)
        {
            SlotData.GetSlot(j).SetExhausted(false);
        }

        MarkUpdateNeeded();
    }

    public void OnCardSettled(CardSlot playedSlot, GameCardScript playedCard)
    {
        MarkUpdateNeeded();
    }

    public void MarkUpdateNeeded()
    {
        StartCoroutine(PrepareGraphUpdate());
    }

    public IEnumerator PrepareGraphUpdate()
    {
        if (PreparedToUpdate) yield break;

        PreparedToUpdate = true;

        yield return null;

        int currentGameTime = CardStockManager.instance.TimeManager.GetCardGameTime();
        foreach (GraphManagerScript graphManager in GraphManagerList)
        {
            graphManager.UpdateGraph(currentGameTime);
        }
        CalculateMinMax();
        foreach (GraphManagerScript graphManager in GraphManagerList)
        {
            graphManager.CreateLines(currentGameTime);
        }

        PreparedToUpdate = false;
    }

    public void OnNewSlotsGenerated(PlaySlotGenerator slotGenerator)
    {
        MarkUpdateNeeded();
    }

    public void UpdateGraph(int updateStartIndex)
    {
        if (TheoryCardActive)
        {
            updateStartIndex += TheoryTime;
        }

        if (updateStartIndex > SlotData.GetSlotCount()) return;

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
            GameCardScript activeCard;
            if (TheoryCardActive)
            {
                activeCard = SlotData.GetSlot(i).GetPredictionCard();
            } else
            {
                activeCard = SlotData.GetSlot(i).GetActiveCard();
            }
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
    }

    private static void CalculateMinMax()
    {
        AllGraphMax = -999999;
        AllGraphMin = 999999;

        foreach(GraphManagerScript graphManager in GraphManagerList)
        {
            if (graphManager.GraphValues.Count == 0) continue;

            int localMax = graphManager.GraphValues.Max();
            int localMin = graphManager.GraphValues.Min();

            if (localMax > AllGraphMax) AllGraphMax = localMax;
            if(localMin < AllGraphMin) AllGraphMin = localMin;
        }

        int goalValue = CardStockManager.instance.GoalValue;
        if (AllGraphMax < goalValue) AllGraphMax = goalValue;
        if (AllGraphMin > goalValue) AllGraphMin = goalValue;
    }

    private void CreateLines(int currentTimeIndex)
    {
        ClearGraphLines();

        if (GraphValues.Count == 0) return;

        int goalValue = CardStockManager.instance.GoalValue;

        int range = AllGraphMax - AllGraphMin;
        if (range < 5) range = 5;
        int maxY = (int)Math.Ceiling(AllGraphMax + 0.05f * range);
        int minY = (int)Math.Floor(AllGraphMin - 0.05f * range);
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

            float alpha = i > currentTimeIndex ? 0.1f : 1f; 

            DrawLine(pointA, pointB, lineColor, alpha);
        }

        if (UsePredictionValuesInstead) return;
        Vector2 goalA = new Vector2(minXPos, minYPos + (goalValue - minY) * ySpacing);
        Vector2 goalB = new Vector2(-minXPos, minYPos + (goalValue - minY) * ySpacing);
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
            if (Application.isPlaying)
            {
                Destroy(GraphLines[i]);
            } else
            {
                DestroyImmediate(GraphLines[i]);
            }
        }
        GraphLines.Clear();
    }

    private float CalculateXSpacing()
    {

        return GraphArea.sizeDelta.x / (GraphValues.Count - 1f);
    }

    public int GetFinalValue()
    {
        return GraphValues[GraphValues.Count - 1];
    }
}
