using System.Collections;
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
            cameraController.camera.transform.position = cameraController.positionDefault;
            cameraController.camera.orthographicSize = cameraController.sizeDefault;
        }

        if (GUILayout.Button("Set Deafult"))
        {
            cameraController.positionDefault = cameraController.camera.transform.position;
            cameraController.sizeDefault = cameraController.camera.orthographicSize;

        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("View Hero"))
        {
            cameraController.camera.transform.position = cameraController.positionFocusHero;
            cameraController.camera.orthographicSize = cameraController.sizeFocusHero;
        }

        if (GUILayout.Button("Set Hero"))
        {
            cameraController.positionFocusHero = cameraController.camera.transform.position;
            cameraController.sizeFocusHero = cameraController.camera.orthographicSize;

        }

        GUILayout.EndHorizontal();
    }
}
