using UnityEngine;

namespace DebugTools.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "ClutterReveal", menuName = "DebugCommands/ClutterReveal")]
    public class ClutterRevealCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            ClutterScript.RevealAll();

            return true;
        }
    }
}