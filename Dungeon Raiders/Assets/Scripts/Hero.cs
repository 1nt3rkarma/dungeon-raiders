using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Unit
{
    public static Hero singlton;

    [Header("Hero properties")]

    public float safeEdge = 0.15f;

    public bool isListening = true;

    public Coroutine jumpRoutine;

    void Awake()
    {
        InitSinglton(this);
    }

    void Start()
    {
        OnStart();
    }

    void Update()
    {
        if (Player.controllEnabled && isAlive)
            CheckFront();

        if (isAlive)
            CheckGround();
    }

    static void InitSinglton(Hero instance)
    {
        if (singlton != null)
            Destroy(singlton.gameObject);
        singlton = instance;
    }

    #region Перемещение

    void CheckGround()
    {
        var blockZ = block.transform.position.z;
        var heroZ = this.transform.position.z;

        if (!isLeaping && !isFloating && block.isEmpty)
            if (heroZ > (blockZ - 0.5f + safeEdge) && heroZ < (blockZ + 0.5f - safeEdge))
                TakeDamage(health, DamageType.fall, null);
    }

    void CheckFront()
    {
        bool obstacle = false;
        var units = GetUnitsInRange();

        foreach (var unit in units)
        {
            var distance = Mathf.Abs(this.position.z - unit.position.z);
            if (distance <= (attackRange + attackAreaSize) && !obstacle)
                obstacle = true;
        }

        if (obstacle && isMoving)
            Stop();
        else if (!obstacle && !isBusy && !isMoving)
            RequireMove();
    }

    public override void Stop()
    {
        base.Stop();
        Player.StopMoving();
    }

    void RequireMove()
    {
        animHandler.SetMoveFlag(true);
        isMoving = true;
        Player.ContinueMoving();
    }

    #endregion

    void OnDrawGizmosSelected()
    {
        if (block != null)
        {
            Gizmos.color = Color.yellow;
            var center = block.transform.position; ;
            var size = 0.5f;
            Gizmos.DrawWireSphere(center, size);
        }

        if (attackPoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackAreaSize);
        }
    }
}

public enum HeroResources { health, mana }