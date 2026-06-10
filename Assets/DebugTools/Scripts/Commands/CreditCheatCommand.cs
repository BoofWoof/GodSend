using UnityEngine;

namespace DebugTools.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "NewCreditCheat", menuName = "DebugCommands/CreditCheat")]
    public class CreditCheatCommand : ConsoleCommand
    {
        public override bool Process(string[] args)
        {
            CurrencyData.Credits += double.Parse(args[0]);

            return true;
        }
    }
}