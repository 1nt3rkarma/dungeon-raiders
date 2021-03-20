using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core.Easing;

public class CameraPathNode : MonoBehaviour
{
    public new Camera camera;

    public float FOV => camera.fieldOfView;

    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;
    public Vector3 Euler => transform.eulerAngles;

    [Header("Transition settings")]
    public bool transitFOV;
}
