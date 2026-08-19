using UnityEngine;
using UnityEngine.Audio;

public static class AudioExtensions
{
    public static void PlayClipAtPointWithMixer(AudioClip clip, Vector3 position, AudioMixerGroup mixerGroup, float volume = 1f)
    {
        // Create temporary GameObject
        GameObject tempGO = new GameObject("TempAudioSource");
        tempGO.transform.position = position;

        // Add and configure AudioSource
        AudioSource audioSource = tempGO.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.outputAudioMixerGroup = mixerGroup;
        audioSource.spatialBlend = 1.0f;

        audioSource.Play();

        Object.Destroy(tempGO, clip.length);
    }
}