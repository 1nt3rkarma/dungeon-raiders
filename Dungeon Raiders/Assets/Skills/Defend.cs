﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defend : Skill
{
    [Header("Specific settings")]
    public string defendFlag = "isDefended";

    void Awake()
    {
        SubsribeToGameEvents();
    }

    void Update()
    {
        OnUpdate();
    }

    void OnDestroy()
    {
        Deinitialize();
        UnsubsribeToGameEvents();
    }


    protected override void OnUnitDamage(Unit unit, float damage, DamageType type, Object source)
    {
        if (state == SkillStates.casting && unit == caster)
        {
            if (charges == 1)
                StartCoroutine(DelayedFinishRoutine());
            else
                charges--;
        }               
    }

    protected override void BeginScenario()
    {
        base.BeginScenario();

        RequestSetAnimationFlag(defendFlag);
        caster.isDefending = true;
    }

    protected override void CastExpiredOrCancelled()
    {
        base.CastExpiredOrCancelled();

        FinishScenario();
    }

    public override void Interrupt()
    {
        base.Interrupt();

        //Debug.Log("Флаг isDefending снят");
        caster.isDefending = false;
        RequestClearAnimationFlag(defendFlag);
    }

    IEnumerator DelayedFinishRoutine()
    {
        yield return null;
        isCanceled = false;
        FinishScenario();
    }
}
