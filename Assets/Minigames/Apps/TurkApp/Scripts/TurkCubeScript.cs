using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TurkCubeScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Vector2Int cord;

    public Dictionary<Directions, bool> CardinalExpands = new Dictionary<Directions, bool>
    {
        {Directions.Up, false},
        {Directions.Left, false},
        {Directions.Right, false},
        {Directions.Down, false}
    };

    public Dictionary<Directions, bool> DiagonalExpands = new Dictionary<Directions, bool>
    {
        {Directions.LowerLeft, false},
        {Directions.UpperLeft, false},
        {Directions.LowerRight, false},
        {Directions.UpperRight, false}
    };

    public Dictionary<Directions, bool> ConnectedDirections = new Dictionary<Directions, bool>
    {
        {Directions.Up, false},
        {Directions.Left, false},
        {Directions.Right, false},
        {Directions.Down, false}
    };

    public bool FullyCardinallyExpanded = false;
    public bool FullyDiagonallyExpanded = false;

    public bool Linked = false;

    public int GroupID = -1;


    public List<TurkCubeScript> ExpandedToScripts = new List<TurkCubeScript>();

    public PieceHolderScript rootPiece;

    public void OnPointerDown(PointerEventData eventData)
    {
        rootPiece.OnPointerDown(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        rootPiece.OnPointerUp(eventData);
    }


    #region Find Other Pieces

    public GameObject AttemptRandomExpand()
    {
        bool expandDiagonally = FullyCardinallyExpanded;

        foreach (TurkCubeScript ExpandedToScript in ExpandedToScripts)
        {
            GameObject expandToObject = ExpandedToScript.AttemptRandomExpand();
            if (expandToObject != null) return expandToObject;
        }
        if (FullyCardinallyExpanded && !expandDiagonally) return null;
        if (FullyDiagonallyExpanded && expandDiagonally) return null;

        List<Directions> directionsLeft = new List<Directions>();
        if (expandDiagonally)
        {
            foreach(KeyValuePair<Directions, bool> expansionKeyValue in DiagonalExpands)
            {
                if(!expansionKeyValue.Value) directionsLeft.Add(expansionKeyValue.Key);
            }
        } else
        {
            foreach (KeyValuePair<Directions, bool> expansionKeyValue in CardinalExpands)
            {
                if (!expansionKeyValue.Value) directionsLeft.Add(expansionKeyValue.Key);
            }
        }
        if (directionsLeft.Count == 0)
        {
            if (expandDiagonally)
            {
                FullyDiagonallyExpanded = true;
            } else
            {
                FullyCardinallyExpanded = true;
            }
            return null;
        }

        while (directionsLeft.Count > 0)
        {
            Directions RandomDir = directionsLeft[Random.Range(0, directionsLeft.Count)];
            GameObject selectedObject = Expand(RandomDir);
            directionsLeft.Remove(RandomDir);

            if (selectedObject != null) return selectedObject;
        }
        return null;
    }

    private GameObject Expand(Directions ExpandDirection)
    {
        if (CardinalExpands.ContainsKey(ExpandDirection)) CardinalExpands[ExpandDirection] = true;
        if (DiagonalExpands.ContainsKey(ExpandDirection)) DiagonalExpands[ExpandDirection] = true;


        Vector2Int expandCordShift = ExpandDirection.ToCordShift();
        if (!TurkPuzzleScript.IsCoordinateInsideGrid(cord.x + expandCordShift.x, cord.y + expandCordShift.y)) return null;
        GameObject expandedTo = TurkPuzzleScript.puzzlePieceGrid[cord.x + expandCordShift.x, cord.y + expandCordShift.y];
        TurkCubeScript expandToScript = expandedTo.GetComponent<TurkCubeScript>();

        if (expandToScript.Linked &&
            expandToScript.GroupID != GroupID
            ) return null;

        if (CardinalExpands.ContainsKey(ExpandDirection))
        {
            ConnectedDirections[ExpandDirection] = true;
            expandToScript.ConnectedDirections[ExpandDirection.FlipDirection()] = true;
        }

        if (expandToScript.Linked) return null;

        if (!ExpandedToScripts.Contains(expandToScript)) ExpandedToScripts.Add(expandToScript);
        expandToScript.Linked = true;
        expandToScript.GroupID = GroupID;

        return expandedTo;
    }

    public void ConnectionCheck()
    {
        foreach (Directions connectionKey in DirectionHelper.CardinalDirections)
        {
            Vector2Int posShift = connectionKey.ToCordShift();
            if (!TurkPuzzleScript.IsCoordinateInsideGrid(cord.x + posShift.x, cord.y + posShift.y))
            {
                ConnectedDirections[connectionKey] = false;
            }
            else
            {
                ConnectedDirections[connectionKey] = TurkPuzzleScript.puzzlePieceGrid[cord.x + posShift.x, cord.y + posShift.y].GetComponent<TurkCubeScript>().GroupID == GroupID;
            }
        }
    }
    #endregion

    public void UpdateSprite()
    {
        transform.localRotation = Quaternion.Euler(0, 0, 90f * rootPiece.Rotations);

        Directions AdjustedUp = Directions.Up;
        Directions AdjustedDown = Directions.Down;
        Directions AdjustedLeft = Directions.Left;
        Directions AdjustedRight = Directions.Right;

        for (int i = 0; i < rootPiece.Rotations; i++)
        {
            AdjustedUp = AdjustedUp.TurnCounterClockwise();
            AdjustedDown = AdjustedDown.TurnCounterClockwise();
            AdjustedLeft = AdjustedLeft.TurnCounterClockwise();
            AdjustedRight = AdjustedRight.TurnCounterClockwise();
        }

        GetComponent<Image>().sprite = TurkPuzzleScript.instance.constallationTiles.GetSprite(
            !ConnectedDirections[AdjustedUp],
            !ConnectedDirections[AdjustedDown],
            !ConnectedDirections[AdjustedLeft],
            !ConnectedDirections[AdjustedRight]
            );
    }
    public void ClearMat()
    {
        Image img = GetComponent<Image>();
        img.material = TurkPuzzleScript.instance.ConstMat;
    }

    public void ActivateMat()
    {
        Image img = GetComponent<Image>();
        img.material = TurkPuzzleScript.instance.ActiveConstMat;
    }
}
