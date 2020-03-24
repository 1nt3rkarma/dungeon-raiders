using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController : MonoBehaviour
{
    [Header("Debugging")]
    public float tapHoldTimer = 0;

    public bool tapPress1 = false;
    public bool tapRelease1 = false;
    public bool tapPress2 = false;
    public bool tapRelease2 = false;

    [Header("Settings")]

    [Tooltip("Max delay for holding a tap")]
    public float tapHoldThreshold = 0.22f;


    void Update()
    {
        OnUpdate();
    }

    // Update должен быть переопределен в потомках
    protected virtual void OnUpdate()
    {
        #region Имитация свайпа

        #endregion

        #region Имитация нажатия на тач-скрин

        #endregion
    }

    protected virtual void CatchTapPress()
    {
        if (!tapPress1)
            tapPress1 = true;
        else if (!tapPress2)
            tapPress2 = true;
    }

    protected virtual void CatchTapRelease()
    {
        if (!tapRelease1 && tapPress1)
            tapRelease1 = true;
        else if (!tapRelease2 && tapPress2)
            tapRelease2 = true;
    }

    protected void CatchSwipe(SwipeDirections direction)
    {
        GameEvent.InvokeSwipe(direction);

        ClearFlags();

        string d = "NONE";
        switch (direction)
        {
            case SwipeDirections.Right:
                d = "RIGHT";
                break;
            case SwipeDirections.Left:
                d = "LEFT";
                break;
            case SwipeDirections.Up:
                d = "UP";
                break;
            case SwipeDirections.Down:
                d = "DOWN";
                break;
            default:
                break;
        }
        DebugMobile.Log($"Swipe {d}");
    }

    protected void CatchSingleTap()
    {
        tapHoldTimer = 0;

        ClearFlags();

        GameEvent.InvokeSingleTap();
    }

    protected void CatchDoubleTap()
    {
        tapHoldTimer = 0;

        ClearFlags();

        GameEvent.InvokeDoubleTap();
    }

    protected void CatchTapHold()
    {
        tapHoldTimer = 0;

        ClearFlags();

        GameEvent.InvokeTapHold();
    }

    protected void ClearFlags()
    {
        tapPress1 = false;
        tapRelease1 = false;
        tapPress2 = false;
        tapRelease2 = false;
    }
}

public enum SwipeDirections { None, Right, Left, Up, Down }