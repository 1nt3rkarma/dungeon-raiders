using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController singlton;

    public Camera camera;

    public Vector3 positionDefault;
    public float sizeDefault;

    public Vector3 positionFocusHero;
    public float sizeFocusHero;

    private void Awake()
    {
        singlton = this;
    }

    public static void FocusHero()
    {
        FocusHero(0);
    }

    public static void FocusHero(float overTime)
    {
        singlton.StopAllCoroutines();

        if (overTime > 0)
        {
            singlton.StartCoroutine(singlton.SetCameraSizeOverTimeRoutine(overTime, singlton.sizeFocusHero));
            singlton.StartCoroutine(singlton.MoveCameraOverTimeRoutine(overTime, singlton.positionFocusHero));
        }
        else
        {
            singlton.camera.orthographicSize = singlton.sizeFocusHero;
            singlton.camera.transform.position = singlton.positionFocusHero;
        }
    }
    
    public static void ResetCamera()
    {
        ResetCamera(0);
    }

    public static void ResetCamera(float overTime)
    {
        singlton.StopAllCoroutines();

        if (overTime > 0)
        {
            singlton.StartCoroutine(singlton.SetCameraSizeOverTimeRoutine(overTime, singlton.sizeDefault));
            singlton.StartCoroutine(singlton.MoveCameraOverTimeRoutine(overTime, singlton.positionDefault));
        }
        else
        {
            singlton.camera.orthographicSize = singlton.sizeDefault;
            singlton.camera.transform.position = singlton.positionDefault;
        }
    }

    IEnumerator MoveCameraOverTimeRoutine(float time, Vector3 point)
    {
        var speed = Vector3.Magnitude(point - camera.transform.position) / time;

        while (Vector3.Distance(camera.transform.position, point) > 0.05)
        {
            camera.transform.position = Vector3.Lerp(camera.transform.position, point, speed * Time.deltaTime);
            yield return null;
        }
        camera.transform.position = point;
    }

    IEnumerator SetCameraSizeOverTimeRoutine(float time, float size)
    {
        var delta = Mathf.Abs(camera.orthographicSize - size);
        var speed = delta / time;

        while (delta > 0.05)
        {
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, size, speed * Time.deltaTime);
            delta = Mathf.Abs(camera.orthographicSize - size);
            yield return null;
        }
        camera.orthographicSize = size;
    }

}
