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

        if (Input.GetKeyDown(KeyCode.Space))
            CatchTapPress();

        if (Input.GetKeyUp(KeyCode.Space))
            CatchTapRelease();

        if (tapPress)
        {
            if (tapHoldTimer < tapHoldThreshold)
            {
                tapHoldTimer += Time.deltaTime;
            }
            else
            {
                if (tapRelease)
                    CatchSingleTap();
                else if (!tapRelease)
                    CatchTapHold();
            }
        }

        #endregion
    }

}
