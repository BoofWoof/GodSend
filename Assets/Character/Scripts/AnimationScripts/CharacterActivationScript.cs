using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterSpeechScript))]
public class CharacterActivationScript : MonoBehaviour
{
    public GameObject CharacterRoot;
    private static Dictionary <string, CharacterActivationScript> CharacterActivationScripts = new Dictionary<string, CharacterActivationScript>();


    public void OnEnable()
    {
        CharacterSpeechScript cSS = GetComponent<CharacterSpeechScript>();
        CharacterActivationScripts.Add(cSS.SpeakerName, this);
        CharacterActivationScripts.Add(cSS.NickName, this);
    }

    public void OnDestroy()
    {
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
}
