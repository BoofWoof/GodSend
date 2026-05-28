using UnityEngine;

public class TriggerAerialDefense : MonoBehaviour
{
    public void Trigger()
    {
        AerialDefenseScript.Instance.StartLevel();
    }
}
