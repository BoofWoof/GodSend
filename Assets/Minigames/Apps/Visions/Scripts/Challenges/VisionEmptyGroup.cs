using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisionEmptyGroup : MonoBehaviour
{
    public Texture2D DefaultTexture;
    public GameObject PuzzleHolePrefab;

    //Any group is accepted by default.
    public string AcceptedGroupName;
    public bool RequireAllHaveSameGroupName;

    public List<TurkHoleScript> Holes = new();
    public Dictionary<Vector2Int, TurkHoleScript> HoleLookupDict = new();

    private static List<VisionEmptyGroup> EmptyGroups = new();

    [ContextMenu("RefindPieces")]
    public void RefindPieces()
    {
        Holes.Clear();
        HoleLookupDict = new();

        foreach (Transform child in transform)
        {
            TurkHoleScript childScript = child.GetComponent<TurkHoleScript>();
            Debug.Log(childScript);
            if (childScript == null) continue;
            Holes.Add(childScript);
            AddEmptyToLookup(childScript, childScript.cord);
        }
    }

    public void OnEnable()
    {
        EmptyGroups.Add(this);
    }

    public void OnDisable()
    {
        EmptyGroups.Remove(this);
    }

    [ContextMenu("GeneratePiece")]
    public void GenerateEmptyFromDefaultTexture()
    {
        GenerateEmptyFromTexture(DefaultTexture);
    }
    public void GenerateEmptyFromTexture(Texture2D sourceTexture)
    {
        PuzzleShapeSO shape = new PuzzleShapeSO();
        shape.puzzleTexture = sourceTexture;
        shape.GenerateHoles();
        GenerateEmptyFromShapeSO(shape);
    }
    public void GenerateEmptyFromShapeSO(PuzzleShapeSO sourceShapeSO)
    {
        ClearPieces();

        Vector2Int center_cord = Vector2Int.RoundToInt(new Vector2(sourceShapeSO.GetWidth() / 2f, sourceShapeSO.GetHeight() / 2f));
        for (int y = 0; y < sourceShapeSO.GetHeight(); y++)
        {
            for (int x = 0; x < sourceShapeSO.GetWidth(); x++)
            {
                // Skip hole positions
                if (sourceShapeSO.IsHole(x, y))
                {
                    continue;
                }

                Vector2Int cord = new Vector2Int(x - center_cord.x, y - center_cord.y);

                // Instantiate a new square
                GameObject newSquare = Instantiate(PuzzleHolePrefab);
                newSquare.transform.parent = transform;

                float squareSize = TurkPuzzleScript.squareSize;
                RectTransform rectTransform = newSquare.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(squareSize, squareSize);

                // Set the position of the square
                newSquare.transform.localRotation = Quaternion.identity;
                newSquare.transform.localScale = Vector3.one;
                rectTransform.localPosition = TurkPuzzleScript.GridIdxToPos(cord);

                // Add an Image component and set the sprite
                Image imageComponent = newSquare.GetComponent<Image>();
                imageComponent.color = new Color(255, 255, 255, 1f);

                TurkHoleScript turkCubeScript = newSquare.GetComponent<TurkHoleScript>();
                turkCubeScript.cord = cord;

                Holes.Add(turkCubeScript);
                AddEmptyToLookup(turkCubeScript, cord);
            }
        }
    }

    [ContextMenu("ClearPieces")]
    public void ClearPieces()
    {
        // Clear existing squares if any
        foreach (TurkHoleScript hole in Holes)
        {
#if UNITY_EDITOR
            DestroyImmediate(hole.gameObject);
#else
            Destroy(hole.gameObject);
#endif
        }
        Holes.Clear();
        HoleLookupDict = new();
    }

    public void AddEmptyToLookup(TurkHoleScript piece, Vector2Int cord)
    {
        HoleLookupDict[cord] = piece;
    }

    public TurkHoleScript HoleLookup(Vector2Int cord)
    {
        if (!HoleLookupDict.ContainsKey(cord)) return null;

        return HoleLookupDict[cord];
    }

    public static TurkHoleScript HoleLookupAny(Vector2Int cord)
    {
        foreach (VisionEmptyGroup group in EmptyGroups)
        {
            TurkHoleScript holeScript = group.HoleLookup(cord);
            if (holeScript != null)
            {
                return holeScript;
            }
        }
        return null;
    }

    public static int GetTotalEmpties()
    {
        int EmptyCount = 0;
        foreach (VisionEmptyGroup group in EmptyGroups)
        {
            EmptyCount += group.Holes.Count;
        }
        return EmptyCount;
    }

    public static List<TurkHoleScript> GetAllHoles(){
        List<TurkHoleScript> fullList = new();

        foreach (VisionEmptyGroup group in EmptyGroups)
        {
            fullList.AddRange(group.Holes);
        }
        return fullList;
    }

    public bool IsEmptyInLookup(Vector2Int cord)
    {
        return HoleLookupDict.ContainsKey(cord);
    }

    public static bool IsEmptyInAnyLookup(Vector2Int cord)
    {
        foreach (VisionEmptyGroup group in EmptyGroups)
        {
            if (group.IsEmptyInLookup(cord))
            {
                return true;
            }
        }
        return false;
    }

    [ContextMenu("LockPieceToGrid")]
    public void ReLockToGrid()
    {
        HoleLookupDict = new();

        foreach (TurkHoleScript hole in Holes)
        {
            Vector2 localPosition = hole.transform.localPosition;
            Vector2Int newPosIdx = TurkPuzzleScript.PosToGridIdx(localPosition);
            hole.transform.localPosition = TurkPuzzleScript.GridIdxToPos(newPosIdx);
            AddEmptyToLookup(hole, newPosIdx);
            hole.cord = newPosIdx;
        }
    }

    public bool CheckForWin()
    {
        if (RequireAllHaveSameGroupName)
        {
            if (!Holes[0].isFilled()) return false;
            AcceptedGroupName = Holes[0].FilledWith.rootPiece.GroupName;
        }

        foreach (TurkHoleScript hole in Holes)
        {
            if (string.IsNullOrEmpty(AcceptedGroupName))
            {
                if (!hole.isFilled())
                {
                    return false;
                }
            } else
            {
                if (!hole.isFilledSpecified(AcceptedGroupName))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static bool CheckForAnyWin()
    {
        Debug.Log(EmptyGroups.Count);

        foreach(VisionEmptyGroup group in EmptyGroups)
        {
            if(group.CheckForWin())
            {
                return true;
            }
        }
        return false;
    }

    public static void DestroyAllEmptyGroups()
    {
        for (int i = EmptyGroups.Count - 1; i >= 0; i--)
        {
            Destroy(EmptyGroups[i].gameObject);
        }
    }
}
