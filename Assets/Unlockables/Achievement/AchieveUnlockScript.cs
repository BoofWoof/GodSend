using TMPro;
using UnityEngine;

public class AchieveUnlockScript : MonoBehaviour
{
    public static AchieveUnlockScript instance;

    public TMP_Text AchievementName;

    public void Awake()
    {
        instance = this;
    }

    public void ShowUnlock(string UnlockName)
    {
        AchievementName.text = UnlockName;
        GetComponent<Animator>().Play("Unlock");
        GetComponent<AudioSource>().Play();
    }
}
