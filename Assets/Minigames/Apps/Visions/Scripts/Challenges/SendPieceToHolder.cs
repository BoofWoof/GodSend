using System.Collections.Generic;
using UnityEngine;

public class SendPieceToHolder : MonoBehaviour
{
    public List<GameObject> PiecePrefabs;

    public void SendPiece()
    {
        foreach(GameObject piecePrefab in PiecePrefabs)
        {
            GameObject pieceClone = Instantiate(piecePrefab);
            PieceHolderScript phs = pieceClone.GetComponent<PieceHolderScript>();

            TurkPuzzleScript.puzzlePiece.Add(phs);
            phs.SendToPieceHolder(pieceClone);
            phs.AddFakeSquares();
        }

    }
}
