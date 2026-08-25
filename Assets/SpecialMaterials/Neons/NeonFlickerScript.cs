using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class NeonFlickerScript : MonoBehaviour
{
    public Renderer TargetRenderer;
    public UnityEvent OnSpark;

    public float FlickerLength = 2f;
    public Vector2 FlickerPeriodRange;

    public AnimationCurve FlickerCurve;

    public void Start()
    {
        StartCoroutine(FlickerCoroutine());
    }

    public IEnumerator FlickerCoroutine()
    {
        Material[] possibleMaterials = TargetRenderer.sharedMaterials;
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(FlickerPeriodRange.x, FlickerPeriodRange.y));

            float timePassed = 0f;
            Material targetMaterial = possibleMaterials[Random.Range(0, possibleMaterials.Length)];

            OnSpark?.Invoke();
            while(timePassed < FlickerLength)
            {
                float progress = timePassed / FlickerLength;

                targetMaterial.SetFloat("_FlickerDim", FlickerCurve.Evaluate(progress));

                timePassed += Time.deltaTime;

                yield return null;
            }
            targetMaterial.SetFloat("_FlickerDim", 1);
        }
    }
}
