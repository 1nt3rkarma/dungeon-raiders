using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Isometrics : MonoBehaviour
{
    public static Isometrics singlton;

    public static float angleFR { get => singlton.angleBorderFR; }
    public static float angleFL { get => singlton.angleBorderFL; }
    public static float angleBR { get => singlton.angleBorderBR; }
    public static float angleBL { get => singlton.angleBorderBL; }

    public new Camera camera;

    public Transform zeroPoint;
    public Transform borderFRPoint;
    public Transform borderFLPoint;
    public Transform borderBRPoint;
    public Transform borderBLPoint;

    public Transform SwipeDownPosition;
    public Transform SwipeUpPosition;
    public Transform SwipeAxisPosition;

    [Space]
    public Vector2 swipeDownPos;
    public Vector2 swipeUpPos;
    public Vector2 swipeAxisPos;
    public float angleSwipeToAxis;

    [Space]
    public Vector2 isoCenterPos;
    public Vector2 isoAxisPos;
    public Vector2 borderFRPos;
    public Vector2 borderFLPos;
    public Vector2 borderBRPos;
    public Vector2 borderBLPos;

    [Space]
    public float angleBorderFR;
    public float angleBorderFL;
    public float angleBorderBR;
    public float angleBorderBL;

    void Awake()
    {
        singlton = this;
    }

    private void Start()
    {
        UpdatePerspective();
    }

    public void UpdatePerspective()
    {
        var axisCenterScreen = camera.WorldToScreenPoint(zeroPoint.position);
        var borderFRScreen = camera.WorldToScreenPoint(borderFRPoint.position);
        var borderFLScreen = camera.WorldToScreenPoint(borderFLPoint.position);
        var borderBRScreen = camera.WorldToScreenPoint(borderBRPoint.position);
        var borderBLScreen = camera.WorldToScreenPoint(borderBLPoint.position);

        var swipeDownScreen = camera.WorldToScreenPoint(SwipeDownPosition.position);
        var swipeUpScreen = camera.WorldToScreenPoint(SwipeUpPosition.position);
        var swipeAxisScreen = camera.WorldToScreenPoint(SwipeAxisPosition.position);

        isoCenterPos = new Vector2(axisCenterScreen.x, axisCenterScreen.y);
        isoAxisPos = isoCenterPos + 10 * Vector2.right;
        borderFRPos = new Vector2(borderFRScreen.x, borderFRScreen.y);
        borderFLPos = new Vector2(borderFLScreen.x, borderFLScreen.y);
        borderBRPos = new Vector2(borderBRScreen.x, borderBRScreen.y);
        borderBLPos = new Vector2(borderBLScreen.x, borderBLScreen.y);

        swipeDownPos = new Vector2(swipeDownScreen.x, swipeDownScreen.y);
        swipeUpPos = new Vector2(swipeUpScreen.x, swipeUpScreen.y);
        swipeAxisPos = new Vector2(swipeAxisScreen.x, swipeAxisScreen.y);

        Vector2 swipeDirection = swipeUpPos - swipeDownPos;
        Vector2 swipeAxisDirection = swipeAxisPos - swipeDownPos;
        angleSwipeToAxis = Vector2.SignedAngle(swipeAxisDirection, swipeDirection);

        Vector2 isoAxisDirection = isoAxisPos - isoCenterPos;
        Vector2 borderFRDirection = borderFRPos - isoCenterPos;
        angleBorderFR = Vector2.SignedAngle(isoAxisDirection, borderFRDirection);

        Vector2 borderFLDirection = borderFLPos - isoCenterPos;
        angleBorderFL = Vector2.SignedAngle(isoAxisDirection, borderFLDirection);

        Vector2 borderBRDirection = borderBRPos - isoCenterPos;
        angleBorderBR = Vector2.SignedAngle(isoAxisDirection, borderBRDirection);

        Vector2 borderBLDirection = borderBLPos - isoCenterPos;
        angleBorderBL = Vector2.SignedAngle(isoAxisDirection, borderBLDirection);
    }

    public static float GetSwipeAngle(Vector2 positionDown, Vector2 positionUp)
    {
        Vector2 axisPos = positionDown + Vector2.right * 100;

        Vector2 direction = positionUp - positionDown;
        Vector2 axisDirection = axisPos - positionDown;

        var angle = Vector2.SignedAngle(axisDirection, direction);
        return angle;
    }

    private void OnDrawGizmosSelected()
    {
        UpdatePerspective();
    }
}
