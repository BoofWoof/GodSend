using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterSpeechScript))]
public class CharacterActivationScript : MonoBehaviour
{
    public GameObject CharacterRoot;
    public string TakenName;
    private static Dictionary <string, CharacterActivationScript> CharacterActivationScripts = new Dictionary<string, CharacterActivationScript>();
    private static List<CharacterActivationScript> CharacterActivationScriptsList = new List<CharacterActivationScript>();


    public void OnEnable()
    {
        CharacterSpeechScript cSS = GetComponent<CharacterSpeechScript>();
        TakenName = cSS.SpeakerName;
        CharacterActivationScriptsList.Add(this);
        CharacterActivationScripts.Add(cSS.SpeakerName, this);
        CharacterActivationScripts.Add(cSS.NickName, this);
    }

    public void OnDestroy()
    {
        CharacterActivationScriptsList.Clear();
        CharacterActivationScripts.Clear();
    }

    public static void DisableCharacter(string targetName)
    {
        if (!CharacterActivationScripts.ContainsKey(targetName)) return;
        CharacterActivationScripts[targetName].CharacterRoot.SetActive(false);
    }

    public static void EnableCharacter(string targetName)
    {
        if (!CharacterActivationScripts.ContainsKey(targetName)) return;
        CharacterActivationScripts[targetName].CharacterRoot.SetActive(true);
    }

    public static List<CharacterActivationScript> GetAllActivationScripts()
    {
        return CharacterActivationScriptsList;
    }
}
