using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PieceHolderScript : MonoBehaviour
{
    public string GroupName;

    public TurkCubeScript SeedPiece;
    public List<TurkCubeScript> Pieces = new List<TurkCubeScript>();

    public bool FirstRelease = true;
    public bool EverReleased = false;

    public int Rotations = 0;
    public int PreviousValidRotations = 0;

    public bool isStored = false;

    public static bool PickupEnabled = true;
    public static bool RotationEnabled = false;
    public static bool PieceHolderRestraint = true;
    public static bool StorePiece = false;

    private bool isDragging;
    private Vector2 offset;

    public static List<PieceHolderScript> PieceList = new List<PieceHolderScript>();

    public bool FullyFilled = false;

    private GameObject Shadow;

    public Color OriginalColor;

    public Vector3 LastKnownGoodPosition;

    public bool LockPiece = false;

    public UnityEvent OnSuccessfulPlacement;

    public void Awake()
    {
        PickupEnabled = true;
        PieceHolderRestraint = true;
        StorePiece = false;

        TurkPuzzleScript.instance.OnBeforePuzzleGenerate.AddListener(DestroySelf);
    }

    public void UpdateColors(Color newColor)
    {
        foreach (TurkCubeScript piece in Pieces)
        {
            piece.GetComponent<Image>().color = newColor;
        }
    }

    public void GetCurrentColor()
    {
        OriginalColor = Pieces[0].GetComponent<Image>().color;
    }

    public void Start()
    {
        PieceList.Add(this);
        StartCoroutine(DelayedPlaceDown());
    }

    public IEnumerator DelayedPlaceDown()
    {
        yield return null;
        yield return null; Vector2 startPos = GetComponent<RectTransform>().anchoredPosition;
        Vector2Int gridIdx = TurkPuzzleScript.PosToGridIdx(startPos);
        Vector2 pos = TurkPuzzleScript.GridIdxToPos(gridIdx);
        GetComponent<RectTransform>().anchoredPosition = pos;

        UpdateCord();
    }

    public void OnDestroy()
    {
        PieceList.Remove(this);
        TurkPuzzleScript.instance.OnBeforePuzzleGenerate.RemoveListener(DestroySelf);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public static void ClearPieces()
    {

        PieceList.Clear();
    }

    public static void SafetyCheckAllPositions()
    {
        foreach(PieceHolderScript piece in PieceList)
        {
            if (piece.isStored) continue;
            if (!piece.PieceInValidZone()) piece.SendToPieceHolder();
        }
    }

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

    public void AddFakeSquares()
    {
        foreach (TurkCubeScript piece in Pieces)
        {
            TurkPuzzleScript.puzzlePieceSquares.Add(piece.gameObject);
        }
    }

    public bool UpdateCord()
    {

        //Checks if we can.
        foreach (TurkCubeScript puzzlePiece in Pieces)
        {
            RectTransform puzzleTransform = puzzlePiece.GetComponent<RectTransform>();

            Vector3 adjustedPiecePosition = TurkPuzzleScript.instance.transform.InverseTransformPoint(puzzleTransform.position);
            Vector2Int newCord = TurkPuzzleScript.PosToGridIdx((Vector2)adjustedPiecePosition);

            if (puzzlePiece.cord == newCord) return false;

            if (TurkPuzzleScript.IsCordTaken(newCord, Pieces)) return false;

            //Vector2 newPos = TurkPuzzleScript.GridIdxToPos(new Vector2Int(newCord.x, newCord.y));
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
            
            if (VisionEmptyGroup.IsEmptyInAnyLookup(oldCord)) VisionEmptyGroup.HoleLookupAny(oldCord).EmptyHole();
            fillers.Add(puzzlePiece.GetComponent<TurkCubeScript>());
        }
        FillHoles();
        return true;
    }

    public void FillHoles()
    {
        FullyFilled = true;
        foreach (TurkCubeScript filler in Pieces)
        {
            Vector2Int cord = filler.cord;
            if (!VisionEmptyGroup.IsEmptyInAnyLookup(cord))
            {
                FullyFilled = false;
            }
        }
        foreach (TurkCubeScript filler in Pieces)
        {
            Vector2Int cord = filler.cord;
            if (VisionEmptyGroup.IsEmptyInAnyLookup(cord))
            {
                VisionEmptyGroup.HoleLookupAny(cord).FillHole(filler, FullyFilled);
            }
        }
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
        DeleteShadow();

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

            Vector3 rawPos = (Vector3)(mousePos + offset); 
            
            rawPos.x = Mathf.Round(rawPos.x);
            rawPos.y = Mathf.Round(rawPos.y);

            Vector2Int newCord = TurkPuzzleScript.PosToGridIdx((Vector2)rawPos);

            Vector2 newPos = TurkPuzzleScript.GridIdxToPos(new Vector2Int(newCord.x, newCord.y));


            transform.localPosition = rawPos;
            Shadow.transform.SetParent(transform.parent);
            Shadow.transform.localPosition = newPos;
            Shadow.transform.SetParent(transform);
            Shadow.transform.SetAsFirstSibling();
        }
    }

    public void RotatePiece(InputAction.CallbackContext c)
    {
        if (!isDragging) return;
        if (!RotationEnabled) return;

        TurkPuzzleScript.instance.PlayRotateSound();

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

        CreateShadow();
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
        if (LockPiece) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!PickupEnabled) return;

        FullyFilled = false;

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

        CreateShadow();

        transform.SetAsLastSibling();
    }

    public void CreateShadow()
    {
        DeleteShadow();
        Shadow = Instantiate(gameObject, transform);
        Destroy(Shadow.GetComponent<PieceHolderScript>());
        TurkCubeScript[] ShadowScripts = Shadow.GetComponentsInChildren<TurkCubeScript>();
        foreach (TurkCubeScript script in ShadowScripts)
        {
            Destroy(script);
        }
        Shadow.transform.SetAsFirstSibling();
        Shadow.transform.localRotation = Quaternion.identity;
        SetAllDark();
    }

    public void DeleteShadow()
    {
        if (Shadow != null)
        {
            Shadow.transform.SetParent(null);
            Destroy(Shadow);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (LockPiece) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!PickupEnabled || !isDragging) return;

        DeleteShadow();

        isDragging = false;
        TurkPuzzleScript.instance.Pickup.Stop();

        if (StorePiece)
        {
            TurkPuzzleScript.instance.Drop.Play();
            SendToPieceHolder(true);
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
            EverReleased = true;
            LastKnownGoodPosition = transform.localPosition;
            if (FullyFilled)
            {
                int Filled = -1;
                foreach(PieceHolderScript piece in PieceList)
                {
                    if (piece.FullyFilled) Filled++;
                }
                Debug.Log(Filled);
                Debug.Log(PieceList.Count);
                //TurkPuzzleScript.instance.DropGood.pitch = Mathf.Lerp(1f, 2f, ((float)Filled)/PieceList.Count);
                TurkPuzzleScript.instance.PlayGoodDropAudio(Filled);
            } else
            {
                TurkPuzzleScript.instance.Drop.Play();
            }
            PreviousValidRotations = Rotations;

            OnSuccessfulPlacement?.Invoke();
        }
        else
        {
            TurkPuzzleScript.instance.DropBad.Play();
            int RestoreRotations = PreviousValidRotations - Rotations;
            if (RestoreRotations < 0) RestoreRotations += 4;
            for(int i = 0; i < RestoreRotations; i++)
            {
                CenterRotate();
            }
            transform.localPosition = LastKnownGoodPosition;
            TurkPuzzleScript.CheckWin();
            return;
        }

        TurkPuzzleScript.CheckWin();
    }

    public void SendToPieceHolder(bool PlaySounds = false)
    {
        DeleteShadow();
        ClearAllMat();

        if (!FirstRelease)
        {
            foreach (TurkCubeScript puzzlePiece in Pieces)
            {
                Vector2Int oldCord = puzzlePiece.GetComponent<TurkCubeScript>().cord;
                
                if (VisionEmptyGroup.IsEmptyInAnyLookup(oldCord)) VisionEmptyGroup.HoleLookupAny(oldCord).EmptyHole();
                puzzlePiece.GetComponent<TurkCubeScript>().cord = new Vector2Int(-99, -99);
            }
        }
        PreviousValidRotations = Rotations;

        FirstRelease = true;
        TruePieceHolderScript.instance.StorePiece(this, PlaySounds);
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

        offset.x = Mathf.Round(offset.x);
        offset.y = Mathf.Round(offset.y);

        return offset;
    }

    public void UpdateAllSprites()
    {
        foreach (RectTransform child in transform)
        {
            TurkCubeScript tS = child.GetComponent<TurkCubeScript>();
            if (tS == null) continue;
            tS.UpdateSprite();
        }
    }
    public void ClearAllMat()
    {
        foreach (RectTransform child in transform)
        {
            TurkCubeScript tS = child.GetComponent<TurkCubeScript>();
            if (tS == null) continue;
            tS.ClearMat();
        }
    }

    public void ActivateAllMat()
    {
        foreach (RectTransform child in transform)
        {
            TurkCubeScript tS = child.GetComponent<TurkCubeScript>();
            if (tS == null) continue;
            tS.ActivateMat();
        }
    }

    public void SetAllDark()
    {
        foreach (RectTransform child in Shadow.transform)
        {
            child.GetComponent<TurkCubeScript>().SetDark();
        }
    }
}
