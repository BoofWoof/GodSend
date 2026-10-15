using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaySlotGenerator : MonoBehaviour
{
    public GameObject SeparatorPrefab;
    public GameObject SlotPrefab;

    public int DefaultSlotNumbers;
    private int SlotCount;

    public HorizontalLayoutGroup TargetLayoutGroup;

    private List<GameObject> Separators = new();
    private List<GameObject> Slots = new();

    private float AreaWidth = 4280;
    private float RemainingWidth;

    public GraphManagerScript GraphManager;

    public void Start()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        Separators.Clear();
        Slots.Clear();

        GenerateDefaultRow();
    }

    [ContextMenu("GeneratePlayRow")]
    public void GenerateDefaultRow()
    {
        GeneratePlayRow(DefaultSlotNumbers);
    }
    public void GeneratePlayRow(int slotNumber)
    {
        SlotCount = slotNumber;

        ClearRow();

        AddSeparator();

        for (int i = 0; i < slotNumber; i++)
        {
            AddSlot();
            AddSeparator();
        }

        TargetLayoutGroup.spacing = RemainingWidth / (TotalItems() - 1f);

        GraphManager.UpdateGraph(0);
    }

    private int TotalItems()
    {
        return Separators.Count + Slots.Count;
    }
    public void ClearRow()
    {
        AreaWidth = GetComponent<RectTransform>().rect.width - 100f;
        RemainingWidth = AreaWidth;

        for (int i = Separators.Count - 1; i >= 0; i--) {
            RemoveSeparator(Separators[i]);
        }
        for (int i = Slots.Count - 1; i >= 0; i--)
        {
            RemoveSlot(Slots[i]);
        }

        Separators.Clear();
        Slots.Clear();
    }

    public GameObject AddSeparator()
    {
        GameObject newSeparator = Instantiate(SeparatorPrefab);
        newSeparator.name = $"Separator_{Separators.Count}";
        Separators.Add(newSeparator);

        const float SeparatorScale = 2.24f;

        Transform nsTrans = newSeparator.transform;
        nsTrans.SetParent(TargetLayoutGroup.transform);
        nsTrans.localPosition = Vector3.zero;
        nsTrans.localRotation = Quaternion.identity;
        nsTrans.localScale = Vector3.one * SeparatorScale;

        RemainingWidth -= ((RectTransform)nsTrans).rect.width * SeparatorScale;

        TMP_Text sepValue = newSeparator.GetComponentInChildren<TMP_Text>();
        if(Separators.Count == 1)
        {
            sepValue.alignment = TextAlignmentOptions.Left;
            sepValue.GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
            sepValue.transform.localPosition = new Vector2(0f, sepValue.transform.localPosition.y);
        }
        if (Separators.Count == SlotCount + 1)
        {
            sepValue.alignment = TextAlignmentOptions.Right;
            sepValue.GetComponent<RectTransform>().pivot = new Vector2(1f, 0.5f);
            sepValue.transform.localPosition = new Vector2(0f, sepValue.transform.localPosition.y);
        }

        return newSeparator;
    }
    public void RemoveSeparator(GameObject separatorToRemove)
    {
        Slots.Remove(separatorToRemove);

        if (Application.isPlaying)
        {
            Destroy(separatorToRemove);
        }
        else
        {
            DestroyImmediate(separatorToRemove);
        }
    }
    public GameObject AddSlot()
    {
        GameObject newSlot = Instantiate(SlotPrefab);
        Slots.Add(newSlot);

        Transform nsTrans = newSlot.transform;
        nsTrans.SetParent(TargetLayoutGroup.transform);
        nsTrans.localPosition = Vector3.zero;
        nsTrans.localRotation = Quaternion.identity;
        nsTrans.localScale = Vector3.one;

        RemainingWidth -= ((RectTransform)nsTrans).rect.width;

        GraphManager.AddCardSlot(newSlot.GetComponent<CardSlot>());

        newSlot.GetComponent<CardSlot>().PlaySlot = true;

        return newSlot;
    }

    public void RemoveSlot(GameObject slotToRemove)
    {
        Slots.Remove(slotToRemove);

        if (Application.isPlaying)
        {
            Destroy(slotToRemove);
        }
        else
        {
            DestroyImmediate(slotToRemove);             
        }
    }

    public int GetSlotCount()
    {
        return Slots.Count;
    }

    public CardSlot GetSlot(int index)
    {
        return Slots[index].GetComponent<CardSlot>();
    }
    public GameObject GetSeparator(int index)
    {
        return Separators[index];
    }

    public void AddEnemyCard(GameObject card, int targetPosIdx)
    {
        card.GetComponent<GameCardScript>().PlayerCard = false;
        GetSlot(targetPosIdx).AddCardToStack(card, true);
    }

    public void AddQueuedEnemyCard(GameObject card, int targetPosIdx)
    {
        card.GetComponent<GameCardScript>().PlayerCard = false;
        GetSlot(targetPosIdx).AddCardToStack(card, false);
    }
}
