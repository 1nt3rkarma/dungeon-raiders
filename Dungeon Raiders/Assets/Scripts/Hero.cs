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

        attackTimer -= Time.deltaTime;
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
            {
                Debug.Log($"Hero z: {heroZ}");
                Debug.Log($"Block z: {blockZ}");
                TakeDamage(health, DamageSources.fall);
                if (blockZ < heroZ)
                    Level.HaltFlow();
            }
    }

    void CheckFront()
    {
        var objects = forwardBlock.ScanObjects();
        GameObject obstacle = null;
        foreach (var obj in objects)
            if (obstacle == null)
            {
                var searchEnemy = obj.GetComponent<Monster>();
                if (searchEnemy != null)
                    obstacle = searchEnemy.gameObject;
            }

        if (obstacle != null)
        {
            if (isMoving)
                Stop();
        }
        else
        {
            if (!isBusy && !isMoving)
                Move();
        }
    }

    public override void Stop()
    {
        base.Stop();
        Player.StopMoving();
    }

    void Move()
    {
        animHandler.SetMoveFlag(true);
        isMoving = true;
        Player.ContinueMoving();
    }

    public void Jump()
    {
        if (!isBusy && isAlive)
            if (jumpRoutine == null)
                jumpRoutine = StartCoroutine(JumpRoutine());
    }

    IEnumerator JumpRoutine()
    {
        isBusy = true;

        GameEvent.InvokeHeroJump(this);
        animHandler.PlayAnimation("jump");

        // Ждем событие анимации - прыжок
        yield return WaitForAnimationEvent(AnimationEvents.jumpStart);
        isFloating = true;


        // Ждем событие анимации - прыжок закончен
        yield return WaitForAnimationEvent(AnimationEvents.jumpEnd);
        isFloating = false;


        isBusy = false;
        jumpRoutine = null;
    }

    #endregion

    #region Нанесение и получение урона

    public override void TakeDamage(float damage)
    {
        TakeDamage(damage, DamageSources.common);
    }

    public override void TakeDamage(float damage, DamageSources source)
    {
        InterruptRoutines();
        base.TakeDamage(damage, source);
    }

    public override void Die(DamageSources source)
    {
        base.Die(source);

        Stop();

        Player.Defeat();
    }

    #endregion

    protected override void InterruptRoutines()
    {
        if (leapRoutine != null)
            StopCoroutine(leapRoutine);
        leapRoutine = null;
        isLeaping = false;

        if (jumpRoutine != null)
            StopCoroutine(jumpRoutine);
        jumpRoutine = null;
        isFloating = false;

        if (castRoutine != null)
            StopCoroutine(castRoutine);
        castRoutine = null;
    }

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