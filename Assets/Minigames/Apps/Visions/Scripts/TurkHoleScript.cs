using UnityEngine;
using UnityEngine.UI;

public class TurkHoleScript : MonoBehaviour
{
    public Vector2Int cord;
    public TurkCubeScript filledWith;

    public ParticleSystem PlaceBurst;
    public bool Filled = false;

    public void FillHole(TurkCubeScript filler, bool fullyFilled)
    {
        filledWith = filler;
        if(fullyFilled) PlaceBurst.Play();
    }

    public void EmptyHole()
    {
        filledWith = null;
    }

    public bool isFilled()
    {
        Filled = filledWith != null;
        if (Filled) {
            Image img = filledWith.GetComponent<Image>();
            img.material = TurkPuzzleScript.instance.ActiveConstMat;
        } 
        return Filled;
    }
}
