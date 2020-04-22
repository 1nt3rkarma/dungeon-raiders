using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : Skill
{
    void Update()
    {
        OnUpdate();
    }

    protected override void BeginScenario()
    {
        base.BeginScenario();
        RequestAnimation(animationTagCast);
    }

    protected override void Cast()
    {
        caster.CastMissile();
    }

    protected override void CastEnded()
    {
        FinishScenario();
    }
}
