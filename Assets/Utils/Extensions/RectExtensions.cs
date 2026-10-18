using UnityEngine;

public static class RectExtensions
{
    public static float EqualSpacingX(this RectTransform input, int divisions)
    {
        return input.sizeDelta.x / divisions;
    }
    public static float EqualSpacingY(this RectTransform input, int divisions)
    {
        return input.sizeDelta.y / divisions;
    }
}
