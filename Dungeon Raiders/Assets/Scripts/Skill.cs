using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviourExtended
{
    [Header("Common settings")]
    public SkillModes mode;

    public bool drawUIButton;
    public Sprite icon;
    public Sprite iconDisabled;

    public string animationTagDelay;
    public string animationTagCast;

    public float delayMoving = 0f;
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

    protected Unit caster;
    //[HideInInspector]
    public SkillStates state;

    public Action<float> onInterrupted = moveDelay => { };

    public void Initialize(Unit caster)
    {
        this.caster = caster;

        caster.animHandler.onCastingStarted.AddListener(OnCastStartedEvent);
        caster.animHandler.onCast.AddListener(OnCastEvent);
        caster.animHandler.onCastingEnded.AddListener(OnCastEndedEvent);

        charges = chargesMax;

        if (caster is Hero)
            onInterrupted += ((Hero)caster).DelayMove;
    }

    protected void Deinitialize()
    {
        caster.animHandler.onCastingStarted.RemoveListener(OnCastStartedEvent);
        caster.animHandler.onCast.RemoveListener(OnCastEvent);
        caster.animHandler.onCastingEnded.RemoveListener(OnCastEndedEvent);
    }

    // -- СКОПИРОВАТЬ ПОТОМКАМ !
    void Update()
    {
        OnUpdate();
    }

    void OnDestroy()
    {
        Deinitialize();
    }
    // --

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
                switch (mode) 
                {
                    case SkillModes.controlledTime:
                        if (((castTimer >= castTimeMax && !unlimited) || (castTimer >= castTimeMin && isCanceled)) && !isExpired)
                            CastExpiredOrCancelled();
                        break;
                    case SkillModes.fixedTime:
                        if (castTimer >= castTimeMax && !isExpired)
                            CastExpiredOrCancelled();
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

        onInterrupted.Invoke(delayMoving);
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
        //Debug.Log($"{name} IS ACTIVATED");
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

    private void OnCastStartedEvent()
    {
        if (state == SkillStates.casting)
            CastStarted();
    }
    protected virtual void CastStarted()
    {

    }

    private void OnCastEvent()
    {
        if (state == SkillStates.casting)
            Cast();
    }
    protected virtual void Cast()
    {

    }

    protected virtual void CastExpiredOrCancelled()
    {
        isExpired = true;
    }

    private void OnCastEndedEvent()
    {
        if (state == SkillStates.casting)
            switch (mode)
            {
                case SkillModes.singleAnimation:
                    CastEnded();
                    FinishScenario();
                    break;
                case SkillModes.controlledTime:
                case SkillModes.fixedTime:
                    CastEnded();
                    break;
            }
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
}

public enum SkillStates { idling, preparing, casting }

public enum SkillModes { singleAnimation, controlledTime, fixedTime }

