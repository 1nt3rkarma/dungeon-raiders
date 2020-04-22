using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAI : MonsterAI
{
    Monster skeleton { get => monster; }

    public int shootDistance;
    public bool shootLineFactor;

    public override void MainRoutine()
    {
        Hero hero = SearchHeroInLine();

        if (hero != null)
            if (hero.isAlive && skeleton.ammo > 0)
            {
                var delta = MathUtilities.LineDelta(skeleton, hero);
                var distance = MathUtilities.LineDistance(skeleton, hero);

                var shootDistance = this.shootDistance;
                if (shootLineFactor)
                    shootDistance -= skeleton.line;

                if (distance < shootDistance && delta > 0)
                    skeleton.Shoot();
            }
    }
}
