using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnitAnimationHandler))]
public class AnimationHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var animHanlder = (UnitAnimationHandler) target;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Animation Debugging");

        if (GUILayout.Button("Walk"))
            animHanlder.SetMoveFlag(true);
        if (GUILayout.Button("Stop"))
            animHanlder.SetMoveFlag(false);
        if (GUILayout.Button("Attack"))
            animHanlder.PlayAnimation("attack");
    }
}
