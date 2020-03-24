using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelSettings))]
public class LevelSettingsEditor : Editor
{
    public bool showPresets;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //var settings = (LevelSettings) target;

        //GUIStyle fontStyleItalic = new GUIStyle();
        //fontStyleItalic.fontStyle = FontStyle.Italic;

        //if (settings.presetsParams == null)
        //    settings.presetsParams = new List<LevelPresetParams>();

        //showPresets = EditorGUILayout.Foldout(showPresets, "Presets");

        //if (showPresets)
        //{
        //    EditorGUI.indentLevel += 1;

        //    foreach (var preset in settings.presetsParams)
        //    {
        //        if (preset.presetPrefab == null)
        //        {
        //            GUILayout.Label("-Level Preset-");

        //            preset.presetPrefab = (LevelPreset)EditorGUILayout.ObjectField(
        //                preset.presetPrefab, typeof(LevelPreset), false);

        //            EditorGUILayout.LabelField("-", fontStyleItalic);

        //            EditorGUI.BeginDisabledGroup(true);

        //            EditorGUILayout.Toggle("Infinite", preset.infinite);

        //            EditorGUILayout.IntField("Times per level", preset.count);

        //            EditorGUILayout.Toggle("Generate items", preset.allowItemsGeneration);

        //            EditorGUI.EndDisabledGroup();
        //        }
        //        else
        //        {
        //            GUILayout.Label(preset.presetPrefab.name);

        //            preset.presetPrefab = (LevelPreset)EditorGUILayout.ObjectField(
        //                preset.presetPrefab, typeof(LevelPreset), false);

        //            EditorGUILayout.LabelField(preset.presetPrefab.description, fontStyleItalic);

        //            preset.infinite = EditorGUILayout.Toggle("Infinite", preset.infinite);

        //            EditorGUI.BeginDisabledGroup(preset.infinite);

        //            int count = EditorGUILayout.IntField("Times per level", preset.count);
        //            preset.count = Mathf.Clamp(count, 1, 10);

        //            EditorGUI.EndDisabledGroup();

        //            preset.allowItemsGeneration = EditorGUILayout.Toggle("Generate items", preset.allowItemsGeneration);
        //        }

        //        GUILayout.Space(10);
        //    }

        //    EditorGUILayout.BeginHorizontal();

        //    if (GUILayout.Button("Add"))
        //    {
        //        settings.presetsParams.Add(new LevelPresetParams());
        //    }

        //    if (settings.presetsParams.Count > 0)
        //    {
        //        if (GUILayout.Button("Remove"))
        //        {
        //            settings.presetsParams.RemoveAt(settings.presetsParams.Count - 1);
        //        }
        //    }

        //    EditorGUILayout.EndHorizontal();

        //    EditorGUI.indentLevel -= 1;
        //}

        //EditorUtility.SetDirty(target);
    }
}
