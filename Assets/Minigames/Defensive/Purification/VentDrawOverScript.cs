using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VentDrawOverScript : MonoBehaviour
{
    public Material WrongColor;
    public Material CorrectColor;
    public Material DefaultMaterial;

    public float TransitionPeriod = 3f;

    Coroutine FadeIn;

    public bool Active = false;

    public void DrawRoutes(List<VentRouteData> ventDatas)
    {
        foreach (VentRouteData ventData in ventDatas)
        {
            bool failedRoute = ventData.LeakFound || !ventData.GoalFound;
            Material successMaterial = failedRoute ? WrongColor : CorrectColor;

            ListTint(ventData.PrimaryExpanded, successMaterial, true);
            ListTint(ventData.SecondaryExpanded, successMaterial, false);
        }
    }

    public void ListTint(List<PipeStackScript> pipeList, Material targetMaterial, bool primary)
    {
        foreach (PipeStackScript targetPipe in pipeList)
        {
            if(targetPipe.isSource || targetPipe.isGoal)
            {
                continue;
            }

            if (primary)
            {
                if (targetPipe.isCapped) continue;
                targetPipe.Pipe.GetComponent<Image>().material = targetMaterial; 
            } else
            {
                if (targetPipe.isNormalWithCaps) continue;
                targetPipe.PipeSecondLayer.GetComponent<Image>().material = targetMaterial;
            }
        }
    }

    public void StartTintFadeIn()
    {
        if (Active) return;
        FadeIn = StartCoroutine(TintFadeIn());
    }

    public IEnumerator TintFadeIn()
    {
        Active = true;
        float timePassed = 0;

        while (timePassed < TransitionPeriod)
        {
            timePassed += Time.deltaTime;
            Shader.SetGlobalFloat("_TintPower", timePassed / TransitionPeriod);
            yield return null;
        }

        Shader.SetGlobalFloat("_TintPower", 1);
    }

    public void ResetTint()
    {
        if (FadeIn != null) StopCoroutine(FadeIn);
        Shader.SetGlobalFloat("_TintPower", 0);
        Active = false;
    }
}
