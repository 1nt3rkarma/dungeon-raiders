using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI : MonoBehaviourExtended
{
    public Monster monster;

    public float stoppingDistance = 0.8f;
    public bool avoidGaps;

    protected Hero hero { get => Hero.singlton; }

    public virtual void MainRoutine()
    {
        // Пример алгоритма ИИ - монстр движется вперед
        // и атакует Героя в ближнем бою
        if (monster.forwardBlock != null)
        {
            Hero hero = SearchHeroInLine();

            if (hero != null)
            {
                var distance = MathUtils.LineDistance(monster, hero);

                if (distance <= 2 && monster.isMoving)
                    OrderStop();

                if (distance <= 1 && hero.isAlive)
                    OrderAttack();

                if (distance > 2)
                    if (monster.canWalk && (!monster.forwardBlock.isEmpty || !avoidGaps))
                        OrderMove();
                    else if (monster.isMoving)
                        OrderStop();
            }
            else if (monster.canWalk && (!monster.forwardBlock.isEmpty || !avoidGaps))
                OrderMove();
            else if (monster.isMoving)
                OrderStop();
        }
    }

    protected void OrderMove()
    {
        monster.Move();
    }

    protected void OrderStop()
    {
        monster.Stop();
    }

    protected void OrderAttack()
    {
        monster.Attack();
    }

    public Hero SearchHeroInLine()
    {
        if (hero.line == monster.line)
            return Hero.singlton;
        else
            return null;
    }
}
