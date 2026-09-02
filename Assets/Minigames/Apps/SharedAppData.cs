
using UnityEngine;
using UnityEngine.Events;

public class SharedAppData : MonoBehaviour
{
    public static bool _BackLock = false;
    public static bool BackLock
    {
        get { return _BackLock; }
        set {
            Debug.Log($"Changing backlock to: {value}");
            _BackLock = value;
        }
    }
    public static UnityEvent ReplacementBack = new();
    public void Start()
    {
        BackLock = false;
        ReplacementBack = new();
    }
}
