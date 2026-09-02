using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "AlphaMovieSO", menuName = "Scriptable Objects/AlphaMovieSO")]
public class AlphaMovieSO : ScriptableObject
{
    public VideoClip ColorClip;
    public VideoClip AlphaClip;
    public AudioClip MovieAudio;
}
