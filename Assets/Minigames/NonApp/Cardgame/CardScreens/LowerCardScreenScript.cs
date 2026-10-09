using System.Collections.Generic;
using UnityEngine;

public class LowerCardScreenScript : CardScreenScript
{
    public static List<LowerCardScreenScript> LowerCardScreens = new();

    public void Start()
    {
        LowerCardScreens.Add(this);
        gameObject.SetActive(false);
    }

    public void OnDestroy()
    {
        LowerCardScreens.Remove(this);
    }

    public static void SetAllScreens(bool on)
    {
        foreach(LowerCardScreenScript lowerScreen in LowerCardScreens)
        {
            lowerScreen.gameObject.SetActive(on);
        }
    }
}
