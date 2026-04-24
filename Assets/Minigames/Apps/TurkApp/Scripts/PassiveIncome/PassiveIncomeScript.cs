using System.Collections;
using TMPro;
using UnityEngine;

public class PassiveIncomeScript : MonoBehaviour
{
    public static PassiveIncomeScript instance;

    private static bool PassiveIncomeActive = false;
    private static float PassiveIncomeQuantity = 0f;

    private static float PayoutPeriod = 10f;

    public TMP_Text PassiveIncomeText;

    public GameObject UpgradePanel;

    public void Awake()
    {
        instance = this;
        PassiveIncomeText.gameObject.SetActive(false);
        PassiveIncomeText.text = "";
    }

    public static void StartPassiveIncome()
    {
        PassiveIncomeActive = true;
        PassiveIncomeQuantity = 0f;
        instance.PassiveIncomeText.gameObject.SetActive(true);
        instance.UpdateText();
        instance.StartPayoutCoroutine();
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
                if (UpgradePanel.activeSelf)
                {
                    PassiveIncomeText.text = "Passive Income Paused While Blessing";
                }
                else
                {
                    timePassed += Time.deltaTime;
                    UpdateText();
                }
                yield return null;
            }
            CurrencyData.Credits += PassiveIncomeQuantity;
            if (!AppScript.CheckIfActive("Visions")) continue;
            GetComponentInChildren<Animator>().Play("PopUp");
            GetComponent<AudioSource>().Play();
        }
    }

    public static void IncreasePassiveIncome(float increase)
    {
        PassiveIncomeQuantity += increase;
        instance.UpdateText();
    }

    public static void ImproveIncomePeriod(float newPeriod)
    {
        if (newPeriod < PayoutPeriod)
        {
            PayoutPeriod = newPeriod;
            instance.UpdateText();
            instance.GetComponent<AudioSource>().volume = 0.2f / (10 / newPeriod);
        }
    }

    public void UpdateText()
    {
        PassiveIncomeText.text = $"+ <sprite index=1> <b>{PassiveIncomeQuantity.NumberToString().TrimEnd()}</b> <size=15>EVERY</size> <b>{PayoutPeriod}</b> <size=15>SECS</size>";
    }

    public static bool isPassiveIncomeActive()
    {
        return PassiveIncomeActive;
    }
}
