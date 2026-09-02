using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ScreenMoviePlayerScript : MonoBehaviour
{
    public static ScreenMoviePlayerScript instance;

    public VideoPlayer ColorChannel;
    public VideoPlayer AlphaChannel;
    public AudioSource AudioComponent;
    public Image VideoPanel;

    public Image DimPanel;
    private Color DimPanelTarget;
    public float FadeInPeriod = 0.2f;

    public bool VideoPlaying;

    public void Awake()
    {
        instance = this;
        DimPanelTarget = DimPanel.color;
        DimPanel.gameObject.SetActive(false);
        VideoPanel.gameObject.SetActive(false);
    }

    public Coroutine PlayVideo(AlphaMovieSO targetMovie)
    {
        if (VideoPlaying) return null;
        return StartCoroutine(PlayVideoCoroutine(targetMovie));
    }

    public IEnumerator PlayVideoCoroutine(AlphaMovieSO targetMovie)
    {
        VideoPlaying = true;

        DimPanel.color = Color.clear;

        DimPanel.gameObject.SetActive(true);

        ColorChannel.clip = targetMovie.ColorClip;
        ColorChannel.Play();
        AlphaChannel.clip = targetMovie.AlphaClip;
        AlphaChannel.Play();

        if (targetMovie.MovieAudio != null)
        {
            AudioComponent.clip = targetMovie.MovieAudio;
            AudioComponent.Play();
        }

        VideoPanel.gameObject.SetActive(true);
        VideoPanel.color = Color.white;

        float timePassed = 0f;
        while (timePassed < FadeInPeriod)
        {
            timePassed += Time.deltaTime;
            float progress = timePassed / FadeInPeriod;
            DimPanel.color = Color.Lerp(Color.clear, DimPanelTarget, progress);
            yield return null;
        }
        DimPanel.color = DimPanelTarget;

        yield return new WaitForSeconds((float)targetMovie.ColorClip.length - FadeInPeriod);

        VideoPlaying = false;

        timePassed = 0f;
        while (timePassed < (FadeInPeriod*2f))
        {
            timePassed += Time.deltaTime;
            float progress = timePassed / (FadeInPeriod*2f);
            Color targetColor = Color.Lerp(DimPanelTarget, Color.clear, progress);
            DimPanel.color = targetColor;
            VideoPanel.color = targetColor;
            yield return null;
        }
        DimPanel.color = Color.clear;

        DimPanel.gameObject.SetActive(false);
        VideoPanel.gameObject.SetActive(false);

        Graphics.SetRenderTarget(ColorChannel.targetTexture);
        GL.Clear(true, true, Color.clear);
        Graphics.SetRenderTarget(null);
        Graphics.SetRenderTarget(AlphaChannel.targetTexture);
        GL.Clear(true, true, Color.clear);
        Graphics.SetRenderTarget(null);
    }
}
