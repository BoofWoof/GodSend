using DebugTools.DeveloperConsole.Commands;
using UnityEngine;

[CreateAssetMenu(fileName = "ForceWinCommand", menuName = "DebugCommands/ForceWinCommand")]
public class ForceWinCommand : ConsoleCommand
{
    public bool ActuallyLoseInstead = false;

    public override bool Process(string[] args)
    {
        ChannelChanger.instance.ForceWinCommand(ActuallyLoseInstead);

        return true;
    }

}
