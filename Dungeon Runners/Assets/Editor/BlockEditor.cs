using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Block))]
public class BlockEditor : Editor
{

    public override void OnInspectorGUI()
    {
        var block = (Block)target;

        base.OnInspectorGUI();
        if (!block.isEmpty)
        {
            if (GUILayout.Button("Make Empty"))
                block.MakeEmpty();
        }
        else
        {
            if (GUILayout.Button("Make Solid"))
                block.MakeSolid();
        }

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
