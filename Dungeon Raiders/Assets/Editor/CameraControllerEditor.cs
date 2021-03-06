﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraController))]
public class CameraControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var cameraController = (CameraController)target;

        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("View Deafult"))
        {
            cameraController.cameraMain.transform.position = cameraController.positionDefault;
            cameraController.cameraMain.orthographicSize = cameraController.sizeDefault;
        }

        if (GUILayout.Button("Set Deafult"))
        {
            cameraController.positionDefault = cameraController.cameraMain.transform.position;
            cameraController.sizeDefault = cameraController.cameraMain.orthographicSize;

        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("View Hero"))
        {
            cameraController.cameraMain.transform.position = cameraController.positionFocusHero;
            cameraController.cameraMain.orthographicSize = cameraController.sizeFocusHero;
        }

        if (GUILayout.Button("Set Hero"))
        {
            cameraController.positionFocusHero = cameraController.cameraMain.transform.position;
            cameraController.sizeFocusHero = cameraController.cameraMain.orthographicSize;

        }

        GUILayout.EndHorizontal();
    }
}
