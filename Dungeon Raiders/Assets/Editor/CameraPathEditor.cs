using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraPath))]
public class CameraPathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var path = (CameraPath)target;

        if (GUILayout.Button("Update form children"))
        {
            var nodes = new List<CameraPathNode>();
            foreach (Transform child in path.transform)
            {
                var node = child.GetComponent<CameraPathNode>();
                if (node != null)
                    nodes.Add(node);
            }


            path.UpdateSegments(nodes);
        }
    }
}
