using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var level = (Level)target;

        if (GUILayout.Button("Update from child"))
        {
            if (level.rows == null)
                level.rows = new List<Row>();
            else
                level.rows.Clear();

            foreach (Transform child in level.transform)
            {
                var row = child.GetComponent<Row>();
                if (row)
                    level.rows.Add(row);

                row.name = $"Row {level.rows.Count-1}";
            }
        }
    }

}
