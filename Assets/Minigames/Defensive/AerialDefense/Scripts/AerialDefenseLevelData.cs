using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AerialDefenseLevelData : MonoBehaviour
{
    public string LevelName;
    public List<ADWaveInfoSO> LevelWaves;

    [Header("Quick Dialogues")]
    public string FailureVoiceLinePath;
    public string WinVoiceLinePath;
    public string PerfectWinVoiceLinePath;

    [Header("Full Dialogue")]
    public string OnWinDialogueName;

    [Header ("Dialogue Variables")]
    public string OnFinishVariableName = "LatestAerialOutcome"; //Sets to zero if lose, one if win, and two if flawless victory;

    private int WinType = 0; //Sets to zero if lose, one if win, and two if flawless victory;

    public static Dictionary<string, AerialDefenseLevelData> LevelDictionary = new Dictionary<string, AerialDefenseLevelData>();

    public Volume OptionalVolume;

    public void Start()
    {
        LevelDictionary[LevelName] = this;
        Lua.RegisterFunction("PrepAerial", null, SymbolExtensions.GetMethodInfo(() => PrepLevelByName("")));
        Lua.RegisterFunction("InitiateAerial", null, SymbolExtensions.GetMethodInfo(() => InitiateLevel()));
        Lua.RegisterFunction("PlayAerial", null, SymbolExtensions.GetMethodInfo(() => StartLevelByName("")));
    }

    #region EndPuzzle

    public void MarkLoss()
    {
        WinType = 0;
        UpdateEndVariable();
    }

    public void MarkWin()
    {
        WinType = 1;
        UpdateEndVariable();
    }

    public void MarkPerfectWin()
    {
        WinType = 2;
        UpdateEndVariable();
    }

    public void UpdateEndVariable()
    {
        DialogueLua.SetVariable(OnFinishVariableName, WinType);
    }

    public bool PlayEndingDialogue()
    {
        //Return value determines if the minigame should be replayed.

        if (!string.IsNullOrEmpty(OnWinDialogueName) && WinType > 0)
        {
            MessageQueue.addDialogue(OnWinDialogueName);
            return false;
        } else if (!string.IsNullOrEmpty(WinVoiceLinePath) && WinType == 1)
        {
            CharacterSpeechScript.CentralNode.StartBroadcastSpeechAttempt("", WinVoiceLinePath);
            return false;
        } else if (!string.IsNullOrEmpty(PerfectWinVoiceLinePath) && WinType == 2)
        {
            CharacterSpeechScript.CentralNode.StartBroadcastSpeechAttempt("", PerfectWinVoiceLinePath);
            return false;
        } else if (!string.IsNullOrEmpty(FailureVoiceLinePath))
        {
            CharacterSpeechScript.CentralNode.StartBroadcastSpeechAttempt("", FailureVoiceLinePath);
            return true;
        }
        return false;
    }
    #endregion

    #region StartPuzzle
    public static void PrepLevelByName(string levelName)
    {
        if (LevelDictionary.ContainsKey(levelName))
        {
            LevelDictionary[levelName].PrepLevel();
        } else
        {
            Debug.LogError($"Level name not found: {levelName}.");
        }
    }

    public void PrepLevel()
    {
        AerialDefenseScript.Instance.PreSetLevel(this);
    }

    public static void StartLevelByName(string levelName)
    {
        if (LevelDictionary.ContainsKey(levelName))
        {
            LevelDictionary[levelName].StartLevel();
        }
    }

    public void StartLevel()
    {
        PrepLevel();
        InitiateLevel();
    }

    public static void InitiateLevel()
    {
        AerialDefenseScript.Instance.StartLevel();
    }
    #endregion
}
