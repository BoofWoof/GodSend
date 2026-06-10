using JetBrains.Annotations;
using UnityEngine;

public class WindTriggerScript : MonoBehaviour
{
    public float NewWindSpeed = 0f;

    public void Trigger()
    {
        WindScript.instance.SetWindTarget(NewWindSpeed);
        Destroy(gameObject);
    }
}
