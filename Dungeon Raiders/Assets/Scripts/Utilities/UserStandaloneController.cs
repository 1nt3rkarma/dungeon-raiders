using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStandaloneController : UserController
{

    private void Update()
    {
        OnUpdate();
    }

    protected override void OnUpdate()
    {
        #region Имитация свайпа

        var dirHorizontal = 0;
        if (Input.GetKeyDown(KeyCode.A))
            dirHorizontal -= 1;
        if (Input.GetKeyDown(KeyCode.D))
            dirHorizontal += 1;

        if (dirHorizontal == 1)
            CatchSwipe(SwipeDirections.Right);
        if (dirHorizontal == -1)
            CatchSwipe(SwipeDirections.Left);

        var dirVertical = 0;
        if (Input.GetKeyDown(KeyCode.W))
            dirVertical += 1;
        if (Input.GetKeyDown(KeyCode.S))
            dirVertical -= 1;

        if (dirVertical == 1)
            CatchSwipe(SwipeDirections.Up);
        if (dirVertical == -1)
            CatchSwipe(SwipeDirections.Down);

        #endregion

        #region Имитация нажатия на тач-скрин

        //if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        if (Input.GetKeyDown(KeyCode.Space))
            CatchTapPress();

        if (Input.GetKeyUp(KeyCode.Space))
            //if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
            CatchTapRelease();

        if (tapPress1)
        {
            if (tapHoldTimer < tapHoldThreshold)
            {
                tapHoldTimer += Time.deltaTime;

                if (tapRelease1 && tapPress2)
                    CatchDoubleTap();
            }
            else
            {
                if (tapRelease1 && !tapPress2)
                    CatchSingleTap();
                else if (!tapRelease1)
                    CatchTapHold();
                else
                    CatchDoubleTap();
            }
        }

        #endregion
    }

}
