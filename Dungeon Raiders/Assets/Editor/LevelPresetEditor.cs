using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelPreset))]
public class LevelPresetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var preset = (LevelPreset)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Update rows"))
        {
            preset.UpdateRows();
        }

        if (GUILayout.Button("Update objects"))
        {
            preset.UpdateObjects();
        }

        GUILayout.EndHorizontal();
    }
}
