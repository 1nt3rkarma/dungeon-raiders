using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ResourceBar))]
public class ResourceBarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var bar = (ResourceBar)target;

        if (GUI.changed)
        {
            bar.SetValue(bar.value);
        }
    }
}
