using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TruePieceHolderScript : MonoBehaviour
{
    //CORG FEATURES
    public static TruePieceHolderScript instance;

    public List<PieceHolderScript> Pieces;

    public int SelectedPiece = 0;

    public Animator PieceHolderAnimator;

    public Button LeftButton;
    public Button RightButton;

    public AudioSource ObjectStorageSound;

    public void Awake()
    {
        instance = this;
    }

    public void MovePieceToCenter()
    {
        PieceHolderScript selectedPiece = Pieces[SelectedPiece];

        MovePieceToCenter(selectedPiece);
    }
    public void MovePieceToCenter(PieceHolderScript selectedPiece)
    {
        selectedPiece.gameObject.SetActive(true);

        selectedPiece.transform.parent = transform;

        Vector2 Offset = selectedPiece.CalcualteCenterOffset();

        selectedPiece.transform.localPosition = -Offset;
    }
    public void InteractableButtonCheck()
    {
        LeftButton.interactable = Pieces.Count > 1;
        RightButton.interactable = Pieces.Count > 1;
    }

    public void HideCurrentPiece()
    {
        PieceHolderScript selectedPiece = Pieces[SelectedPiece];

        selectedPiece.gameObject.SetActive(false);
    }

    public void StorePiece(PieceHolderScript Piece, bool PlaySound = false)
    {
        if(PlaySound) ObjectStorageSound.Play();

        if (Pieces.Contains(Piece)) {
            MovePieceToCenter(Piece);
            return;
        }

        Pieces.Add(Piece);
        InteractableButtonCheck();
        if (Pieces.Count == 1) {
            ShowBoard();
            SelectedPiece = 0;
            MovePieceToCenter();
            PieceHolderScript.PieceHolderRestraint = true;

            PieceHolderScript.SafetyCheckAllPositions();
        } else
        {
            Piece.gameObject.SetActive(false);
        }
    }

    public void RemovePiece(PieceHolderScript Piece)
    {
        if (!Pieces.Contains(Piece)) return;

        Pieces.Remove(Piece);
        InteractableButtonCheck();
        if (Pieces.Count == 0)
        {
            PieceHolderScript.PieceHolderRestraint = false;
            HideBoard();
            return;
        } else
        {
            SelectedPiece = SelectedPiece % Pieces.Count;
            MovePieceToCenter();
        }
    }

    public void NextPiece()
    {
        if (Pieces.Count == 0) return;

        HideCurrentPiece();

        SelectedPiece++;
        SelectedPiece %= Pieces.Count;
        if (SelectedPiece < 0) SelectedPiece += Pieces.Count;

        MovePieceToCenter();
    }

    public void PrevPiece()
    {
        if (Pieces.Count == 0) return;

        HideCurrentPiece();

        SelectedPiece--;
        SelectedPiece %= Pieces.Count;
        if (SelectedPiece < 0) SelectedPiece += Pieces.Count;

        MovePieceToCenter();
    }

    public void HideBoard()
    {
        PieceHolderAnimator.Play("SlideIn");
    }

    public void ShowBoard()
    {
        PieceHolderAnimator.Play("SlideOut");
    }

    public void TurnOnStorePieces()
    {
        PieceHolderScript.StorePiece = true;
    }

    public void TurnOffStorePieces()
    {
        PieceHolderScript.StorePiece = false;
    }
}
