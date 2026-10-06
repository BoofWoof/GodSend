using PixelCrushers;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FirstTimeSave : Saver
{
    private static List<PossibleFirsts> Firsts;
    public static Action<PossibleFirsts> OnNewFirst;

    //Always add new values to the end.
    public enum PossibleFirsts
    {
        Alesssandro,
        Sid,
        Deimos,
        Vape,
        Missile
    }

    public override void Awake()
    {
        base.Awake();

        Firsts = new List<PossibleFirsts>();
    }

    //Has a LUA command AddNewFirst.
    public static void AddNewFirst(PossibleFirsts firstName)
    {
        if (!Firsts.Contains(firstName))
        {
            OnNewFirst?.Invoke(firstName);
            Firsts.Add(firstName);
            Debug.Log($"New first added {firstName}.");
        }
    }

    public static void AddNewFirstByString(string firstName)
    {
        if (Enum.TryParse(firstName, true, out PossibleFirsts caseInsensitiveResult))
        {
            AddNewFirst(caseInsensitiveResult);
        }
        else
        {
            Debug.LogError($"INVALID ENUM NAME {firstName}.");
        }
    }

    public override void ApplyData(string s)
    {
        throw new System.NotImplementedException();
    }

    public override string RecordData()
    {
        throw new System.NotImplementedException();
    }
}
