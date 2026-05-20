using PixelCrushers.DialogueSystem;
using UnityEngine;

public class BalconyButtonScript : MonoBehaviour
{

    public ActivatableObjectScript LeftButton;
    public ActivatableObjectScript RightButton;

    public void Start()
    {
        LeftButton.enabled = false;
        RightButton.enabled = false;

        Lua.RegisterFunction("StartBalconyButtons", this, SymbolExtensions.GetMethodInfo(() => StartBalconyButtons()));
    }

    public void StartBalconyButtons()
    {
        GetComponent<Animator>().Play("RaiseTurners");

        LeftButton.enabled = true;
        RightButton.enabled = true;
    }
}
