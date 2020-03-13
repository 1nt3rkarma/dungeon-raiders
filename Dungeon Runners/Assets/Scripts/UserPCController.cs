using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPCController : MonoBehaviour
{
    [Tooltip("Макс. задержка между тапами")]
    public float tapHoldLimit = 0.22f;

    [Header("Для дебаггинга")]
    public float tapHoldTimer = 0;

    public bool tapHold1 = false;
    public bool tapRelease1 = false;
    public bool tapHold2 = false;
    public bool tapRelease2 = false;

    void Update()
    {
        #region Имитация свайпа

        var direction = 0;
        if (Input.GetKeyDown(KeyCode.A))
            direction -= 1;
        if (Input.GetKeyDown(KeyCode.D))
            direction += 1;

        if (direction == 1)
            CatchSwipe(SwipeDirections.Right);
        if (direction == -1)
            CatchSwipe(SwipeDirections.Left);

        #endregion

        #region Имитация нажатия на тач-скрин

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            CatchTapPress();

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
            CatchTapRelease();

        if (tapHold1)
        {
            if (tapHoldTimer < tapHoldLimit)
            {
                tapHoldTimer += Time.deltaTime;

                if (tapRelease1 && tapHold2)
                    CatchDoubleTap();
            }
            else
            {
                if (tapRelease1 && !tapHold2)
                    CatchSingleTap();
                else if (!tapRelease1)
                    CatchTapHold();
                else
                    CatchDoubleTap();
            }
        }

        #endregion
    }

    void CatchSwipe(SwipeDirections direction)
    {
        GameEvent.InvokeSwipe(direction);
    }

    void CatchTapPress()
    {
        if (!tapHold1)
            tapHold1 = true;
        else if (!tapHold2)
            tapHold2 = true;
    }

    void CatchTapRelease()
    {
        if (!tapRelease1 && tapHold1)
            tapRelease1 = true;
        else if (!tapRelease2 && tapHold2)
            tapRelease2 = true;
    }

    void CatchSingleTap()
    {
        tapHoldTimer = 0;

        tapHold1 = false;
        tapRelease1 = false;
        tapHold2 = false;
        tapRelease2 = false;

        Debug.Log("Произошел один тап");
        GameEvent.InvokeSingleTap();
    }

    void CatchDoubleTap()
    {
        tapHoldTimer = 0;

        tapHold1 = false;
        tapRelease1 = false;
        tapHold2 = false;
        tapRelease2 = false;

        Debug.Log("Произошел двойной тап");
        GameEvent.InvokeDoubleTap();
    }

    void CatchTapHold()
    {
        tapHoldTimer = 0;

        tapHold1 = false;
        tapRelease1 = false;
        tapHold2 = false;
        tapRelease2 = false;

        Debug.Log("Произошло удержание");
        GameEvent.InvokeTapHold();
    }
}
