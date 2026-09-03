using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class SecretOCTriggerScript : OCUnlockTriggerScript
{

    public static Dictionary<int, SecretOCTriggerScript> SecretOCDict = new();
    public int PhoneID;

    public override void OnEnable()
    {
        base.OnEnable();
        SecretOCDict.Add(PhoneID, this);
    }
    public override void OnDisable()
    {
        base.OnDisable();
        SecretOCDict.Remove(PhoneID);
    }

    public static void SubmitPhoneNumber(int phoneID)
    {
        if(SecretOCDict.ContainsKey(phoneID)) SecretOCDict[phoneID].Release();
    }
}
