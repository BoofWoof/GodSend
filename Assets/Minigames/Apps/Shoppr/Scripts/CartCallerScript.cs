using UnityEngine;

public class CartCallerScript : MonoBehaviour
{
    public void CallCart()
    {
        CartScript.AddTarget(transform);
    }
}
