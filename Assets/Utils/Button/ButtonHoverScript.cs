using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonHoverScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent PointerEnterEvent;
    public UnityEvent PointerExitEvent;

    //This is only needed cause trigger devours things.
    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnterEvent?.Invoke();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExitEvent?.Invoke();
    }
}
