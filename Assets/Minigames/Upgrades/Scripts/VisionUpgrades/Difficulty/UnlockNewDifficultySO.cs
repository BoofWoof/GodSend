
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VisionDifficultyUpgrade", menuName = "Upgrades/Difficulty/VisionDifficultyUpgrade")]
public class UnlockNewDifficultySO : UpgradesAbstract
{
    public GameObject VisionChallengePrefab;
    public bool OnlyRunChallengeDayOne;

    public override void OnBuy(bool load)
    {
        if (OnlyRunChallengeDayOne && DayInfo.CurrentDay == 1) return; 
        TurkPuzzleScript.instance.StartChallenge(VisionChallengePrefab);

        //TurkPuzzleScript.instance.UnlockNewDifficulty();
        //if(DayInfo.CurrentDay <= 1) TurkPuzzleScript.instance.IncreaseDifficultyToMax();



        //ActiveBroadcast.BroadcastActivation("DifficultyGlow");
        //VisionMascotScript.OnNewDifficutlyUnlocked();
    }
}
