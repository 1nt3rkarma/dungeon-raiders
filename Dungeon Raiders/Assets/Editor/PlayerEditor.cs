using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var player = (Player)target;

        if (GUILayout.Button("Clear save"))
        {
            PlayerData.Clear();
        }
    }

}
