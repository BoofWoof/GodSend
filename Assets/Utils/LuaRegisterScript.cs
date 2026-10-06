using PixelCrushers.DialogueSystem;
using UnityEngine;

public class LuaRegisterScript : MonoBehaviour
{
    void Start()
    {
        Lua.RegisterFunction("SetForceEvent", null, SymbolExtensions.GetMethodInfo(() => GameStateMonitor.SetForceEvent()));
        Lua.RegisterFunction("ReleaseForceEvent", null, SymbolExtensions.GetMethodInfo(() => GameStateMonitor.ReleaseForceEvent()));
        Lua.RegisterFunction("StartDay", null, SymbolExtensions.GetMethodInfo(() => DaytaScript.StaticStartDay()));
        Lua.RegisterFunction("UnlockOC", null, SymbolExtensions.GetMethodInfo(() => OCUnlockTriggerScript.UnlockOCByName("")));
        Lua.RegisterFunction("AddNewFirst", null, SymbolExtensions.GetMethodInfo(() => FirstTimeSave.AddNewFirstByString("")));

        Lua.RegisterFunction("SetSong", null, SymbolExtensions.GetMethodInfo(() => MusicSelectorScript.SetOverworldSong(0)));
        Lua.RegisterFunction("FadeInSong", null, SymbolExtensions.GetMethodInfo(() => MusicSelectorScript.FadeInOverworldSong(0)));
        Lua.RegisterFunction("SetPhoneSong", null, SymbolExtensions.GetMethodInfo(() => MusicSelectorScript.SetPhoneSong(0)));
        Lua.RegisterFunction("RevertSong", null, SymbolExtensions.GetMethodInfo(() => MusicSelectorScript.RevertOverworldSong()));
        Lua.RegisterFunction("RevertPhoneSong", null, SymbolExtensions.GetMethodInfo(() => MusicSelectorScript.RevertPhoneSong()));
        Lua.RegisterFunction("PauseMusic", null, SymbolExtensions.GetMethodInfo(() => MusicSelectorScript.PauseMusic()));
        Lua.RegisterFunction("ResumeMusic", null, SymbolExtensions.GetMethodInfo(() => MusicSelectorScript.ResumeMusic()));
    }
}
