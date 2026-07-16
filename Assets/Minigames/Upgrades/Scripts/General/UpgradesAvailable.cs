using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesAvailable : MonoBehaviour
{
    public GameObject ButtonGlow;

    public UpgradeScreenScript UpgradeScreen;

    public TMP_Text UpgradeText;
    public Color AvailableTextColor;
    private Color UpgradeColor;

    public Color StartFillColor;
    public Color CompleteFillColor;
    public Image FillPanel;

    public void Start()
    {
        if (ButtonGlow == null) return;
        StartCoroutine(StartUpgradeCheck());
        UpgradeColor = UpgradeText.color;
    }

    public IEnumerator StartUpgradeCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            float UpgradePercentage = UpgradeScreen.UpgradePercent();
            bool UpgradeAvailable = UpgradePercentage >= 1f;

            ButtonGlow.SetActive(UpgradeAvailable);
            if (UpgradeAvailable) UpgradeText.color = AvailableTextColor;
            else UpgradeText.color = UpgradeColor;

            if (UpgradeAvailable) FillPanel.color = CompleteFillColor;
            else FillPanel.color = StartFillColor;

            FillPanel.fillAmount = UpgradePercentage;
        }
    }
}
