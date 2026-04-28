using UnityEngine;

public class OverworldAngryRockScript : MonoBehaviour
{
    public static OverworldAngryRockScript instance;

    public AudioSource HappyBaaAudio;
    public AudioSource MidBaaAudio;
    public AudioSource AngryBaaAudio;
    public AudioSource DeathAudio;

    public bool WarningZero = false;
    public bool WarningOne = false;
    public bool WarningTwo = false;
    public bool Death = false;

    public void Awake()
    {
        instance = this;
    }

    public static void PlayHappyBaa()
    {
        instance.HappyBaaAudio.Play();
    }

    public static void PlayAngryBaa()
    {
        instance.AngryBaaAudio.Play();
    }

    public static void Reset()
    {
        instance.WarningZero = false;
        instance.WarningOne = false;
        instance.WarningTwo = false;
        instance.Death = false;
    }

    public void Update()
    {
        float timePassed = AngyRockScript.TimePassed();
        if (timePassed > 60 && !WarningZero)
        {
            WarningZero = true;
            MidBaaAudio.Play();
        }
        if (timePassed > 4 * 60 && !WarningOne) { 
            WarningOne = true;
            AngryBaaAudio.Play();
        }
        if (timePassed > 4 * 60 + 50 && !WarningTwo)
        {
            WarningTwo = true;
            AngryBaaAudio.Play();
        }
        if (timePassed > 5 * 60 && !Death)
        {
            Death = true;
            DeathAudio.Play();
        }
    }

}
