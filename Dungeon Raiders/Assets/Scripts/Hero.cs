using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Unit
{
    public static Hero singlton;

    [Header("Hero properties")]

    public int rowView;
    public int row { get => block.GetRowIndex(); }
    public int lineView;
    public int line { get => block.GetLineIndex(); }

    public Enemy enemy;
    public Block block;
    public Block nextBlock { get => Level.GetBlock(row + 1, line); }

    public float safeEdge = 0.15f;
    public float leapDuration = 0.25f;
    public bool isFloating = false;
    public bool isLeaping = false;
    public bool isMoving = true;
    public bool isListening = true;

    public Coroutine leapRoutine;
    public Coroutine jumpRoutine;
    public Coroutine castRoutine;

    void Awake()
    {
        if (singlton != null)
            Destroy(singlton.gameObject);
        singlton = this;
    }

    void Start()
    {
        block = Level.GetNearestBlock(transform.position);
        rowView = row;
    }

    void Update()
    {
        lineView = line;

        if (Player.controllEnabled && isAlive)
            CheckMovement();

        if (isAlive)
            CheckGround();
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

    void CheckMovement()
    {
        var objects = nextBlock.ScanObjects();
        foreach (var obj in objects)
        {
            if (this.enemy == null)
            {
                var searchEnemy = obj.GetComponent<Enemy>();
                if (searchEnemy != null)
                    enemy = searchEnemy;
            }
        }

        if (enemy != null)
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

    void Stop()
    {
        animHandler.SetMoveFlag(false);
        isMoving = false;
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

    public void Leap(LeapDirections direction)
    {
        if (!isBusy && isAlive)
            if (leapRoutine == null)
            {
                if (direction == LeapDirections.Left && line == 0)
                    return;
                if (direction == LeapDirections.Right && line == 2)
                    return;

                leapRoutine = StartCoroutine(LeapRoutine(direction));
            }
    }

    IEnumerator LeapRoutine(LeapDirections direction)
    {
        isBusy = true;
        GameEvent.InvokeHeroLeap(this, direction);

        if (direction == LeapDirections.Left)
            animHandler.PlayAnimation("leapL");
        if (direction == LeapDirections.Right)
            animHandler.PlayAnimation("leapR");

        // Ждем событие анимации - прыжок
        yield return WaitForAnimationEvent(AnimationEvents.jumpStart);
        isLeaping = true;

        StartCoroutine(LeapTransitionRoutine(direction));

        // Ждем событие анимации - конец прыжка
        yield return WaitForAnimationEvent(AnimationEvents.jumpEnd);
        isLeaping = false;

        leapRoutine = null;
        isBusy = false;
        enemy = null;
    }

    IEnumerator LeapTransitionRoutine(LeapDirections direction)
    {
        var speed = 1 / leapDuration;

        int sign = (int)direction;
        var targetPosition = transform.position + sign * transform.right;

        if (direction == LeapDirections.Left)
            while (transform.position.x > targetPosition.x)
            {
                transform.position -= transform.right * speed * Time.deltaTime;
                yield return null;
            }
        else if (direction == LeapDirections.Right)
            while (transform.position.x < targetPosition.x)
            {
                transform.position += transform.right * speed * Time.deltaTime;
                yield return null;
            }
        transform.position = targetPosition;
    }

    public void SwitchBlockTo(Block block)
    {
        this.block = block;
    }

    #endregion

    #region Нанесение и получение урона

    public void MeleeAttack()
    {
        if (!isBusy && isAlive)
            if (castRoutine == null)
            {
                castRoutine = StartCoroutine(MeleeAttackRoutine());
            }
    }

    IEnumerator MeleeAttackRoutine()
    {
        isBusy = true;
        Stop();

        GameEvent.InvokeHeroAttack(this);
        animHandler.PlayAnimation("attack");

        // Ждем событие анимации - начало анимации
        while (!animHandler.animEventCastStart)
            yield return null;
        animHandler.animEventCastStart = false;

        // Ждем событие анимации - применение
        while (!animHandler.animEventCast)
            yield return null;
        animHandler.animEventCast = false;
        CastMeleeDamage();

        // Ждем событие анимации - окончание анимации
        while (!animHandler.animEventCastEnd)
            yield return null;
        animHandler.animEventCastEnd = false;

        castRoutine = null;
        isBusy = false;
    }

    public void CastMeleeDamage()
    {
        if (enemy)
            enemy.TakeDamage(1);
    }

    public override void TakeDamage(float damage, DamageSources source)
    {
        base.TakeDamage(damage, source);

        if (health > 0)
            StartCoroutine(HitRoutine());
    }

    IEnumerator HitRoutine()
    {
        isBusy = true;

        InterruptRoutines();
        animHandler.ClearFalgs();

        animHandler.PlayAnimation("hit");

        // Ждем событие анимации - начало анимации
        yield return WaitForAnimationEvent(AnimationEvents.start);

        // Ждем событие анимации - конец анимации
        yield return WaitForAnimationEvent(AnimationEvents.end);

        isBusy = false;
    }

    public override void Die(DamageSources source)
    {
        base.Die(source);

        InterruptRoutines();

        Stop();

        Player.Defeat();
    }

    #endregion

    void InterruptRoutines()
    {
        if (leapRoutine != null)
            StopCoroutine(leapRoutine);
        leapRoutine = null;

        if (jumpRoutine != null)
            StopCoroutine(jumpRoutine);
        jumpRoutine = null;

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
    }
}

public enum HeroResources { health, mana }