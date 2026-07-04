using UnityEngine;
using UnityEngine.Rendering;

public class AudioArrayScript : MonoBehaviour
{
    public AudioClip[] AudioClips;
    public AudioSource TargetAudioSource;

    private int CurrentClipIdx = 0;
    private bool FirstPlay = true;

    public int GetClipLength()
    {
        return AudioClips.Length;
    }

    public void PlayAudioByIndex(int targetIndex)
    {
        if(targetIndex >= AudioClips.Length)
        {
            Debug.LogError($"Attempt to play nonexistant audio line at index: {targetIndex}");
            return;
        }
        TargetAudioSource.clip = AudioClips[targetIndex];
        TargetAudioSource.Play();

        CurrentClipIdx = targetIndex;

        FirstPlay = false;
    }

    public void PlayNextAudio()
    {
        if(!FirstPlay) CurrentClipIdx = (CurrentClipIdx + 1) % AudioClips.Length;
        TargetAudioSource.clip = AudioClips[CurrentClipIdx];
        TargetAudioSource.Play();

        FirstPlay = false;
    }
}
