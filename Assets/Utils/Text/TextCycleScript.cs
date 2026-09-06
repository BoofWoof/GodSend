using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextCycleScript : MonoBehaviour
{
    public TMP_Text TargetText;
    public TMP_Text PageCount;
    [TextArea] public string[] TextArray;
    private int TargetIdx;
    public int StartingIdx;

    [ContextMenu("SetToDefault")]
    public void Start()
    {
        SetTextTo(StartingIdx);
    }

    private void SetTextTo(int idx)
    {
        TargetIdx = idx;
        TargetText.text = TextArray[TargetIdx];

        if (PageCount != null) PageCount.text = $"{TargetIdx + 1}/{TextArray.Length}";

        LayoutRebuilder.ForceRebuildLayoutImmediate(TargetText.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)TargetText.rectTransform.parent);
    }

    public void ChangeTextIdx(int idxChange)
    {
        int nexIDx = TargetIdx + idxChange;
        nexIDx = (nexIDx % TextArray.Length + TextArray.Length) % TextArray.Length;
        SetTextTo(nexIDx);
    }
}
