using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserKeyboardController : MonoBehaviour
{
    [Header("Settings")]

    [Tooltip("Max delay for holding a tap")]
    public float holdThreshold = 0.1f;
    public float holdTimer = 0;

    [Header("Debugging")]

    public bool pressCaptured;
    public bool actionCaptured;


    void Update()
    {
        if (!Player.controllEnabled)
            return;

        if (Input.anyKey && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            if (Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
                CatchPress();
            else
            {
                if (!actionCaptured)
                {
                    holdTimer += Time.deltaTime;

                    if (holdTimer >= holdThreshold)
                        CatchStationary();
                    else
                    {
                        var dirHorizontal = 0;
                        if (Input.GetKey(KeyCode.A))
                            dirHorizontal -= 1;
                        if (Input.GetKey(KeyCode.D))
                            dirHorizontal += 1;

                        if (dirHorizontal == 1)
                            CatchSwipe(SwipeDirections.Right);
                        if (dirHorizontal == -1)
                            CatchSwipe(SwipeDirections.Left);

                        var dirVertical = 0;
                        if (Input.GetKey(KeyCode.W))
                            dirVertical += 1;
                        if (Input.GetKey(KeyCode.S))
                            dirVertical -= 1;

                        if (dirVertical == 1)
                            CatchSwipe(SwipeDirections.Up);
                        if (dirVertical == -1)
                            CatchSwipe(SwipeDirections.Down);
                    }
                }
            }
        }
        else
            CatchRelease();

    }

    void CatchPress()
    {
        if (!pressCaptured)
        {
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
}
