using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Common unit properties")]
    public float health = 1;
    public int healthMax = 1;

    public bool isAlive = true;
    public bool isBusy = false;

    public Animator animator { get => animHandler.animator; }
    public HeroAnimationHandler animHandler;

    public virtual void TakeDamage(float damage)
    {
        if (!isAlive)
            return;

        health -= damage;
        if (health <= 0)
            Die();
    }

    public virtual void Die()
    {
        isAlive = false;
    }

    protected IEnumerator WaitForAnimationEvent(AnimationEvents eventTag)
    {
        switch (eventTag)   
        {
            case AnimationEvents.start:
                while (!animHandler.animEventStart)
                    yield return null;
                animHandler.animEventCastStart = false;
                break;
            case AnimationEvents.end:
                while (!animHandler.animEventEnd)
                    yield return null;
                animHandler.animEventEnd = false;
                break;
            case AnimationEvents.leap:
                while (!animHandler.animEventLeap)
                    yield return null;
                animHandler.animEventLeap = false;
                break;
            case AnimationEvents.jumpStart:
                while (!animHandler.animEventJumpStart)
                    yield return null;
                animHandler.animEventJumpStart = false;
                break;
            case AnimationEvents.jumpEnd:
                while (!animHandler.animEventJumpEnd)
                    yield return null;
                animHandler.animEventJumpEnd = false;
                break;
            case AnimationEvents.castStart:
                while (!animHandler.animEventCastStart)
                    yield return null;
                animHandler.animEventCastStart = false;
                break;
            case AnimationEvents.cast:
                while (!animHandler.animEventCast)
                    yield return null;
                animHandler.animEventCast = false;
                break;
            case AnimationEvents.castEnd:
                while (!animHandler.animEventCastEnd)
                    yield return null;
                animHandler.animEventCastEnd = false;
                break;
        }
    }
}
