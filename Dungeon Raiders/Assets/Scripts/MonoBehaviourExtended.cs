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

        GameEvent.onStationaryEvent  += OnStationary;
        GameEvent.onSwipeEvent       += OnSwipe;
        GameEvent.onPressEvent       += OnPress;
        GameEvent.onReleaseEvent     += OnRelease;

        // События игровой логики
        // Действия Юнита
        GameEvent.onUnitLeapEvent       += OnUnitLeap;
        GameEvent.onUnitAttackEvent     += OnUnitAttack;
        GameEvent.onUnitDamageEvent     += OnUnitDamage;
        GameEvent.onUnitDieEvent        += OnUnitDie;
        GameEvent.onUnitJumpEvent       += OnUnitJump;
        // Действия Героя
        GameEvent.onHeroPicksItem       += OnHeroPicksItem;
        // Действия Игрока
        GameEvent.onPlayerUseItem       += OnPlayerUseItem;

    }

    protected virtual void UnsubsribeToGameEvents()
    {

        // События пользовательского ввода

        GameEvent.onStationaryEvent  -= OnStationary;
        GameEvent.onSwipeEvent       -= OnSwipe;
        GameEvent.onPressEvent       -= OnPress;
        GameEvent.onReleaseEvent     -= OnRelease;

        // События игровой логики
        // Действия Юнита
        GameEvent.onUnitLeapEvent       -= OnUnitLeap;
        GameEvent.onUnitAttackEvent     -= OnUnitAttack;
        GameEvent.onUnitDamageEvent     -= OnUnitDamage;
        GameEvent.onUnitDieEvent        -= OnUnitDie;
        GameEvent.onUnitJumpEvent       -= OnUnitJump;
        // Действия Героя
        GameEvent.onHeroPicksItem       -= OnHeroPicksItem;
        // Действия Игрока
        GameEvent.onPlayerUseItem       -= OnPlayerUseItem;

    }

    #region События пользовательского вввода

    protected virtual void OnPress()
    {

    }

    protected virtual void OnRelease()
    {

    }

    protected virtual void OnStationary()
    {

    }

    protected virtual void OnSwipe(SwipeDirections direction)
    {

    }

    #endregion

    #region События игровой логики

    #region Действия Юнита

    protected virtual void OnUnitLeap(Unit unit, LeapDirections direction)
    {

    }

    protected virtual void OnUnitAttack(Unit unit)
    {

    }

    protected virtual void OnUnitDamage(Unit unit, float damage, DamageType type, UnityEngine.Object source)
    {

    }

    protected virtual void OnUnitDie(Unit unit, DamageType type, UnityEngine.Object source)
    {

    }

    protected virtual void OnUnitJump(Unit unit)
    {

    }

    #region Действия Героя

    protected virtual void OnHeroPicksItem(Hero hero, Item item)
    {

    }

    #endregion

    #region Действия Игрока

    protected virtual void OnPlayerUseItem(Item item)
    {

    }

    #endregion

    #endregion

    #endregion
}

public static class GameEvent
{
    #region События пользовательского ввода

    public static event Action onPressEvent;
    public static void InvokePress()
    {
        //Debug.Log("Заметили НАЖАТИЕ");

        if (onPressEvent != null)
            onPressEvent();
    }

    public static event Action onReleaseEvent;
    public static void InvokeRelease()
    {
        //Debug.Log("Заметили ОТПУСКАНИЕ");
        if (onReleaseEvent != null)
            onReleaseEvent();
    }

    public static event Action<SwipeDirections> onSwipeEvent;
    public static void InvokeSwipe(SwipeDirections direction)
    {
        //Debug.Log($"Произошел СВАЙП: {direction}");

        if (onSwipeEvent != null)
            onSwipeEvent(direction);
    }

    public static event Action onStationaryEvent;
    public static void InvokeStationary()
    {
        //Debug.Log("Произошло УДЕРЖАНИЕ");

        if (onStationaryEvent != null)
            onStationaryEvent();
    }

    #endregion

    #region События игровой логики

    #region Действия Юнита

    public static event Action<Unit, LeapDirections> onUnitLeapEvent;
    public static void InvokeUnitLeap(Unit unit, LeapDirections direction)
    {
        if (onUnitLeapEvent != null)
            onUnitLeapEvent(unit, direction);
    }
    public static void InvokeUnitLeap(Unit unit)
    {
        if (onUnitLeapEvent != null)
            onUnitLeapEvent(unit, LeapDirections.None);
    }

    public static event Action<Unit> onUnitAttackEvent;
    public static void InvokeUnitAttack(Unit unit)
    {
        if (onUnitAttackEvent != null)
            onUnitAttackEvent(unit);
    }

    public static event Action<Unit, float, DamageType, UnityEngine.Object> onUnitDamageEvent;
    public static void InvokeUnitDamage(Unit unit, float damage, DamageType type, UnityEngine.Object source)
    {
        if (onUnitDamageEvent != null)
            onUnitDamageEvent(unit, damage, type, source);
    }

    public static event Action<Unit, DamageType, UnityEngine.Object> onUnitDieEvent;
    public static void InvokeUnitDie(Unit unit, DamageType type, UnityEngine.Object source)
    {
        if (onUnitDieEvent != null)
            onUnitDieEvent(unit, type, source);
    }

    public static event Action<Unit> onUnitJumpEvent;
    public static void InvokeUnitJump(Unit unit)
    {
        if (onUnitJumpEvent != null)
            onUnitJumpEvent(unit);
    }

    #endregion

    #region Действия Героя

    public static event Action<Hero, Item> onHeroPicksItem;
    public static void InvokeHeroPicksItem(Hero hero, Item item)
    {
        if (onHeroPicksItem != null)
            onHeroPicksItem(hero, item);
    }

    #endregion

    #region Действия Игрока

    public static event Action<Item> onPlayerUseItem;
    public static void InvokePlayerUseItem(Item item)
    {
        if (onPlayerUseItem != null)
            onPlayerUseItem(item);
    }

    #endregion

    #endregion
}
