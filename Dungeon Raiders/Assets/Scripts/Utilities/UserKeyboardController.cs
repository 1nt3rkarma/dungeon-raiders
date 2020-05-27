using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserKeyboardController : MonoBehaviour
{
    [Header("Settings")]

    [Tooltip("Max delay for holding a tap")]
    public float holdThreshold = 0.1f;
    public float holdTimer = 0;

    public KeyCode swipeUpKey = KeyCode.W;
    public KeyCode swipeDownKey = KeyCode.S;
    public KeyCode swipeLeftKey = KeyCode.A;
    public KeyCode swipeRightKey = KeyCode.D;
    public KeyCode tapKey = KeyCode.Space;

    [Space]
    public KeyCode item1Key = KeyCode.Alpha1;
    public KeyCode item2Key = KeyCode.Alpha2;
    public KeyCode item3Key = KeyCode.Alpha3;

    [Space]
    public SkillUI skillUI;
    public KeyCode skillKey = KeyCode.E;

    [Header("Debugging")]

    public bool pressCaptured;
    public bool actionCaptured;

    public bool AnyKey()
    {
        if (Input.GetKey(swipeUpKey))
            return true;
        if (Input.GetKey(swipeDownKey))
            return true;
        if (Input.GetKey(swipeLeftKey))
            return true;
        if (Input.GetKey(swipeRightKey))
            return true;
        if (Input.GetKey(tapKey))
            return true;

        return false;
    }

    public bool AnyKeyDown()
    {
        if (Input.GetKeyDown(swipeUpKey))
            return true;
        if (Input.GetKeyDown(swipeDownKey))
            return true;
        if (Input.GetKeyDown(swipeLeftKey))
            return true;
        if (Input.GetKeyDown(swipeRightKey))
            return true;
        if (Input.GetKeyDown(tapKey))
            return true;

        return false;
    }

    void Update()
    {
        if (!Player.controllEnabled)
            return;

        if (AnyKey())
        {
            if (AnyKeyDown())
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
                        if (Input.GetKey(swipeLeftKey))
                            dirHorizontal -= 1;
                        if (Input.GetKey(swipeRightKey))
                            dirHorizontal += 1;

                        if (dirHorizontal == 1)
                            CatchSwipe(SwipeDirections.Right);
                        if (dirHorizontal == -1)
                            CatchSwipe(SwipeDirections.Left);

                        var dirVertical = 0;
                        if (Input.GetKey(swipeUpKey))
                            dirVertical += 1;
                        if (Input.GetKey(swipeDownKey))
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

        if (Input.GetKeyDown(item1Key))
            Player.UseItem(0);
        if (Input.GetKeyDown(item2Key))
            Player.UseItem(1);
        if (Input.GetKeyDown(item3Key))
            Player.UseItem(2);

        if (Input.GetKeyDown(skillKey))
            skillUI.mainButton.OnPress();
        else if (Input.GetKeyUp(skillKey))
            skillUI.mainButton.OnRelease();
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
