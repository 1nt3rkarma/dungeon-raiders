using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Unit
{
    [Header("Monster settings")]
    public bool canWalk;
    public float moveSpeed;

    public MonsterAI AI;

    private void Awake()
    {
        OnAwake();
    }

    private void Start()
    {
        OnStart();
        facing = Facing.backward;
    }

    private void Update()
    {
        if (isAlive)
            AI.MainRoutine();
    }

    private void FixedUpdate()
    {
        OnFixedUpdate();
    }

    public void Move()
    {
        if (!isBusy && isAlive)
        {
            if (!isMoving)
            {
                animHandler.SetMoveFlag(true);
                isMoving = true;
            }

            var direction = (int)facing * Vector3.forward;
            transform.localPosition += direction * moveSpeed * Time.deltaTime;

            var newBlock = Level.GetBlock(transform.position);
            if (newBlock != block)
            {
                block = newBlock;
                transform.SetParent(block.transform.parent);
            }
        }
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

        if (shootPoint)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(shootPoint.position, new Vector3(0.3f, 0.3f, 0.3f));
            Gizmos.DrawLine(shootPoint.position, shootPoint.position + transform.forward);
        }
    }
}
