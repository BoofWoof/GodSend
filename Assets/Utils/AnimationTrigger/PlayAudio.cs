using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    public AudioSource targetAudio;

    public void Trigger()
    {
        targetAudio.Play();
    }
}
