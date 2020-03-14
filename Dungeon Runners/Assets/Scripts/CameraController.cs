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

        var lineShift = Vector3.right * (Player.singlton.hero.line - 1);
        var point = singlton.positionFocusHero + lineShift;
        var size = singlton.sizeFocusHero;

        if (overTime > 0)
        {
            singlton.StartCoroutine(singlton.MoveCameraOverTimeLerpRoutine(overTime, point));
            singlton.StartCoroutine(singlton.SetCameraSizeOverTimeLerpRoutine(overTime, size));
        }
        else
        {
            singlton.camera.transform.position = point;
            singlton.camera.orthographicSize = size;
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
            singlton.camera.transform.position = point;
            singlton.camera.orthographicSize = size;
        }
    }

    IEnumerator MoveCameraOverTimeLerpRoutine(float time, Vector3 point)
    {
        var timer = 0f;
        var speed = Vector3.Magnitude(point - camera.transform.position) / time;

        while (Vector3.Distance(camera.transform.position, point) > 0.05f)
        {
            timer += Time.deltaTime;
            camera.transform.position = Vector3.Lerp(camera.transform.position, point, speed * Time.deltaTime);
            yield return null;
        }
        camera.transform.position = point;
        //Debug.Log($"Заданное время: {time}");
        //Debug.Log($"Исполнение заняло: {timer}");
    }

    IEnumerator SetCameraSizeOverTimeLerpRoutine(float time, float size)
    {
        var delta = Mathf.Abs(camera.orthographicSize - size);
        var speed = delta / time;

        while (delta > 0.1)
        {
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, size, speed * Time.deltaTime);
            delta = Mathf.Abs(camera.orthographicSize - size);
            yield return null;
        }
        camera.orthographicSize = size;
    }

}
