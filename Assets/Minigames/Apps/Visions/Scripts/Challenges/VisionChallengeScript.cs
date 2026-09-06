using System;
using System.Collections;
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

    public float ColorSwapPeriod = 2f;
    public float ColorHoldPeriod = 1f;

    public UnityEvent OnStartEvents;

    public UnityEvent OnCorrectSolution;
    public Color CorrectPieceColor = Color.green;
    public UnityEvent AfterCorrectColorSwap;

    public UnityEvent OnIncorrectSolution;
    public Color IncorrectPieceColor = Color.red;
    public UnityEvent AfterIncorrectColorSwap;

    public List<SolutionData> Solutions = new();
    public List<PieceHolderScript> CustomPieces = new();

    public AudioArrayScript AltPlacementAudio;

    public Material ReplaceOffMat;
    public Material ReplaceOnMat;

    [Header("Piece Treatment")]
    public bool LeavePiecesOnBoard;

    [Header("Display")]
    public bool HidePieceHolder = false;
    public bool HideRedoTutorial = false;
    public bool HideTalkToBird = false;
    public bool HideDifficultyStats = true;
    public bool HideCompletionstStats = true;


    public void StartChallenge()
    {
        if(HidePieceHolder) TurkPuzzleScript.instance.PieceHolder.SetActive(false);
        if (HideRedoTutorial) TurkPuzzleScript.instance.RedoTutorialButton.SetActive(false);
        if (HideTalkToBird) TurkPuzzleScript.instance.TalkToBirdButton.SetActive(false);
        if (HideDifficultyStats) TurkPuzzleScript.instance.DifficultyStats.SetActive(false);
        if (HideCompletionstStats) TurkPuzzleScript.instance.CompletionistStats.SetActive(false);

        OnStartEvents?.Invoke();

        GameStateMonitor.ChallengeActive = true;

        TurkPuzzleScript.instance.PuzzleEarningsText.gameObject.SetActive(false);
        TurkPuzzleScript.instance.FundsObject.SetActive(false);

        TurkPuzzleScript.puzzlePiece.Clear();
        foreach (PieceHolderScript piece in CustomPieces)
        {
            if (piece.LockPiece) continue;
            TurkPuzzleScript.puzzlePiece.Add(piece);
        }

        foreach(PieceHolderScript piece in CustomPieces)
        {
            piece.AddFakeSquares();
        }
        foreach(SolutionData solution in Solutions)
        {
            solution.TargetEmptyGroup.RefindPieces();
            solution.TargetEmptyGroup.ReLockToGrid();
        }

        TurkPuzzleScript.instance.Shuffle();

        if (LeavePiecesOnBoard)
        {
            foreach (PieceHolderScript piece in CustomPieces)
            {
                piece.RelockPiece();
                PieceHolderScript.PieceHolderRestraint = false;
            }
        } else
        {
            TurkPuzzleScript.instance.PlacePieces();
            TurkPuzzleScript.instance.ScrambleCords();
        }

        TurkPuzzleScript.instance.ShowArtist(false);
        TurkPuzzleScript.instance.UpdateStatText(false);

        TurkPuzzleScript.instance.StartCoroutine(TurkPuzzleScript.instance.GraphicScan());

        if (!string.IsNullOrEmpty(MascotEntranceText))
        {
            VisionMascotScript.SayText(MascotEntranceText);
        }
    }

    public void ChangeExitText(string newExitText)
    {
        MascotExitText = newExitText;
    }

    public void CheckForWin()
    {
        bool AllCorrectSolved = true;
        bool AnyIncorrectSolved = false;

        int CorrectSolved = 0;

        foreach (SolutionData solution in Solutions)
        {
            if (solution.TargetEmptyGroup.CheckForWin())
            {
                CorrectSolved++;
            }
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
            StartCoroutine(OnLose());
            return;
        }
        if (AllCorrectSolved)
        {
            StartCoroutine(OnWin());
        }
        Debug.Log($"Solved: {CorrectSolved}");
    }

    public IEnumerator OnLose()
    {
        OnIncorrectSolution?.Invoke();
        yield return TurkPuzzleScript.instance.StartCoroutine(TurkPuzzleScript.instance.RevealShine(IncorrectPieceColor, ColorSwapPeriod));
        yield return new WaitForSeconds(ColorHoldPeriod);
        AfterIncorrectColorSwap?.Invoke();
    }

    public IEnumerator OnWin()
    {
        OnCorrectSolution?.Invoke();
        yield return TurkPuzzleScript.instance.StartCoroutine(TurkPuzzleScript.instance.RevealShine(CorrectPieceColor, ColorSwapPeriod));

        if (!string.IsNullOrEmpty(MascotExitText)) VisionMascotScript.SayText(MascotExitText);
        while (VisionMascotScript.instance.MascotTextIsActive())
        {
            yield return null;
        }

        yield return new WaitForSeconds(ColorHoldPeriod);
        AfterCorrectColorSwap?.Invoke();
    }

    public void StartNewChallenge(GameObject challenge)
    {
        TurkPuzzleScript.instance.EndChallenge();
        TurkPuzzleScript.instance.StartChallenge(challenge);
        TurkPuzzleScript.instance.ResetShine();
    }

    public void ResetChallenge()
    {
        TurkPuzzleScript.instance.ResetChallenge();
        TurkPuzzleScript.instance.ResetShine();
    }

    public void CompleteChallenge()
    {
        if (HidePieceHolder) TurkPuzzleScript.instance.PieceHolder.SetActive(true);
        if (HideRedoTutorial) TurkPuzzleScript.instance.RedoTutorialButton.SetActive(true);
        if (HideTalkToBird) TurkPuzzleScript.instance.TalkToBirdButton.SetActive(true);
        if (HideDifficultyStats) TurkPuzzleScript.instance.DifficultyStats.SetActive(true);
        if (HideCompletionstStats) TurkPuzzleScript.instance.CompletionistStats.SetActive(true);

        TurkPuzzleScript.instance.ResetShine();
        TurkPuzzleScript.instance.EndChallenge();
        UpgradesAbstract.ChallengeReward.OnBuy();
        GameStateMonitor.ChallengeActive = false;
        if(DayInfo.CurrentDay <= 1) TurkPuzzleScript.instance.IncreaseDifficultyToMax();
        TurkPuzzleScript.instance.FundsObject.SetActive(true);
    }
}
