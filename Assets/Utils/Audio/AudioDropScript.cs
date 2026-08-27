using UnityEngine;
using UnityEngine.Audio;

public class AudioDropScript : MonoBehaviour
{
    public AudioClip clip;
    public AudioMixerGroup mixerGroup;
    public float volume;

    public void Trigger()
    {
        AudioExtensions.PlayClipAtPointWithMixer(clip, transform.position, mixerGroup, volume);
    }
}
