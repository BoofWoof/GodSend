using PixelCrushers;
using System;
using UnityEngine;

public class SetInitialPoster : Saver
{
    public int InitialPosterIdx = 0;
    public string PosterName;

    [Serializable]
    public class PosterSave
    {
        public string PosterName;
    }
    public override string RecordData()
    {
        PosterSave saveData = new PosterSave();

        saveData.PosterName = PosterName;

        return SaveSystem.Serialize(saveData);
    }
    public override void ApplyData(string s)
    {
        PosterSave saveData = SaveSystem.Deserialize<PosterSave>(s);

        if (saveData == null) return;

        SetPoster(saveData.PosterName);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetPoster(InitialPosterIdx);
    }

    public void SetPoster(string posterName)
    {
        PosterName = posterName;
        GetComponent<MeshRenderer>().material.SetTexture("_MainTex", UnlockablesManager.PostersDict[posterName].Image.texture);
    }
    public void SetPoster(OfficePoster poster)
    {
        SetPoster(poster.Name);
    }
    public void SetPoster(int posterIdx)
    {
        SetPoster(UnlockablesManager.PostersList[posterIdx].Name);
    }
}
