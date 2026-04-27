using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClutterScript : MonoBehaviour
{

    public static List<ClutterScript> ClutterScripts = new List<ClutterScript>();
    public bool FirstReveal = true;


    public void OnEnable()
    {
        ClutterScripts.Add(this);
    }

    public void OnDisable()
    {
        ClutterScripts.Remove(this);
    }

    public void AddHighlights()
    {
        int newLayer = LayerMask.NameToLayer("Outline");
        gameObject.layer = newLayer;
        foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.gameObject.layer = newLayer;
        }
    }

    public void RemoveHighlights()
    {
        int newLayer = LayerMask.NameToLayer("Default");
        gameObject.layer = newLayer;
        foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer.gameObject.layer = newLayer;
        }
    }

    public void Reveal()
    {
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(true);
        }
        if (FirstReveal)
        {
            FirstReveal = false;
            int newLayer = LayerMask.NameToLayer("NewOutline");
            gameObject.layer = newLayer;
            foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.gameObject.layer = newLayer;
            }
        }
    }

    public static void RevealAll()
    {
        foreach (ClutterScript script in ClutterScripts)
        {
            script.Reveal();
        }
    }

    public static void HideAll()
    {
        foreach (ClutterScript script in ClutterScripts)
        {
            foreach (Transform t in script.transform)
            {
                t.gameObject.SetActive(true);
            }
        }
    }
}
