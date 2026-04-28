using System;
using UnityEngine;
using System.Collections.Generic;

public class WallpaperChangerScript : MonoBehaviour
{
    public static WallpaperChangerScript instance;

    public Material WallpaperTextureMaterial;
    public Material FloorTileMaterial;
    public Material WallTileMaterial;
    public Material GoldMaterial;
    public Material CouchMaterial;
    public Material CeilingMaterial;
    public Material Ceiling2Material;
    public Material WoodMaterial;
    public Material LightMetalMaterial;

    public int DebugWallpaperIndex;

    public List<WallpaperData> WallpaperChoices = new List<WallpaperData>();

    [Serializable]
    public class WallpaperData
    {
        public Texture2D WallpaperTexture;
        public Color FloorTileTint = Color.white;
        public Color WallTileTint = Color.white;
        public Color MetalTint = Color.white;
        public Color CouchTint = Color.red;
        public Color CeilingTint = Color.white;
        public Color Ceiling2Tint = Color.white;
        public Color WoodTint = Color.white;
        public Color LightMetal = Color.white;
    }

    public void Awake()
    {
        WallpaperChangerScript.instance = this;
        SetWallpaperTexture(0);
    }

    [ContextMenu("Change Wallpaper Now")]
    public void DebugWallpaperChange()
    {
        SetWallpaperTexture(DebugWallpaperIndex);
    }

    public void SetWallpaperTexture(int textureIndex)
    {
        WallpaperData targetWallpaper = WallpaperChoices[textureIndex];

        Debug.Log("CHANGING TEXTURE");
        WallpaperTextureMaterial.SetTexture("_BaseMap", targetWallpaper.WallpaperTexture);
        FloorTileMaterial.color = targetWallpaper.FloorTileTint;
        WallTileMaterial.color = targetWallpaper.WallTileTint;
        GoldMaterial.color = targetWallpaper.MetalTint;
        CouchMaterial.color = targetWallpaper.CouchTint;
        CeilingMaterial.color = targetWallpaper.CeilingTint;
        Ceiling2Material.color = targetWallpaper.Ceiling2Tint;
        WoodMaterial.color = targetWallpaper.WoodTint;
        LightMetalMaterial.color = targetWallpaper.LightMetal;
    }
}
