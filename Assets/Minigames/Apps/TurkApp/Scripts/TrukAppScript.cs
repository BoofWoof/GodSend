using UnityEngine;

public class TrukAppScript : AppScript
{
    public static Canvas PhoneScreenCanvas;
    public Canvas phoneScreenCanvas;

    public int StartSongInt;

    public void StartSong()
    {
        MusicSelectorScript.SetPhoneSong(StartSongInt, true);
    }
    public void EndSong()
    {
        MusicSelectorScript.SetPhoneSong(MusicSelectorScript.instance.DefaultStartSongPhoneID, true);
    }

    private void OnEnable()
    {
        base.OnEnable();
        OnShowApp += StartSong;
        OnHideApp += EndSong;
    }

    private void OnDisable()
    {
        base.OnDisable();
        OnShowApp -= StartSong;
        OnHideApp -= EndSong;
    }

    public void Awake()
    {
        base.Awake();
        PhoneScreenCanvas = phoneScreenCanvas;
    }
}
