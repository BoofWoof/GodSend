using UnityEngine;
using UnityEngine.UI;

public class PieceGeneratorScript : MonoBehaviour
{
    public Texture2D PieceMap;

    public TileSetSO ConstallationTiles;
    public Material ConstMat;

    public Color PieceColor;

    [ContextMenu("GeneratePiece")]
    public void GenerateEmptyFromDefaultTexture()
    {
        GeneratePieceFromTexture(PieceMap);
    }
    public void GeneratePieceFromTexture(Texture2D sourceTexture)
    {
        PuzzleShapeSO shape = new PuzzleShapeSO();
        shape.puzzleTexture = sourceTexture;
        shape.GenerateHoles();
        GeneratePuzzleFromShapeSO(shape);
    }

    public void GeneratePuzzleFromShapeSO(PuzzleShapeSO sourceShapeSO)
    {
        // Instantiate a new square
        GameObject pieceBase = new GameObject($"PieceBase");
        pieceBase.AddComponent<RectTransform>();
        pieceBase.transform.parent = transform;
        pieceBase.transform.localRotation = Quaternion.identity;
        pieceBase.transform.localScale = Vector2.one;
        pieceBase.transform.localPosition = Vector3.zero;

        PieceHolderScript phs = pieceBase.AddComponent<PieceHolderScript>();

        Vector2Int center_cord = Vector2Int.RoundToInt(new Vector2(sourceShapeSO.GetWidth() / 2f, sourceShapeSO.GetHeight() / 2f));
        for (int y = 0; y < sourceShapeSO.GetHeight(); y++)
        {
            for (int x = 0; x < sourceShapeSO.GetWidth(); x++)
            {
                if (sourceShapeSO.IsHole(x, y))
                {
                    continue;
                }

                Vector2Int cord = new Vector2Int(x - center_cord.x, y - center_cord.y);

                // Instantiate a new square
                GameObject newSquare = new GameObject($"PuzzlePiece_{x},{y}");
                newSquare.transform.parent = pieceBase.transform;

                RectTransform rectTransform = newSquare.AddComponent<RectTransform>();
                rectTransform.pivot = new Vector2(0.5f, 0.5f);

                float squareSize = TurkPuzzleScript.squareSize;
                rectTransform.sizeDelta = new Vector2(squareSize, squareSize);

                // Set the position of the square
                newSquare.transform.localRotation = Quaternion.identity;
                newSquare.transform.localScale = Vector2.one;
                newSquare.transform.localPosition = TurkPuzzleScript.GridIdxToPos(cord);

                // Add an Image component and set the sprite
                Image imageComponent = newSquare.AddComponent<Image>();
                imageComponent.sprite = ConstallationTiles.GetSprite(true, true, true, true);
                imageComponent.color = PieceColor;
                imageComponent.material = ConstMat;

                TurkCubeScript turkCubeScript = newSquare.AddComponent<TurkCubeScript>();
                turkCubeScript.cord = cord;
                turkCubeScript.rootPiece = phs;

                phs.Pieces.Add(turkCubeScript);
            }
        }
        foreach (TurkCubeScript piece in phs.Pieces)
        {
            piece.ConnectionCheck();
            piece.UpdateSprite(ConstallationTiles);
        }
    }
}
