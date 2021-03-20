using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Unit
{
    public static Hero singlton;

    [Header("Hero properties")]

    public float safeEdge = 0.18f;

    public bool isListening = true;

    //public Coroutine jumpRoutine;
    bool moveIsDelayed => delayMoveRoutine != null;

    Coroutine delayMoveRoutine;

    void Awake()
    {
        OnAwake();

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

    private void FixedUpdate()
    {
        OnFixedUpdate();
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
                TakeDamage(Health.Value, DamageType.fall, null);
    }

    void CheckFront()
    {
        bool therIsAnObstacle = false;
        var obstacles = GetObstaclesInRange();

        if (!therIsAnObstacle && obstacles.Count > 0)
            therIsAnObstacle = true;

        if (therIsAnObstacle && isMoving)
            Stop();
        else if (!therIsAnObstacle && !isBusy && !isMoving && !moveIsDelayed)
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
        Player.ContinueMoving(0.2f);
    }

    public void DelayMove(float delay)
    {
        if (delay > 0)
        {
            if (delayMoveRoutine != null)
                StopCoroutine(delayMoveRoutine);
            delayMoveRoutine = StartCoroutine(DelayMoveRoutine(delay));
        }
    }

    IEnumerator DelayMoveRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        delayMoveRoutine = null;
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

