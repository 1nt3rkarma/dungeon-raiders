﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviourExtended
{
    [Header("Common trap settings")]

    public TrapActivationModes mode;

    public Collider trapAreaCollider;
    public AudioSource audioSource;

    public List<AudioClip> activationSounds;
    public List<AudioClip> damageSounds;

    public GameObject damageEffect;

    [Tooltip("Наносит ли урон Ловушка, если Герой подпрыгнул?")]
    public bool airDamage = false;
    [Tooltip("Наносит ли урон Ловушка один раз?")]
    public bool oneShot = false;

    public float enableDelay = 4;
    public float activationInterval = 3;
    public float activationDelay = 0.1f;
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
        PlayRandomSound(activationSounds);
        StartCoroutine(ActivationRoutine());
    }

    protected IEnumerator ActivationRoutine()
    {
        yield return new WaitForSeconds(activationDelay);
        trapAreaCollider.enabled = true;
        yield return new WaitForSeconds(activationDuration);
        trapAreaCollider.enabled = false;
        Deactivate();
    }

    public virtual void Deactivate()
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
                activationTimer = enableDelay;
                break;
            case TrapActivationModes.periodic:
                activationTimer = enableDelay;
                break;
            case TrapActivationModes.trigger:
                activationTimer = enableDelay;
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
            Unit target = collider.GetComponent<Unit>();
            if (target)
                if (!target.isFloating || airDamage )
                    CastDamage(target);
        }
    }

    public virtual void CastDamage(Unit target)
    {
        //Debug.Log("Ловушка нанесла урон");
        PlayRandomSound(damageSounds);
        var position = target.GetUnitPoint(UnitBodyPoints.chest).position;
        var effect = Instantiate(damageEffect, position, Quaternion.identity);
        effect.transform.localEulerAngles = new Vector3(0, 180, 0);

        if (oneShot)
            isExecuted = true;
        else
            damageTimer += damageInterval;

        target.TakeDamage(damage, this);
    }

    public void PlayRandomSound(List<AudioClip> sounds)
    {
        if (sounds.Count == 0)
            return;

        var i = Random.Range(0, sounds.Count);
        audioSource.PlayOneShot(sounds[i]);
    }
}

public enum TrapActivationModes { single, periodic, trigger, custom }
