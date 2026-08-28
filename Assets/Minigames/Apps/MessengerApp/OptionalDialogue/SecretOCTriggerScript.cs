using System.Collections.Generic;

public class SecretOCTriggerScript : OCUnlockTriggerScript
{
    public static Dictionary<int, SecretOCTriggerScript> SecretOCDict = new();
    public int PhoneID;

    public void OnEnable()
    {
        SecretOCDict.Add(PhoneID, this);
    }
    public void OnDisable()
    {
        SecretOCDict.Remove(PhoneID);
    }

    public static void SubmitPhoneNumber(int phoneID)
    {
        if(SecretOCDict.ContainsKey(phoneID)) SecretOCDict[phoneID].Release();
    }
}
