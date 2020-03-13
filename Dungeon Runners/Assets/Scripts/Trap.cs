using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviourExtended
{
    [Header("Common trap settings")]

    public TrapActivationModes mode;

    public Collider trapAreaCollider;

    [Tooltip("Наносит ли урон Ловушка, если Герой подпрыгнул?")]
    public bool airDamage = false;
    [Tooltip("Наносит ли урон Ловушка один раз?")]
    public bool oneShot = false;

    public float activationInterval = 3;
    public float activationDelay = 4;
    public float activationDuration = 1;
    public float activationTimer = 0;

    public float damage;
    public float damageInterval = 1;
    public float damageTimer = 0;

    public bool isActive = false;
    public bool isExecuted = false;

    public virtual void Activate()
    {
        isActive = true;
        StartCoroutine(ActivationRoutine());
    }

    protected IEnumerator ActivationRoutine()
    {
        trapAreaCollider.enabled = true;
        yield return new WaitForSeconds(activationDuration);
        trapAreaCollider.enabled = false;
        Disactivate();
    }

    public virtual void Disactivate()
    {
        isActive = false;
        activationTimer = activationInterval;

        switch (mode)   
        {
            case TrapActivationModes.single:
                enabled = false;
                break;
            case TrapActivationModes.periodic:
                break;
            case TrapActivationModes.trigger:
                break;
            case TrapActivationModes.custom:
                break;
            default:
                break;
        }
    }

    protected void OnStartCall()
    {
        switch (mode)
        {
            case TrapActivationModes.single:
                activationTimer = activationDelay;
                break;
            case TrapActivationModes.periodic:
                activationTimer = activationDelay;
                break;
            case TrapActivationModes.trigger:
                activationTimer = activationDelay;
                break;
            case TrapActivationModes.custom:
                break;
            default:
                break;
        }
    }

    protected void OnTriggerEnterCall(Collider other)
    {
        TryCastDamage(other);
    }

    protected void OnTriggerStayCall(Collider other)
    {
        TryCastDamage(other);
    }

    protected void OnUpdateCall()
    {
        if (!isExecuted && !isActive)
            switch (mode)
            {
                case TrapActivationModes.single:
                    if (damageTimer > 0)
                        damageTimer -= Time.deltaTime;

                    if (activationTimer > 0)
                        activationTimer -= Time.deltaTime;
                    break;
                case TrapActivationModes.periodic:
                    if (damageTimer > 0)
                        damageTimer -= Time.deltaTime;

                    if (activationTimer > 0)
                        activationTimer -= Time.deltaTime;
                    else
                        Activate();
                    break;
                case TrapActivationModes.trigger:
                    break;
                case TrapActivationModes.custom:
                    break;
            }
    }

    protected void TryCastDamage(Collider collider)
    {
        if (!isExecuted && damageTimer <= 0)
        {
            Hero hero = collider.GetComponent<Hero>();
            if (hero)
                if (!hero.isFloating || airDamage )
                    CastDamage(hero);
        }
    }

    public virtual void CastDamage(Hero hero)
    {
        if (oneShot)
            isExecuted = true;
        else
            damageTimer += damageInterval;

        hero.TakeDamage(damage);
    }
}

public enum TrapActivationModes { single, periodic, trigger, custom }
