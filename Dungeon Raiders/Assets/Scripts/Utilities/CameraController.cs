using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController singlton;

    public Camera cameraMain;
    public Camera cameraMainRender;
    public Camera cameraIntro;

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

        var lineShift = Vector3.right * (Hero.singlton.line - 1);
        var point = singlton.positionFocusHero + lineShift;
        var size = singlton.sizeFocusHero;

        if (overTime > 0)
        {
            singlton.StartCoroutine(singlton.MoveCameraOverTimeLerpRoutine(overTime, point));
            singlton.StartCoroutine(singlton.SetCameraSizeOverTimeLerpRoutine(overTime, size));
        }
        else
        {
            singlton.cameraMain.transform.position = point;
            singlton.cameraMain.orthographicSize = size;
        }
    }
    
    public static void ResetCamera()
    {
        ResetCamera(0);
    }

    public static void ResetCamera(float overTime)
    {
        singlton.StopAllCoroutines();

        var point = singlton.positionDefault;
        var size = singlton.sizeDefault;

        if (overTime > 0)
        {
            singlton.StartCoroutine(singlton.MoveCameraOverTimeLerpRoutine(overTime, point));
            singlton.StartCoroutine(singlton.SetCameraSizeOverTimeLerpRoutine(overTime, size));
        }
        else
        {
            singlton.cameraMain.transform.position = point;
            singlton.cameraMain.orthographicSize = size;
        }
    }

    IEnumerator MoveCameraOverTimeLerpRoutine(float time, Vector3 point)
    {
        var timer = 0f;
        var speed = Vector3.Magnitude(point - cameraMain.transform.position) / time;

        while (Vector3.Distance(cameraMain.transform.position, point) > 0.05f)
        {
            timer += Time.deltaTime;
            cameraMain.transform.position = Vector3.Lerp(cameraMain.transform.position, point, speed * Time.deltaTime);
            yield return null;
        }
        cameraMain.transform.position = point;
    }

    IEnumerator SetCameraSizeOverTimeLerpRoutine(float time, float size)
    {
        var delta = Mathf.Abs(cameraMain.orthographicSize - size);
        var speed = delta / time;

        while (delta > 0.1)
        {
            cameraMain.orthographicSize = Mathf.Lerp(cameraMain.orthographicSize, size, speed * Time.deltaTime);
            delta = Mathf.Abs(cameraMain.orthographicSize - size);
            yield return null;
        }
        cameraMain.orthographicSize = size;
    }

}
