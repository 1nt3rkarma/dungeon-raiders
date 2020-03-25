using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Расширенный класс MonoBehaviourExtended служит интерфесом
/// для подписки на игровые события и задает сигнатуры методов,
/// обрабатывающих эти события
/// </summary>
public class MonoBehaviourExtended : MonoBehaviour
{
    protected void SubsribeToGameEvents()
    {
        // События пользовательского ввода

        GameEvent.onSingleTapEvent      += OnSingleTap;
        GameEvent.onDoubleTapEvent      += OnDoubleTap;
        GameEvent.onTapHoldEvent        += OnTapHold;
        GameEvent.onSwipeEvent          += OnSwipe;
        GameEvent.onTapPressEvent       += OnTapPress;
        GameEvent.onTapReleaseEvent     += OnTapRelease;

        // События игровой логики
        GameEvent.onHeroLeapEvent       += OnHeroLeap;
        GameEvent.onHeroJumpEvent       += OnHeroJump;
        GameEvent.onHeroAttackEvent     += OnHeroAttack;
        GameEvent.onHeroPicksItem       += OnHeroPicksItem;
        GameEvent.onPlayerUseItem       += OnPlayerUseItem;

    }

    protected virtual void UnsubsribeToGameEvents()
    {

        // События пользовательского ввода

        GameEvent.onSingleTapEvent      -= OnSingleTap;
        GameEvent.onDoubleTapEvent      -= OnDoubleTap;
        GameEvent.onTapHoldEvent        -= OnTapHold;
        GameEvent.onSwipeEvent          -= OnSwipe;
        GameEvent.onTapPressEvent       -= OnTapPress;
        GameEvent.onTapReleaseEvent     -= OnTapRelease;

        // События игровой логики
        GameEvent.onHeroLeapEvent       -= OnHeroLeap;
        GameEvent.onHeroJumpEvent       -= OnHeroJump;
        GameEvent.onHeroAttackEvent     -= OnHeroAttack;
        GameEvent.onHeroPicksItem       -= OnHeroPicksItem;
        GameEvent.onPlayerUseItem       -= OnPlayerUseItem;

    }

    #region События пользовательского вввода

    protected virtual void OnTapPress()
    {

    }

    protected virtual void OnTapRelease()
    {

    }

    protected virtual void OnSingleTap()
    {

    }

    protected virtual void OnDoubleTap()
    {

    }

    protected virtual void OnTapHold()
    {

    }

    protected virtual void OnSwipe(SwipeDirections direction)
    {

    }

    #endregion

    #region События игровой логики

    #region Действия героя

    protected virtual void OnHeroLeap(Hero hero, LeapDirections direction)
    {

    }

    protected virtual void OnHeroJump(Hero hero)
    {

    }

    protected virtual void OnHeroAttack(Hero hero)
    {

    }

    protected virtual void OnHeroPicksItem(Hero hero, Item item)
    {

    }

    protected virtual void OnPlayerUseItem(Item item)
    {

    }

    #endregion

    #endregion
}

public static class GameEvent
{
    #region События пользовательского ввода

    public static event Action<SwipeDirections> onSwipeEvent;
    public static void InvokeSwipe(SwipeDirections direction)
    {
        if (onSwipeEvent != null)
            onSwipeEvent(direction);
    }

    public static event Action onTapPressEvent;
    public static void InvokeTapPress()
    {
        if (onTapPressEvent != null)
            onTapPressEvent();
    }

    public static event Action onTapReleaseEvent;
    public static void InvokeTapRelease()
    {
        if (onTapReleaseEvent != null)
            onTapReleaseEvent();
    }

    public static event Action onSingleTapEvent;
    public static void InvokeSingleTap()
    {
        if (onSingleTapEvent != null)
            onSingleTapEvent();
    }

    public static event Action onDoubleTapEvent;
    public static void InvokeDoubleTap()
    {
        if (onDoubleTapEvent != null)
            onDoubleTapEvent();
    }

    public static event Action onTapHoldEvent;
    public static void InvokeTapHold()
    {
        if (onTapHoldEvent != null)
            onTapHoldEvent();
    }

    #endregion

    #region События игровой логики

    #region Действия героя

    public static event Action<Hero, LeapDirections> onHeroLeapEvent;
    public static void InvokeHeroLeap(Hero hero, LeapDirections direction)
    {
        if (onHeroLeapEvent != null)
            onHeroLeapEvent(hero, direction);
    }
    public static void InvokeHeroLeap(Hero hero)
    {
        if (onHeroLeapEvent != null)
            onHeroLeapEvent(hero, LeapDirections.None);
    }

    public static event Action<Hero> onHeroJumpEvent;
    public static void InvokeHeroJump(Hero hero)
    {
        if (onHeroJumpEvent != null)
            onHeroJumpEvent(hero);
    }

    public static event Action<Hero> onHeroAttackEvent;
    public static void InvokeHeroAttack(Hero hero)
    {
        if (onHeroAttackEvent != null)
            onHeroAttackEvent(hero);
    }

    public static event Action<Hero, Item> onHeroPicksItem;
    public static void InvokeHeroPicksItem(Hero hero, Item item)
    {
        if (onHeroPicksItem != null)
            onHeroPicksItem(hero, item);
    }

    public static event Action<Item> onPlayerUseItem;
    public static void InvokePlayerUseItem(Item item)
    {
        if (onPlayerUseItem != null)
            onPlayerUseItem(item);
    }

    #endregion

    #endregion

}
