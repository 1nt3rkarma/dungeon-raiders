using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserMobileController : UserController
{
    public static bool IsoMode = true;

    public float doubleTapThreshold = 0.1f;
    private float doubleTapTimer = 0;

    [Tooltip("Minimum distance of the swipe to count this action")]
    public float swipeSensitivity = 0.1f;

    private float maxPixelDelta;
    private Vector2 touchPosDown;
    private Vector2 touchPosUp;

    public new Camera camera;

    private void Start()
    {
        maxPixelDelta = camera.pixelWidth / 2f;
    }

    private void Update()
    {
        if (!Player.controllEnabled)
            return;

        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchPosDown = touch.position;
                touchPosUp = touch.position;

                CatchTapPress();
            }

            if (touch.phase == TouchPhase.Ended)
            {
                touchPosUp = touch.position;

                CatchTapRelease();
            }
        }

        if (tapPress1 && tapRelease1)
            doubleTapTimer += Time.deltaTime;
        else
            doubleTapTimer = 0;

        if (tapPress1 && !tapRelease1)
            tapHoldTimer += Time.deltaTime;
        else
            tapHoldTimer = 0;

        if (tapHoldTimer >= tapHoldThreshold)
        {
            if (tapPress1 && !tapRelease1)
                CatchTapHold();
        }
        else
        {
            if (tapPress1 && tapRelease1)
            {
                SwipeDirections direction;
                if (IsoMode)
                    direction = GetSwipeDirectionIsometric(touchPosDown, touchPosUp);
                else
                    direction = GetSwipeDirection(touchPosDown, touchPosUp);

                if (direction != SwipeDirections.None)
                    CatchSwipe(direction);
                else
                    CatchSingleTap();
            }
            else if (tapPress1 && !tapRelease1)
            {
                SwipeDirections direction;
                if (IsoMode)
                    direction = GetSwipeDirectionIsometric(touchPosDown, touchPosUp);
                else
                    direction = GetSwipeDirection(touchPosDown, touchPosUp);

                if (direction != SwipeDirections.None)
                    CatchSwipe(direction);
            }
        }
    }

    SwipeDirections GetSwipeDirection(Vector2 positionDown, Vector2 positionUp)
    {
        DebugMobile.PrintLabel1($"From {(int)positionDown.x}, {(int)positionDown.y} to {(int)positionUp.x}, {(int)positionUp.y}");
        var deltaVertical = Mathf.Abs(positionUp.y - positionDown.y);
        var deltaHorizontal = Mathf.Abs(positionUp.x - positionDown.x);

        if (deltaVertical < maxPixelDelta * swipeSensitivity
            && deltaHorizontal < maxPixelDelta * swipeSensitivity)
        {
            return SwipeDirections.None;
        }

        if (Mathf.Abs(deltaVertical) > Mathf.Abs(deltaHorizontal))
        {
            if (positionUp.y < positionDown.y)
                return SwipeDirections.Down;
            else
                return SwipeDirections.Up;
        }
        else
        {
            if (positionDown.x < positionUp.x)
                return SwipeDirections.Right;
            else
                return SwipeDirections.Left;
        }
    }

    SwipeDirections GetSwipeDirectionIsometric(Vector2 positionDown, Vector2 positionUp)
    {
        DebugMobile.PrintLabel1($"From {(int)positionDown.x}, {(int)positionDown.y} to {(int)positionUp.x}, {(int)positionUp.y}");

        var angle = Isometrics.GetSwipeAngle(positionDown, positionUp);
        var sqrDelta = Vector2.SqrMagnitude(positionUp - positionDown);
        var sqrSens = maxPixelDelta * maxPixelDelta * swipeSensitivity;

        if (sqrDelta >= sqrSens)
        {
            DebugMobile.PrintLabel2($"SqrDelta {(int)sqrDelta}");

            if (angle >= Isometrics.angleFR && angle <= Isometrics.angleFL)
                return SwipeDirections.Up;
            else if (angle > Isometrics.angleFL && angle < Isometrics.angleBL)
                return SwipeDirections.Left;
            else if (angle < Isometrics.angleFR && angle > Isometrics.angleBR)
                return SwipeDirections.Right;
            else
                return SwipeDirections.Down;
        }
        else
        {
            DebugMobile.PrintLabel2($"SqrDelta {(int)sqrDelta} (min: {(int)(sqrSens)})");
            return SwipeDirections.None;
        }

    }
}
