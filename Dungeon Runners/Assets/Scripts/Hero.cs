using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Unit
{
    [Header("Hero properties")]

    public int rowView;
    public int row { get => block.GetRowIndex(); }
    public int lineView;
    public int line { get => block.GetLineIndex(); }

    public Enemy enemy;
    public Block block;
    public Block nextBlock { get => GetNextBlock(); }

    public float shiftDuration = 0.25f;
    public bool isFloating = false;
    public bool isMoving = true;

    public Coroutine leapRoutine;
    public Coroutine jumpRoutine;
    public Coroutine castRoutine;

    void Start()
    {
        block = Level.GetNearestBlock(transform.position);
        rowView = row;
    }

    void Update()
    {
        lineView = line;

        if (Player.controllEnabled && isAlive)
            CheckFront();
    }

    void CheckFront()
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
        Debug.Log("Герой остановился");
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

    public Block GetNextBlock()
    {
        return Level.GetBlock(row+1,line);
    }

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

        yield return null;
    }

    public void CastMeleeDamage()
    {
        if (enemy)
            enemy.TakeDamage(1);
    }

    public void Jump()
    {
        if (!isBusy && isAlive)
        {
            if (jumpRoutine == null)
            {
                jumpRoutine = StartCoroutine(JumpRoutine());
            }
            else
            {
                Debug.Log("Прыжок не произошел, т. к. корутина все еще активна");
            }
        }
        else
        {
            Debug.Log("Прыжок не произошел, т. к. Герой занят");
        }
    }

    IEnumerator JumpRoutine()
    {
        isBusy = true;

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
        var speed = 1 / shiftDuration;

        if (direction == LeapDirections.Left)
            animHandler.PlayAnimation("leapL");
        if (direction == LeapDirections.Right)
            animHandler.PlayAnimation("leapR");

        int sign = (int)direction;
        var targetPosition = transform.position + sign * transform.right;

        // Ждем событие анимации - прыжок
        yield return WaitForAnimationEvent(AnimationEvents.jumpStart);

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
        leapRoutine = null;
        isBusy = false;
        enemy = null;
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

    void InterruptRoutines()
    {
        if (jumpRoutine != null)
            StopCoroutine(jumpRoutine);
        jumpRoutine = null;

        if (castRoutine != null)
            StopCoroutine(castRoutine);
        castRoutine = null;
    }

    public void SwitchBlockTo(Block block)
    {
        if (block.GetRowIndex() > this.block.GetRowIndex())
            Player.AddStep();

        this.block = block;
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
