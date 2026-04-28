using UnityEngine;

public class WallpaperTriggerScript : MonoBehaviour
{
    public void UpdateWallpaper(int index)
    {
        WallpaperChangerScript.instance.SetWallpaperTexture(index);
    }
}
