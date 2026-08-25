using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VisionChallengeScript : MonoBehaviour
{
    [Serializable]
    public class SolutionData
    {
        public VisionEmptyGroup TargetEmptyGroup;
        public bool MustBeSolved;
        public bool MustNotBeSolved;
    }

    public string ChallengeName;

    [TextArea] public string MascotEntranceText;
    [TextArea] public string MascotExitText;

    public bool QuickWin = false;
    public UnityEvent OnCorrectSolution;
    public bool QuickLoss = false;
    public UnityEvent OnIncorrectSolution;

    public List<SolutionData> Solutions = new();
    public List<PieceHolderScript> CustomPieces = new();


    public void StartChallenge()
    {
        TurkPuzzleScript.instance.PuzzleEarningsText.gameObject.SetActive(false);

        TurkPuzzleScript.puzzlePiece = CustomPieces;
        foreach(PieceHolderScript piece in CustomPieces)
        {
            piece.AddFakeSquares();
        }
        foreach(SolutionData solution in Solutions)
        {
            solution.TargetEmptyGroup.RefindPieces();
        }

        TurkPuzzleScript.instance.Shuffle();

        TurkPuzzleScript.instance.PlacePieces();
        TurkPuzzleScript.instance.ScrambleCords();
        TurkPuzzleScript.instance.ShowArtist(false);
        TurkPuzzleScript.instance.UpdateStatText(false);

        TurkPuzzleScript.instance.StartCoroutine(TurkPuzzleScript.instance.GraphicScan());
    }

    public void CheckForWin()
    {
        bool AllCorrectSolved = true;
        bool AnyIncorrectSolved = false;
        foreach (SolutionData solution in Solutions)
        {
            if (solution.MustBeSolved && !solution.TargetEmptyGroup.CheckForWin())
            {
                AllCorrectSolved = false;
            }
            if (solution.MustNotBeSolved && solution.TargetEmptyGroup.CheckForWin())
            {
                AnyIncorrectSolved = true;
            }
        }
        if (AnyIncorrectSolved)
        {
            OnIncorrectSolution?.Invoke();
            return;
        }
        if (AllCorrectSolved)
        {
            OnCorrectSolution?.Invoke();
        }
    }
}
