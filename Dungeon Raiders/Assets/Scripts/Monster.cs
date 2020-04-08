using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Unit
{
    [Header("Monster settings")]
    public bool canWalk;
    public bool avoidGaps;
    public float moveSpeed;

    public Coroutine moveRoutine;

    private void Start()
    {
        OnStart();
        facing = Facing.backward;
    }

    private void Update()
    {
        if (isAlive)
            CheckFront();

        attackTimer -= Time.deltaTime;
    }

    void CheckFront()
    {
        if (forwardBlock != null)
        {
            Hero hero = SearchHeroInLine();

            if (hero != null)
            {
                var distance = DistanceTo(hero);

                if (distance <= 2 && isMoving)
                    Stop();

                if (distance <= 1 && hero.isAlive)
                    MeleeAttack();

                if (distance > 2)
                    if (canWalk && (!forwardBlock.isEmpty || !avoidGaps))
                        Move();
                    else if (isMoving)
                        Stop();
            }
            else if (canWalk && (!forwardBlock.isEmpty || !avoidGaps))
                Move();
            else if (isMoving)
                Stop();
        }
    }

    public void Move()
    {
        if (!isBusy && isAlive)
            if (moveRoutine == null)
            {
                animHandler.SetMoveFlag(true);
                isMoving = true;

                var direction = (int)facing * Vector3.forward;
                transform.localPosition += direction * moveSpeed * Time.deltaTime;

                var newBlock = Level.GetNearestBlock(transform.position);
                if (newBlock != block)
                {
                    block = newBlock;
                    transform.SetParent(block.transform.parent);
                }
            }
    }

    public int DistanceTo(Unit unit)
    {
        return Mathf.Abs(this.block.GetRowIndex() - unit.block.GetRowIndex());
    }

    public Hero SearchHeroInLine()
    {
        if (Hero.singlton.line == this.line)
            return Hero.singlton;
        else
            return null;
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
