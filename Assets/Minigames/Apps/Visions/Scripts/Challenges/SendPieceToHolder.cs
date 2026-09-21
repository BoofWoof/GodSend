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
            pieceClone.transform.SetParent(TruePieceHolderScript.instance.transform);
            phs.SendToPieceHolder(pieceClone);
            phs.AddFakeSquares();
            phs.SendOffboard();
            phs.transform.localRotation = Quaternion.identity;
        }

        AppScript targetApp = AppScript.AppsDict["Visions"];
        string appName = targetApp.AppName;
        string previewText = $"A new piece has been added to your <b>{appName}</b>'s board!";

        AppNotificationScript.SetNotification(new AppNotificationScript.NotificationInfo
        {
            SourceApp = targetApp,
            PreviewImage = targetApp.AssociatedIcon,
            PreviewText = previewText,
            AdditionalActions = null
        });

    }
}
