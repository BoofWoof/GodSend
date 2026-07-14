using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MaterialTransitionScript : MonoBehaviour
{
    public GameObject DestructionTarget;

    public string ValueName;
    public float StartingValue = 0.3f;
    public float EndingValue = 1.0f;

    public float Duration = 0.5f;

    public bool DestroyOnCompletition = true;

    public void StartTransitionAnimation()
    {
        if(isActiveAndEnabled) StartCoroutine(TransitionAnimation());
    }
    public IEnumerator TransitionAnimation()
    {
        Image image = GetComponent<Image>();
        Material runtimeMaterial = Instantiate(image.material);
        image.material = runtimeMaterial;

        float elapsed = 0f;
        while (elapsed < Duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / Duration);
            float progress = Mathf.Lerp(StartingValue, EndingValue, t);
            image.materialForRendering.SetFloat(ValueName, progress);
            yield return null;
        }

        // Ensure it ends at 1
        image.materialForRendering.SetFloat(ValueName, EndingValue);

        if(DestroyOnCompletition) Destroy(DestructionTarget);
    }
}
