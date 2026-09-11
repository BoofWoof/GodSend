using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static AppNotificationScript;

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
