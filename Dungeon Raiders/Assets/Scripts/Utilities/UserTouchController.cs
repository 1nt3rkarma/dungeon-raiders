using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserTouchController : MonoBehaviour
{
    public static bool IsoMode = true;

    [Header("Settings")]

    [Tooltip("Max delay for holding a tap")]
    public float holdThreshold = 0.1f;
    public float holdTimer = 0;

    [Tooltip("Minimum distance of the swipe to count this action")]
    public float swipeSensitivity = 0.01f;
    private float minPixelDelta;

    private Vector2 touchPosDown;
    private Vector2 touchPosUp;
    private float touchDelta = 0;

    [Header("Debugging")]

    public bool pressCaptured;
    public bool actionCaptured; 

    void Update()
    {
        minPixelDelta = CameraController.singlton.cameraMain.pixelWidth / 2f * swipeSensitivity;

        if (!Player.controllEnabled)
            return;

        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchPosDown = touch.position;
                touchPosUp = touch.position;

                CatchPress();
            }
            else if (pressCaptured)
            {
                touchPosUp = touch.position;

                if (!actionCaptured)
                {
                    holdTimer += Time.deltaTime;

                    if (holdTimer >= holdThreshold)
                    {
                        SwipeDirections direction;
                        if (IsoMode)
                            direction = GetSwipeDirectionIsometric(touchPosDown, touchPosUp);
                        else
                            direction = GetSwipeDirection(touchPosDown, touchPosUp);

                        if (direction != SwipeDirections.None)
                            CatchSwipe(direction);
                        else
                            CatchStationary();
                    }
                    else
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

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    CatchRelease();
            }
        }
    }

    void CatchPress()
    {
        if (!pressCaptured)
        {
            var touch = Input.GetTouch(0);
            if (UI.GetUIElement(touch.position) != null)
            {
                Debug.Log($"В точке касания ЕСТЬ объект интерфейса: {UI.GetUIElement(touch.position).name}");
                Player.restrictControlls = true;
            }
            Debug.Log($"В точке каксания НЕТ объекта интерфейса");


            actionCaptured = false;
            holdTimer = 0;
            pressCaptured = true;

            GameEvent.InvokePress();
        }
    }

    void CatchRelease()
    {
        if (pressCaptured)
        {
            actionCaptured = false;
            holdTimer = 0;
            pressCaptured = false;
            GameEvent.InvokeRelease();
        }
    }

    void CatchStationary()
    {
        actionCaptured = true;
        GameEvent.InvokeStationary();
    }

    void CatchSwipe(SwipeDirections direction)
    {
        actionCaptured = true;
        GameEvent.InvokeSwipe(direction);
    }

    SwipeDirections GetSwipeDirection(Vector2 positionDown, Vector2 positionUp)
    {
        var deltaVertical = Mathf.Abs(positionUp.y - positionDown.y);
        var deltaHorizontal = Mathf.Abs(positionUp.x - positionDown.x);

        if (deltaVertical < minPixelDelta * swipeSensitivity
            && deltaHorizontal < minPixelDelta * swipeSensitivity)
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
        var angle = Isometrics.GetSwipeAngle(positionDown, positionUp);
        var sqrDelta = Vector2.SqrMagnitude(positionUp - positionDown);
        var sqrSens = minPixelDelta * minPixelDelta;

        if (sqrDelta >= sqrSens)
        {
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
            return SwipeDirections.None;
        }

    }
}

public enum SwipeDirections { None, Left, Right, Up, Down}
