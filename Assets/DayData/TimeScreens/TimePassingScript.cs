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

    private Vector3 FinalClockPos;
    private Vector3 FinalTextPos;
    private float Width;
    private float FinalHeight;
    private float FinalAlpha;

    public void Awake()
    {
        FinalClockPos = ClockTransform.transform.localPosition;
        FinalTextPos = TextTransform.transform.localPosition;

        Width = TopBorder.sizeDelta.x;
        FinalHeight = TopBorder.sizeDelta.y;

        FinalAlpha = BackPanel.color.a;

        ObjectBase.SetActive(false);
    }
    public void StartDisplay(TimePassTrigger data)
    {
        StartCoroutine(TimePassingCutscene(data));
    }

    public IEnumerator TimePassingCutscene(TimePassTrigger data)
    {
        TopBorder.sizeDelta = new Vector2 (Width, 0);
        BottomBorder.sizeDelta = new Vector2(Width, 0);

        BackPanel.color = new Color(0, 0, 0, 0);

        Vector3 startClockPos = new Vector3(FinalClockPos.x, 800f, FinalClockPos.z);
        ClockTransform.transform.localPosition = startClockPos;

        Vector3 startTextPos = new Vector3(FinalTextPos.x, -600f, FinalTextPos.z);
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

            TopBorder.sizeDelta = new Vector2(Width, Mathf.Lerp(0, FinalHeight, progress));
            BottomBorder.sizeDelta = new Vector2(Width, Mathf.Lerp(0, FinalHeight, progress));

            ClockTransform.transform.localPosition = Vector3.Lerp(startClockPos, FinalClockPos, progress);
            TextTransform.transform.localPosition = Vector3.Lerp(startTextPos, FinalTextPos, progress);

            BackPanel.color = new Color(0, 0, 0, Mathf.Lerp(0, FinalAlpha, progress));

            yield return null;
        }

        TopBorder.sizeDelta = new Vector2(Width, FinalHeight);
        BottomBorder.sizeDelta = new Vector2(Width, FinalHeight);

        ClockTransform.transform.localPosition = FinalClockPos;
        TextTransform.transform.localPosition = FinalTextPos;

        BackPanel.color = new Color(0, 0, 0, FinalAlpha);

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

            TopBorder.sizeDelta = new Vector2(Width, Mathf.Lerp(FinalHeight, 0, progress));
            BottomBorder.sizeDelta = new Vector2(Width, Mathf.Lerp(FinalHeight, 0, progress));

            ClockTransform.transform.localPosition = Vector3.Lerp(FinalClockPos, startClockPos, progress);
            TextTransform.transform.localPosition = Vector3.Lerp(FinalTextPos, startTextPos, progress);

            BackPanel.color = new Color(0, 0, 0, Mathf.Lerp(FinalAlpha, 0, progress));

            yield return null;
        }

        ObjectBase.SetActive(false);
    }

}
