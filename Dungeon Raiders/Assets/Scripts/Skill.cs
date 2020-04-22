using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviourExtended
{
    public SkillModes mode;

    public bool drawUIButton;
    public Sprite icon;
    public Sprite iconDisabled;

    public string animationTagDelay;
    public string animationTagCast;

    public bool isOccupying = true;
    public bool requireStop = false;
    public bool isInterruptable;

    public float cooldown;
    [HideInInspector]
    public float cooldownTimer;

    [Range(1,10)]
    public int chargesMax = 1;
    [HideInInspector]
    public int charges;

    public float prepareTime;
    [HideInInspector]
    public float preparingTimer;

    public float castTimeMin = 1;
    public float castTimeMax = 1.5f;
    public bool unlimited;
    public bool freeCancel;

    [HideInInspector]
    public float castTimer;
    [HideInInspector]
    public bool isCanceled;
    [HideInInspector]
    public bool isExpired;


    [HideInInspector]
    public Unit caster;
    [HideInInspector]
    public SkillStates state;

    // Не забудь скопировать потомкам
    void Update()
    {
        OnUpdate();
    }

    protected virtual void OnUpdate()
    {
        switch (state)  
        {
            case SkillStates.idling:
                if (cooldownTimer > 0)
                    cooldownTimer -= Time.deltaTime;
                else if (charges < 1)
                    charges = 1;
                break;

            case SkillStates.preparing:
                if (preparingTimer > 0)
                    preparingTimer -= Time.deltaTime;
                else
                    BeginScenario();
                break;

            case SkillStates.casting:
                castTimer += Time.deltaTime;

                if (CatchAnimationFlag(AnimationEvents.castStart))
                    CastStarted();

                if (CatchAnimationFlag(AnimationEvents.cast))
                    Cast();

                switch (mode) 
                {
                    case SkillModes.singleAnimation:
                        if (CatchAnimationFlag(AnimationEvents.castEnd))
                            FinishScenario();
                        break;
                    case SkillModes.controlledTime:
                        if (((castTimer >= castTimeMax && !unlimited) || (castTimer >= castTimeMin && isCanceled)) && !isExpired)
                            CastExpiredOrCancelled();
                        if (CatchAnimationFlag(AnimationEvents.castEnd))
                            CastEnded();
                        break;
                    case SkillModes.fixedTime:
                        if (castTimer >= castTimeMax && !isExpired)
                            CastExpiredOrCancelled();
                        if (CatchAnimationFlag(AnimationEvents.castEnd))
                            CastEnded();
                        break;
                }
                break;
        }
    }

    public void Use()
    {
        if (charges > 0 && state == SkillStates.idling)
            Activate();
    }

    public virtual void Interrupt()
    {
        if (state != SkillStates.casting)
            return;

        charges--;
        castTimer = 0;
        preparingTimer = 0;

        if (isCanceled && freeCancel)
            cooldownTimer = 0;
        else
            cooldownTimer = cooldown;

        isCanceled = false;
        isExpired = false;
        state = SkillStates.idling;

        if (isOccupying && caster.isBusy)
            caster.isBusy = false;
    }

    public virtual void Cancel()
    {
        if (!isCanceled && mode == SkillModes.controlledTime && state == SkillStates.casting)
        {
            //Debug.Log("Флаг прерывания прыжка установлен");
            isCanceled = true;
        }
    }

    protected void Activate()
    {
        //Debug.Log($"{name} активирован");
        if (prepareTime > 0)
        {
            state = SkillStates.preparing;
            preparingTimer = prepareTime;
            RequestAnimation(animationTagDelay);
        }
        else
            BeginScenario();
    }

    protected virtual void BeginScenario()
    {
        state = SkillStates.casting;

        if (requireStop)
            caster.Stop();

        if (isOccupying)
            caster.isBusy = true;

        castTimer = 0;
    }

    protected virtual void CastStarted()
    {

    }

    protected virtual void Cast()
    {

    }

    protected virtual void CastExpiredOrCancelled()
    {
        isExpired = true;
    }

    protected virtual void CastEnded()
    {

    }

    protected virtual void FinishScenario()
    {
        Interrupt();
    }

    protected void RequestAnimation(string tag)
    {
        caster.animHandler.PlayAnimation(tag);
    }

    protected void RequestSetAnimationFlag(string flag)
    {
        caster.animHandler.SetFlag(flag);
    }
    protected void RequestClearAnimationFlag(string flag)
    {
        caster.animHandler.ClearFlag(flag);
    }

    protected IEnumerator WaitForAnimationEvent(AnimationEvents eventTag)
    {
        switch (eventTag)
        {
            case AnimationEvents.start:
                while (!caster.animHandler.animEventStart)
                    yield return null;
                caster.animHandler.animEventStart = false;
                break;
            case AnimationEvents.end:
                while (!caster.animHandler.animEventEnd)
                    yield return null;
                caster.animHandler.animEventEnd = false;
                break;
            case AnimationEvents.castStart:
                while (!caster.animHandler.animEventCastStart)
                    yield return null;
                caster.animHandler.animEventCastStart = false;
                break;
            case AnimationEvents.cast:
                while (!caster.animHandler.animEventCast)
                    yield return null;
                caster.animHandler.animEventCast = false;
                break;
            case AnimationEvents.castEnd:
                while (!caster.animHandler.animEventCastEnd)
                    yield return null;
                caster.animHandler.animEventCastEnd = false;
                break;
        }
    }

    protected bool CatchAnimationFlag(AnimationEvents eventTag)
    {
        switch (eventTag)
        {
            default:
                return false;

            case AnimationEvents.start:
                if (caster.animHandler.animEventStart)
                {
                    caster.animHandler.animEventStart = false;
                    return true;
                }
                else
                    return false;

            case AnimationEvents.end:
                if (caster.animHandler.animEventEnd)
                {
                    caster.animHandler.animEventEnd = false;
                    return true;
                }
                else
                    return false;

            case AnimationEvents.castStart:
                if (caster.animHandler.animEventCastStart)
                {
                    caster.animHandler.animEventCastStart = false;
                    return true;
                }
                else
                    return false;

            case AnimationEvents.cast:
                if (caster.animHandler.animEventCast)
                {
                    caster.animHandler.animEventCast = false;
                    return true;
                }
                else
                    return false;

            case AnimationEvents.castEnd:
                if (caster.animHandler.animEventCastEnd)
                {
                    caster.animHandler.animEventCastEnd = false;
                    return true;
                }
                else
                    return false;
        }
    }
}

public enum SkillStates { idling, preparing, casting }

public enum SkillModes { singleAnimation, controlledTime, fixedTime }

