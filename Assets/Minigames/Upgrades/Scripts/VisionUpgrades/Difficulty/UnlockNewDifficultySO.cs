
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VisionDifficultyUpgrade", menuName = "Upgrades/Difficulty/VisionDifficultyUpgrade")]
public class UnlockNewDifficultySO : UpgradesAbstract
{
    public override void OnBuy()
    {
        TurkPuzzleScript.instance.UnlockNewDifficulty();
        if(DayInfo.CurrentDay <= 1) TurkPuzzleScript.instance.IncreaseDifficultyToMax();
        //ActiveBroadcast.BroadcastActivation("DifficultyGlow");
        //VisionMascotScript.OnNewDifficutlyUnlocked();
    }
}
