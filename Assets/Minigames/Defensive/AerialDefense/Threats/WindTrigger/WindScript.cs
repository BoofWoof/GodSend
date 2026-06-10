using System.Collections;
using UnityEngine;

public class WindScript : MonoBehaviour
{
    public static WindScript instance;

    public static float StageWind { get; private set; } = 0f;
    private static float TargetWind = 0f;

    public float UpdateRate = 1f;

    private Coroutine WindCoroutine;

    public delegate void WindChangedDelegate();
    public static event WindChangedDelegate OnWindChanged;

    public void OnEnable()
    {
        instance = this;
        ResetWind();
    }

    public void ResetWind()
    {
        StopCoroutine(WindCoroutine);
        StageWind = 0f;
        TargetWind = 0f;
    }

    public void SetWindTarget(float newTarget)
    {
        TargetWind = newTarget;

        if (WindCoroutine != null) StopCoroutine(WindCoroutine);
        WindCoroutine = StartCoroutine(MoveTowardsTargetWind());
    }

    public IEnumerator MoveTowardsTargetWind()
    {
        while(StageWind != TargetWind)
        {
            StageWind = Mathf.MoveTowards(StageWind, TargetWind, UpdateRate*Time.deltaTime);
            OnWindChanged?.Invoke();
            yield return null;
        }
    }
}
