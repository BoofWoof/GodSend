using System.Collections;
using UnityEngine;

public class UpgradesAvailable : MonoBehaviour
{
    public GameObject ButtonGlow;

    public UpgradeScreenScript UpgradeScreen;

    public void Start()
    {
        if (ButtonGlow == null) return;
        StartCoroutine(StartUpgradeCheck());
    }

    public IEnumerator StartUpgradeCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            ButtonGlow.SetActive(UpgradeScreen.UpgradeAffordable());
        }
    }
}
