using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PieceHolderScript : MonoBehaviour
{
    public TurkCubeScript SeedPiece;
    public List<TurkCubeScript> Pieces = new List<TurkCubeScript>();

    public bool FirstRelease = true;

    public int Rotations = 0;
    public int PreviousValidRotations = 0;

    public static bool PickupEnabled = true;
    public static bool RotationEnabled = false;
    public static bool PieceHolderRestraint = true;
    public static bool StorePiece = false;

    private bool isDragging;
    private Vector2 offset;

    public void SetSeedPiece(TurkCubeScript targetPiece)
    {
        SeedPiece = targetPiece;

        transform.position = SeedPiece.transform.position;
        transform.rotation = SeedPiece.transform.rotation;
        transform.localScale = Vector3.one;

        AddPiece(targetPiece);
    }

    public void AddPiece(TurkCubeScript targetPiece)
    {
        Pieces.Add(targetPiece);
        targetPiece.transform.parent = transform;

        targetPiece.rootPiece = this;

        targetPiece.GetComponent<Image>().color = SeedPiece.GetComponent<Image>().color;
        targetPiece.GetComponent<Image>().material.SetColor("_Tint", SeedPiece.GetComponent<Image>().color);
    }

    public GameObject ExpandSeed()
    {
        GameObject newPiece = SeedPiece.AttemptRandomExpand();

        if (newPiece != null) AddPiece(newPiece.GetComponent<TurkCubeScript>());

        return newPiece;
    }

    public bool PieceInValidZone()
    {
        foreach(TurkCubeScript piece in Pieces)
        {
            Vector3 worldPos = piece.transform.TransformPoint(Vector3.zero);
            Vector3 grandparentRelativePos = TurkPuzzleScript.instance.transform.InverseTransformPoint(worldPos);

            if (grandparentRelativePos.x < -473) continue;
            if (PieceHolderRestraint)
            {
                if (grandparentRelativePos.x > 444) continue;
            } else
            {
                if (grandparentRelativePos.x > 1075) continue;
            }
            if (grandparentRelativePos.y < -340) continue;
            if (grandparentRelativePos.y > 419) continue;
            return true;
        }

        return false;
    }

    public bool UpdateCord()
    {
        //Checks if we can.
        foreach (TurkCubeScript puzzlePiece in Pieces)
        {
            RectTransform puzzleTransform = puzzlePiece.GetComponent<RectTransform>();

            Vector3 adjustedPiecePosition = TurkPuzzleScript.instance.transform.InverseTransformPoint(puzzleTransform.position);
            Vector2Int newCord = TurkPuzzleScript.PosToGridIdx((Vector2)adjustedPiecePosition);

            if (TurkPuzzleScript.IsCordTaken(newCord, Pieces)) return false;

            Vector2 newPos = TurkPuzzleScript.GridIdxToPos(new Vector2Int(newCord.x, newCord.y));
        }

        if(!PieceInValidZone()) return false;

        List<TurkCubeScript> fillers = new List<TurkCubeScript>();
        //Applies it.
        foreach (TurkCubeScript puzzlePiece in Pieces)
        {
            RectTransform puzzleTransform = puzzlePiece.GetComponent<RectTransform>();
            Vector2Int oldCord = puzzlePiece.GetComponent<TurkCubeScript>().cord;

            Vector3 adjustedPiecePosition = TurkPuzzleScript.instance.transform.InverseTransformPoint(puzzleTransform.position);
            Vector2Int newCord = TurkPuzzleScript.PosToGridIdx((Vector2)adjustedPiecePosition);

            puzzlePiece.cord = newCord;
            if (TurkPuzzleScript.IsCoordinateInsideGrid(oldCord.x, oldCord.y)) TurkPuzzleScript.holeGrid[oldCord.x, oldCord.y].GetComponent<TurkHoleScript>().EmptyHole();
            fillers.Add(puzzlePiece.GetComponent<TurkCubeScript>());
        }
        foreach (TurkCubeScript filler in fillers)
        {
            Vector2Int cord = filler.cord;
            if (TurkPuzzleScript.IsCoordinateInsideGrid(cord.x, cord.y)) TurkPuzzleScript.holeGrid[cord.x, cord.y].GetComponent<TurkHoleScript>().FillHole(filler);
        }
        return true;
    }
    public void OnEnable()
    {
        PhonePositionScript.PhoneToggled += InterruptDrag;
        InputManager.PlayerInputs.Phone.AppReturn.performed += RotatePiece;
    }
    public void OnDisable()
    {
        PhonePositionScript.PhoneToggled -= InterruptDrag;
        InputManager.PlayerInputs.Phone.AppReturn.performed -= RotatePiece;
    }
    public void InterruptDrag(bool phoneUp)
    {
        PickupEnabled = phoneUp;

        if (phoneUp || !isDragging)
        {
            isDragging = false;
            return;
        }

        isDragging = false;

        if (FirstRelease)
        {
            Debug.Log("Back To Holder");
            SendToPieceHolder();
            return;
        }
        Debug.Log("Back To Place");

        transform.localPosition = TurkPuzzleScript.GridIdxToPos(SeedPiece.cord);

    }

    #region Follow Mouse
    void Update()
    {
        if (isDragging)
        {
            RectTransform canvasRect = TurkPuzzleScript.instance.transform as RectTransform;

            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                Input.mousePosition,
                TrukAppScript.PhoneScreenCanvas.worldCamera,
                out mousePos);

            transform.localPosition = (Vector3)(mousePos + offset);
        }
    }

    public void RotatePiece(InputAction.CallbackContext c)
    {
        if (!isDragging) return;
        if (!RotationEnabled) return;

        Rotations = (Rotations + 1) % 4;

        Vector2 localMousePos;
        RectTransform canvasRect = TurkPuzzleScript.instance.transform as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            TrukAppScript.PhoneScreenCanvas.worldCamera,
            out localMousePos);

        Vector3 worldMousePivot = canvasRect.TransformPoint(localMousePos);


        transform.RotateAround(worldMousePivot, transform.forward, 90f);
        offset = new Vector2(-offset.y, offset.x);

        UpdateAllSprites();
    }

    public void CenterRotate()
    {
        Rotations = (Rotations + 1) % 4;

        Vector2 Offset = CalcualteCenterOffset();

        RectTransform canvasRect = TurkPuzzleScript.instance.transform as RectTransform;
        Vector3 offsetPivot = canvasRect.TransformPoint(Offset);


        transform.RotateAround(offsetPivot, transform.forward, 90f);

        UpdateAllSprites();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!PickupEnabled) return;

        isDragging = true;
        TurkPuzzleScript.instance.Pickup.Play();

        RectTransform canvasRect = TurkPuzzleScript.instance.transform as RectTransform;

        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            TrukAppScript.PhoneScreenCanvas.worldCamera,
            out mousePos);

        ClearAllMat();

        transform.parent = TurkPuzzleScript.puzzleScript.transform;

        offset = (Vector2)transform.localPosition - mousePos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!PickupEnabled || !isDragging) return;

        isDragging = false;
        TurkPuzzleScript.instance.Pickup.Stop();

        if (StorePiece)
        {
            TurkPuzzleScript.instance.Drop.Play();
            SendToPieceHolder();
            return;
        }

        Vector2 startPos = GetComponent<RectTransform>().anchoredPosition;
        Vector2Int gridIdx = TurkPuzzleScript.PosToGridIdx(startPos);
        Vector2 pos = TurkPuzzleScript.GridIdxToPos(gridIdx);
        GetComponent<RectTransform>().anchoredPosition = pos;

        bool successfulUpdate = UpdateCord();

        if (FirstRelease)
        {
            if (successfulUpdate)
            {
                FirstRelease = false;
                TruePieceHolderScript.instance.RemovePiece(this);
            }
            else
            {
                SendToPieceHolder();
                return;
            }
        }

        if (successfulUpdate)
        {
            TurkPuzzleScript.instance.Drop.Play();
        }
        else
        {
            TurkPuzzleScript.instance.DropBad.Play();
            GetComponent<RectTransform>().anchoredPosition = TurkPuzzleScript.GridIdxToPos(SeedPiece.cord);
            return;
        }

        if (TurkPuzzleScript.CheckWin()) return;
    }

    public void SendToPieceHolder()
    {
        ClearAllMat();

        if (!FirstRelease)
        {
            foreach (TurkCubeScript puzzlePiece in Pieces)
            {
                Vector2Int oldCord = puzzlePiece.GetComponent<TurkCubeScript>().cord;
                if (TurkPuzzleScript.IsCoordinateInsideGrid(oldCord.x, oldCord.y)) TurkPuzzleScript.holeGrid[oldCord.x, oldCord.y].GetComponent<TurkHoleScript>().EmptyHole();
                puzzlePiece.GetComponent<TurkCubeScript>().cord = new Vector2Int(-99, -99);
            }
        }

        FirstRelease = true;
        TruePieceHolderScript.instance.StorePiece(this);
    }
    #endregion

    public Vector2 CalcualteCenterOffset()
    {
        // Initialize min and max with the first point in the list
        Vector2 min = Vector2.zero;
        Vector2 max = Vector2.zero;

        // Iterate through the list to find min and max x and y values
        foreach (RectTransform child in transform)
        {
            Vector2 point = child.anchoredPosition;
            if (point.x < min.x) min.x = point.x;
            if (point.y < min.y) min.y = point.y;

            if (point.x > max.x) max.x = point.x;
            if (point.y > max.y) max.y = point.y;
        }

        Vector2 offset = new Vector2((min.x + max.x) / 2f, (min.y + max.y) / 2f);

        for(int i = 0; i < Rotations; i++)
        {
            offset = new Vector2(-offset.y, offset.x);
        }

        return offset;
    }

    public void UpdateAllSprites()
    {
        foreach (RectTransform child in transform)
        {
            child.GetComponent<TurkCubeScript>().UpdateSprite();
        }
    }
    public void ClearAllMat()
    {
        foreach (RectTransform child in transform)
        {
            child.GetComponent<TurkCubeScript>().ClearMat();
        }
    }

    public void ActivateAllMat()
    {
        foreach (RectTransform child in transform)
        {
            child.GetComponent<TurkCubeScript>().ActivateMat();
        }
    }
}
