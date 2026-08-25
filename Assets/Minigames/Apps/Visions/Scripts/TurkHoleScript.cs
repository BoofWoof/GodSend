using UnityEngine;
using UnityEngine.UI;

public class TurkHoleScript : MonoBehaviour
{
    public string AcceptedNameOverride;

    public Vector2Int cord;
    public TurkCubeScript FilledWith;

    public ParticleSystem PlaceBurst;
    public bool Filled = false;

    public void FillHole(TurkCubeScript filler, bool fullyFilled)
    {
        FilledWith = filler;
        if(fullyFilled) PlaceBurst.Play();
        Image img = FilledWith.GetComponent<Image>();
        img.material = TurkPuzzleScript.instance.ActiveConstMat;
    }

    public void EmptyHole()
    {
        FilledWith = null;
    }

    public bool isFilled()
    {
        Filled = FilledWith != null;
        return Filled;
    }
    public bool isFilledSpecified(string groupName)
    {
        Filled = FilledWith != null;
        if (Filled)
        {
            if (string.IsNullOrEmpty(AcceptedNameOverride))
            {
                Filled = groupName.ToLower() == FilledWith.rootPiece.GroupName.ToLower();
            } else
            {
                Filled = AcceptedNameOverride.ToLower() == FilledWith.rootPiece.GroupName.ToLower();
            }
        }
        return Filled;
    }
}
