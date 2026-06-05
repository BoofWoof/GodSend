using UnityEngine;

public class SetDayScript : MonoBehaviour
{
    public void SetDay(int value)
    {
        DayInfo.SetDay(value);
        DaytaScript.ExternalSkipStart = false;
    }
}
