using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasHelper
{
    public static Canvas GetHoveredCanvas(Vector2 screenPosition)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            Canvas canvas = result.gameObject.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                return canvas;
            }
        }

        return null;
    }

}
