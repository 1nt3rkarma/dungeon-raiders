using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Skill
{
    void Update()
    {
        OnUpdate();
    }

    protected override void Cast()
    {
        caster.CastDamage();
    }

    protected override void BeginScenario()
    {
        base.BeginScenario();

        Debug.Log($"Requesting ATTACK animation");
        RequestAnimation(animationTagCast);
    }

    protected override void CastEnded()
    {
        FinishScenario();
    }
}
