using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimePassingScript : MonoBehaviour
{
    public GameObject ObjectBase;

    public RectTransform TopBorder;
    public RectTransform BottomBorder;
    public Image BackPanel;

    public Transform ClockTransform;
    public Transform TextTransform;

    public Transform MinuteHand;
    public Transform HourHand;

    public TMP_Text TimeText;
    public TMP_Text TitleText;

    public float fadeInPeriod = 1f;
    public int holdTicks = 3;

    public AudioSource BoomSound;
    public AudioSource TickSound;

    public void Awake()
    {
        ObjectBase.SetActive(false);
    }
    public void StartDisplay(TimePassTrigger data)
    {
        StartCoroutine(TimePassingCutscene(data));
    }

    public IEnumerator TimePassingCutscene(TimePassTrigger data)
    {
        float width = TopBorder.sizeDelta.x;
        float finalHeight = TopBorder.sizeDelta.y;
        TopBorder.sizeDelta = new Vector2 (width, 0);
        BottomBorder.sizeDelta = new Vector2(width, 0);

        float finalAlpha = BackPanel.color.a;
        BackPanel.color = new Color(0, 0, 0, 0);

        Vector3 finalClockPos = ClockTransform.transform.localPosition;
        Vector3 startClockPos = new Vector3(finalClockPos.x, 800f, finalClockPos.z);
        ClockTransform.transform.localPosition = startClockPos;

        Vector3 finalTextPos = TextTransform.transform.localPosition;
        Vector3 startTextPos = new Vector3(finalTextPos.x, -600f, finalTextPos.z);
        TextTransform.transform.localPosition = startTextPos;

        TimeText.text = data.TextTime;
        TitleText.text = data.Title;

        HourHand.localRotation = Quaternion.Euler(0, 0, -360f * data.Hour/12f);
        MinuteHand.localRotation = Quaternion.Euler(0, 0, 0);

        ObjectBase.SetActive(true);

        BoomSound.Play();

        float timePassed = 0f;
        while(timePassed < fadeInPeriod)
        {
            timePassed += Time.deltaTime;
            float progress = timePassed/fadeInPeriod;

            TopBorder.sizeDelta = new Vector2(width, Mathf.Lerp(0, finalHeight, progress));
            BottomBorder.sizeDelta = new Vector2(width, Mathf.Lerp(0, finalHeight, progress));

            ClockTransform.transform.localPosition = Vector3.Lerp(startClockPos, finalClockPos, progress);
            TextTransform.transform.localPosition = Vector3.Lerp(startTextPos, finalTextPos, progress);

            BackPanel.color = new Color(0, 0, 0, Mathf.Lerp(0, finalAlpha, progress));

            yield return null;
        }

        TopBorder.sizeDelta = new Vector2(width, finalHeight);
        BottomBorder.sizeDelta = new Vector2(width, finalHeight);

        ClockTransform.transform.localPosition = finalClockPos;
        TextTransform.transform.localPosition = finalTextPos;

        BackPanel.color = new Color(0, 0, 0, finalAlpha);

        for (int i = 1; i < holdTicks+1; i++)
        {
            MinuteHand.localRotation = Quaternion.Euler(0, 0, -360f * i / 60f);
            TickSound.Play();
            yield return new WaitForSeconds(1f);
        }
        MinuteHand.localRotation = Quaternion.Euler(0, 0, -360f * (holdTicks+1) / 60f);
        TickSound.Play();

        timePassed = 0f;
        while (timePassed < fadeInPeriod)
        {
            timePassed += Time.deltaTime;
            float progress = timePassed / fadeInPeriod;

            TopBorder.sizeDelta = new Vector2(width, Mathf.Lerp(finalHeight, 0, progress));
            BottomBorder.sizeDelta = new Vector2(width, Mathf.Lerp(finalHeight, 0, progress));

            ClockTransform.transform.localPosition = Vector3.Lerp(finalClockPos, startClockPos, progress);
            TextTransform.transform.localPosition = Vector3.Lerp(finalTextPos, startTextPos, progress);

            BackPanel.color = new Color(0, 0, 0, Mathf.Lerp(finalAlpha, 0, progress));

            yield return null;
        }

        ObjectBase.SetActive(false);
    }

}
