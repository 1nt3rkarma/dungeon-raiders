﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : Skill
{
    public string jumpStartTag = "jumpUp";
    public string jumpEndTag = "jumpDown";

    void Update()
    {
        OnUpdate();
    }

    protected override void BeginScenario()
    {
        //Debug.Log("Запрашиваем анимацию прыжка");

        base.BeginScenario();
        RequestAnimation(jumpStartTag);
    }

    protected override void CastStarted()
    {
        caster.isFloating = true;
        GameEvent.InvokeUnitJump(caster);
    }

    protected override void CastExpiredOrCancelled()
    {
        base.CastExpiredOrCancelled();

        RequestAnimation(jumpEndTag);
    }

    protected override void CastEnded()
    {
        caster.isFloating = false;
        FinishScenario();
    }

    public override void Interrupt()
    {
        base.Interrupt();

        caster.isFloating = false;
    }
}
