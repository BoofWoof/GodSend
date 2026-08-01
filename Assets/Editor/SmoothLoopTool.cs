using UnityEngine;
using UnityEditor;

public class SmoothLoopTool : EditorWindow
{
    [MenuItem("Window/Smooth Loop Tool")]
    public static void ShowWindow() => GetWindow<SmoothLoopTool>("Loop Smooth");

    void OnGUI()
    {
        if (GUILayout.Button("Smooth Selected Animation Clips", GUILayout.Height(40)))
        {
            foreach (Object obj in Selection.objects)
            {
                if (obj is AnimationClip clip)
                {
                    SmoothClip(clip);
                }
            }
        }
    }

    void SmoothClip(AnimationClip clip)
    {
        Undo.RecordObject(clip, "Smooth Loop Tangents");
        EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clip);

        foreach (var binding in bindings)
        {
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
            if (curve == null || curve.keys.Length < 2) continue;

            Keyframe[] keys = curve.keys;
            int lastIndex = keys.Length - 1;

            // 1. Force identical start and end values
            keys[lastIndex].value = keys[0].value;

            // 2. Calculate the average slope of start and end
            float avgTangent = (keys[0].outTangent + keys[lastIndex].inTangent) / 2f;

            // 3. Apply the average tangent to create a seamless transition
            keys[0].outTangent = avgTangent;
            keys[lastIndex].inTangent = avgTangent;

            // Update the curve with the new keys
            curve.keys = keys;
            AnimationUtility.SetEditorCurve(clip, binding, curve);
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Successfully smoothed loop for: {clip.name}");
    }
}

