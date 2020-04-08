using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController : MonoBehaviour
{
    [Header("Debugging")]
    public float tapHoldTimer = 0;

    public bool tapPress = false;
    public bool tapRelease = false;

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
        if (!tapPress)
        {
            GameEvent.InvokeTapPress();
            tapPress = true;
        }
    }

    protected virtual void CatchTapRelease()
    {
        if (!tapRelease && tapPress)
        {
            GameEvent.InvokeTapRelease();
            tapRelease = true;
        }
    }

    protected virtual void CatchSwipe(SwipeDirections direction)
    {
        if (direction != SwipeDirections.None)
            GameEvent.InvokeSwipe(direction);

        ClearFlags();

        //string d = "NONE";
        //switch (direction)
        //{
        //    case SwipeDirections.Right:
        //        d = "RIGHT";
        //        break;
        //    case SwipeDirections.Left:
        //        d = "LEFT";
        //        break;
        //    case SwipeDirections.Up:
        //        d = "UP";
        //        break;
        //    case SwipeDirections.Down:
        //        d = "DOWN";
        //        break;
        //    default:
        //        break;
        //}
        //DebugMobile.Log($"Swipe {d}");
    }

    protected virtual void CatchSingleTap()
    {
        tapHoldTimer = 0;

        ClearFlags();

        GameEvent.InvokeSingleTap();
    }

    protected virtual void CatchDoubleTap()
    {
        tapHoldTimer = 0;

        ClearFlags();

        GameEvent.InvokeDoubleTap();
    }

    protected virtual void CatchTapHold()
    {
        tapHoldTimer = 0;

        ClearFlags();

        GameEvent.InvokeTapHold();
    }

    protected virtual void ClearFlags()
    {
        tapPress = false;
        tapRelease = false;
    }
}

public enum SwipeDirections { None, Right, Left, Up, Down }