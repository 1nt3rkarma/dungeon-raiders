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
            bool obstacle = false;

            var units = zombie.GetUnitsInRange();

            foreach (var unit in units)
            {
                var distance = Mathf.Abs(zombie.position.z - unit.position.z);
                if (distance <= (zombie.attackRange + zombie.attackAreaSize) && !obstacle)
                    obstacle = true;
            }

            if (hero != null)
            {
                var delta = MathUtilities.LineDelta(zombie, hero);
                var distance = Mathf.Abs(zombie.position.z - hero.position.z);

                if (delta > -1 && distance <= (zombie.attackRange + zombie.attackAreaSize))
                {
                    obstacle = true;

                    if (hero.isAlive)
                        OrderAttack();
                }
            }

            if (monster.canWalk && !obstacle && (!monster.forwardBlock.isEmpty || !avoidGaps))
                OrderMove();
            else if (monster.isMoving)
                OrderStop();
        }
    }
}
