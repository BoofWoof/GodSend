using System.Collections;
using TMPro;
using UnityEngine;

public class PassiveIncomeScript : MonoBehaviour
{
    public static PassiveIncomeScript instance;

    private static bool PassiveIncomeActive = false;
    private static float PassiveIncomeQuantity = 0f;
    private static float PassiveIncomeQuantityMultiplier = 1f;
    private static int PassiveIncomeTriggerCount = 0;
    public static bool ActivateTriggerMultiplier = false;

    private static float PayoutPeriod = 3f;

    public TMP_Text PassiveIncomeText;

    public GameObject UpgradePanel;
    public GameObject FocusPanel;
    public GameObject FocusPanel2;

    public void Awake()
    {
        instance = this;
        PassiveIncomeText.gameObject.SetActive(false);
        PassiveIncomeText.text = "";

        PassiveIncomeQuantityMultiplier = 1f;
        PassiveIncomeTriggerCount = 0;
        ActivateTriggerMultiplier = false;
    }

    public static void StartPassiveIncome()
    {
        PassiveIncomeActive = true;
        PassiveIncomeQuantity = 0f;
        instance.PassiveIncomeText.gameObject.SetActive(true);
        instance.UpdateText();
        instance.StartPayoutCoroutine();
    }

    public static void InstantPayout()
    {
        CurrencyData.Credits += PassiveIncomeQuantity * PassiveIncomeQuantityMultiplier;

        if (ActivateTriggerMultiplier)
        {
            PassiveIncomeTriggerCount++;
            PassiveIncomeQuantityMultiplier = 1f + PassiveIncomeTriggerCount / 10f;
        } 
    }

    public void StartPayoutCoroutine()
    {
        StartCoroutine(StartPayout());
    }

    public IEnumerator StartPayout()
    {
        while (true)
        {
            float timePassed = 0;
            while(timePassed < PayoutPeriod)
            {
                if (UpgradePanel.activeSelf || FocusPanel.activeSelf || FocusPanel2.activeSelf)
                {
                    UpdateText("PAUSED");
                }
                else
                {
                    timePassed += Time.deltaTime;
                    UpdateText((PayoutPeriod - timePassed).ToString("F1"));
                }
                yield return null;
            }
            InstantPayout();
            if (!AppScript.CheckIfActive("Visions")) continue;
            GetComponentInChildren<Animator>().Play("PopUp");
            GetComponent<AudioSource>().Play();
        }
    }

    public static void IncreasePassiveIncome(float increase)
    {
        PassiveIncomeQuantity += increase;
        instance.UpdateText();

        InstantPayout();
    }

    public static void ImproveIncomePeriod(float newPeriod)
    {
        if (newPeriod < PayoutPeriod)
        {
            PayoutPeriod = newPeriod;
            instance.UpdateText();
            instance.GetComponent<AudioSource>().volume = 0.2f / (3 / newPeriod);
        }
    }

    public void UpdateText(string timeText = "")
    {
        PassiveIncomeText.text = $"+ <sprite index=1> <b>{PassiveIncomeQuantity.NumberToString().TrimEnd()} x {PassiveIncomeQuantityMultiplier.ToString("F1")}</b> <size=15>IN</size> <b>{timeText}</b> <size=15>SECS</size>";
    }

    public static bool isPassiveIncomeActive()
    {
        return PassiveIncomeActive;
    }
}
