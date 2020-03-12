using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public int row { get => block.GetRowIndex(); }
    public int rowView;
    public int line { get => block.GetLineIndex(); }
    public int lineView;

    public Enemy enemy;
    public Block block;
    public Block nextBlock { get => GetNextBlock(); }
    //public Player player;

    public float shiftDuration = 0.25f;
    public bool isFloating = false;
    public bool isMoving = true;
    public bool isBusy = false;

    public Animator animator { get => animHandler.animator; }
    public HeroAnimationHandler animHandler;

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

        if (Player.controllEnabled)
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
        Debug.Log("Остановились");
        animHandler.SetMoveFlag(false);
        isMoving = false;
        Player.StopMoving();
    }

    void Move()
    {
        Debug.Log("Пошли дальше");
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
        if (!isBusy)
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
            enemy.TakeDamage();
    }

    public void Jump()
    {
        if (!isBusy)
            if (jumpRoutine == null)
            {
                jumpRoutine = StartCoroutine(JumpRoutine());
            }
    }

    IEnumerator JumpRoutine()
    {
        isBusy = true;

        animHandler.PlayAnimation("jump");

        // Ждем событие анимации - прыжок
        while (!animHandler.animEventJumpStart)
            yield return null;
        animHandler.animEventJumpStart = false;

        isFloating = true;

        // Ждем событие анимации - прыжок закончен
        while (!animHandler.animEventJumpEnd)
            yield return null;
        animHandler.animEventJumpEnd = false;

        isFloating = false;

        isBusy = false;
        jumpRoutine = null;
    }

    public void Leap(LeapDirections direction)
    {
        if (!isBusy)
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

        int sign;
        if (direction == LeapDirections.Left)
            sign = -1;
        else
            sign = 1;

        var targetPosition = transform.position + sign * transform.right;
        var distance = Mathf.Abs(transform.position.x - targetPosition.x);

        // Ждем событие анимации - прыжок
        while (!animHandler.animEventLeap)
            yield return null;
        animHandler.animEventLeap = false;

        while (distance > 0.1)
        {
            transform.position += sign * transform.right * speed * Time.deltaTime;
            distance = Mathf.Abs(transform.position.x - targetPosition.x);
            yield return null;
        }

        transform.position = targetPosition;
        leapRoutine = null;
        isBusy = false;
        enemy = null;
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
