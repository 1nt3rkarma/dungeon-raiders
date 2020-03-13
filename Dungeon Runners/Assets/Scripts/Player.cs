﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviourExtended
{
    static Player singlton;

    public Hero hero;

    public static bool controllEnabled = true;

    public static int steps = 0;
    public static int coins = 0;

    #region Реакции на события

    #region События движка Unity

    void Awake()
    {
        if (singlton != null)
            Destroy(singlton.gameObject);
        singlton = this;

        SubsribeToGameEvents();
    }

    void Start()
    {
        steps = 0;
        coins = 0;
    }

    void OnDestroy()
    {
        UnsubsribeToGameEvents();
    }

    #endregion

    #region Игровые события

    protected override void OnTapPress()
    {

    }

    protected override void OnTapRelease()
    {

    }

    protected override void OnSingleTap()
    {
        OrderJump();
    }

    protected override void OnDoubleTap()
    {
        OrderAttack();
    }

    protected override void OnTapHold()
    {

    }

    protected override void OnSwipe(SwipeDirections direction)
    {
        switch (direction)  
        {
            case SwipeDirections.Right:
                OrderLeap(LeapDirections.Right);
                break;
            case SwipeDirections.Left:
                OrderLeap(LeapDirections.Left);
                break;
            default:
                break;
        }
    }

    #endregion

    #endregion

    #region Методы

    public void OrderAttack()
    {
        hero.MeleeAttack();
    }

    public void OrderJump()
    {
        hero.Jump();
    }

    public void OrderLeap(LeapDirections direction)
    {
        hero.Leap(direction);
    }

    public static void CountStep()
    {

    }

    public static void AddCoins(int amount)
    {
        coins += amount;
    }

    public static void StopMoving()
    {
        Level.StopFlow();
    }

    public static void ContinueMoving()
    {
        Level.RunFlow();
    }

    public static void Defeat()
    {
        UI.ShowDefeatUI();
    }

    public static void ReloadLevel()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index);
    }

    #endregion

}

public enum SwipeDirections { None, Right, Left, Up, Down }

public enum LeapDirections { None = 0, Right = 1, Left = -1}
