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
            if (preset.rows == null)
                preset.rows = new List<Row>();
            else
                preset.rows.Clear();

            foreach (Transform child in preset.transform)
            {
                var row = child.GetComponent<Row>();
                if (row)
                    preset.rows.Add(row);

                row.name = $"Row {preset.rows.Count - 1}";
            }
        }

        if (GUILayout.Button("Align objects"))
        {
            preset.AlignChildren();
        }

        GUILayout.EndHorizontal();
    }
}
