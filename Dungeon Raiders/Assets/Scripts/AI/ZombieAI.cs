using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAI : MonsterAI
{
    Monster zombie { get => monster; }

    public override void MainRoutine()
    {
        if (monster.forwardBlock != null)
        {
            Hero hero = SearchHeroInLine();
            bool thereIsAnObstacle = false;

            var obstacles = zombie.GetObstaclesInRange();

            if (!thereIsAnObstacle && obstacles.Count > 0)
                thereIsAnObstacle = true;

            if (hero != null)
                if (hero.isAlive && obstacles.Contains(hero.gameObject))
                    OrderAttack();


            if (monster.canWalk && !thereIsAnObstacle && (!monster.forwardBlock.isEmpty || !avoidGaps))
                OrderMove();
            else if (monster.isMoving)
                OrderStop();

        }
    }
}
