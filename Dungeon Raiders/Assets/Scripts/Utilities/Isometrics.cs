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
    public float angleBorderBL;
    public float angleBorderBR;

    [Header("Debugging")]

    public bool isDebugging;

    public float minPixelDelta = 1.26f;

    public Transform SwipeDownPosition;
    public Transform SwipeUpPosition;

    [Space]
    public Vector2 swipeDownPos;
    public Vector2 swipeUpPos;
    public Vector2 swipeAxisPos;
    public float angleSwipeToAxis;
    public SwipeDirections direction;

    void Awake()
    {
        singlton = this;
    }

    private void Start()
    {
        UpdatePerspective();
    }

    private void Update()
    {
        if (isDebugging)
            DebugUpdate();
    }

    public void UpdatePerspective()
    {
        var axisCenterScreen = camera.WorldToScreenPoint(zeroPoint.position);
        var borderFRScreen = camera.WorldToScreenPoint(borderFRPoint.position);
        var borderFLScreen = camera.WorldToScreenPoint(borderFLPoint.position);
        var borderBRScreen = camera.WorldToScreenPoint(borderBRPoint.position);
        var borderBLScreen = camera.WorldToScreenPoint(borderBLPoint.position);

        isoCenterPos = new Vector2(axisCenterScreen.x, axisCenterScreen.y);
        isoAxisPos = isoCenterPos + 10 * Vector2.right;
        borderFRPos = new Vector2(borderFRScreen.x, borderFRScreen.y);
        borderFLPos = new Vector2(borderFLScreen.x, borderFLScreen.y);
        borderBRPos = new Vector2(borderBRScreen.x, borderBRScreen.y);
        borderBLPos = new Vector2(borderBLScreen.x, borderBLScreen.y);

        Vector2 isoAxisDirection = isoAxisPos - isoCenterPos;
        Vector2 borderFRDirection = borderFRPos - isoCenterPos;
        angleBorderFR = Vector2.SignedAngle(isoAxisDirection, borderFRDirection);
        angleBorderFR = MathUtils.SignedTo360(angleBorderFR);

        Vector2 borderFLDirection = borderFLPos - isoCenterPos;
        angleBorderFL = Vector2.SignedAngle(isoAxisDirection, borderFLDirection);
        angleBorderFL = MathUtils.SignedTo360(angleBorderFL);

        Vector2 borderBRDirection = borderBRPos - isoCenterPos;
        angleBorderBR = Vector2.SignedAngle(isoAxisDirection, borderBRDirection);
        angleBorderBR = MathUtils.SignedTo360(angleBorderBR);

        Vector2 borderBLDirection = borderBLPos - isoCenterPos;
        angleBorderBL = Vector2.SignedAngle(isoAxisDirection, borderBLDirection);
        angleBorderBL = MathUtils.SignedTo360(angleBorderBL);
    }

    public void DebugUpdate()
    {
        var swipeDownScreen = camera.WorldToScreenPoint(SwipeDownPosition.position);
        var swipeUpScreen = camera.WorldToScreenPoint(SwipeUpPosition.position);

        swipeDownPos = new Vector2(swipeDownScreen.x, swipeDownScreen.y);
        swipeUpPos = new Vector2(swipeUpScreen.x, swipeUpScreen.y);
        swipeAxisPos = new Vector2(swipeDownScreen.x + 10, swipeDownScreen.y);

        Vector2 swipeDirection = swipeUpPos - swipeDownPos;
        Vector2 swipeAxisDirection = swipeAxisPos - swipeDownPos;
        angleSwipeToAxis = Vector2.SignedAngle(swipeAxisDirection, swipeDirection);
        angleSwipeToAxis = MathUtils.SignedTo360(angleSwipeToAxis);

        direction = UserTouchController.GetSwipeDirectionIsometric(swipeAxisDirection, swipeDirection, minPixelDelta);
    }

    public static float GetSwipeAngle(Vector2 positionDown, Vector2 positionUp, bool signed = false)
    {
        Vector2 axisPos = positionDown + Vector2.right * 100;

        Vector2 direction = positionUp - positionDown;
        Vector2 axisDirection = axisPos - positionDown;

        var angle = Vector2.SignedAngle(axisDirection, direction);
        if (signed)
            return angle;
        else
            return MathUtils.SignedTo360(angle);

    }

    private void OnDrawGizmosSelected()
    {
        UpdatePerspective();
    }
}
