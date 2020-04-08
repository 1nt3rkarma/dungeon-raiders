using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserMobileController : UserController
{
    public static bool IsoMode = true;

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
        OnUpdate();
    }

    protected override void OnUpdate()
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

            touchPosUp = touch.position;

            if (touch.phase == TouchPhase.Ended)
            {
                CatchTapRelease();
            }
        }

        if (tapPress && !tapRelease)
            tapHoldTimer += Time.deltaTime;
        else
            tapHoldTimer = 0;

        if (tapPress)
        {
            if (tapHoldTimer >= tapHoldThreshold)
            {
                if (!tapRelease)
                    CatchTapHold();
            }
            else
            {
                if (tapRelease)
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
                else if (!tapRelease)
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
    }

    protected override void CatchTapPress()
    {
        base.CatchTapPress();

        var touch = Input.GetTouch(0);
        if (UI.GetUIElement(touch.position) != null)
            Player.restrictControlls = true;
    }

    SwipeDirections GetSwipeDirection(Vector2 positionDown, Vector2 positionUp)
    {
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
        var angle = Isometrics.GetSwipeAngle(positionDown, positionUp);
        var sqrDelta = Vector2.SqrMagnitude(positionUp - positionDown);
        var sqrSens = maxPixelDelta * maxPixelDelta * swipeSensitivity;

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
