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
    public AudioSource audioSource;

    public virtual void TakeDamage(float damage)
    {
        TakeDamage(damage, DamageSources.common);
    }

    public virtual void TakeDamage(float damage, DamageSources source)
    {
        if (!isAlive)
            return;

        health -= damage;
        if (health <= 0)
            Die(source);
    }

    public virtual void Die()
    {
        Die(DamageSources.common);
    }

    public virtual void Die(DamageSources source)
    {
        isAlive = false;

        animHandler.ClearFalgs();
        switch (source)
        {
            case DamageSources.fall:
                animHandler.PlayAnimation("dieFall");
                break;
            case DamageSources.fire:
                animHandler.PlayAnimation("dieFire");
                break;
            case DamageSources.frost:
                animHandler.PlayAnimation("dieFrost");
                break;
            case DamageSources.electro:
                animHandler.PlayAnimation("dieElectro");
                break;
            default:
                animHandler.PlayAnimation("die");
                break;
        }
    }

    protected IEnumerator WaitForAnimationEvent(AnimationEvents eventTag)
    {
        switch (eventTag)   
        {
            case AnimationEvents.start:
                while (!animHandler.animEventStart)
                    yield return null;
                animHandler.animEventStart = false;
                break;
            case AnimationEvents.end:
                while (!animHandler.animEventEnd)
                    yield return null;
                animHandler.animEventEnd = false;
                break;
            //case AnimationEvents.leap:
            //    while (!animHandler.animEventLeap)
            //        yield return null;
            //    animHandler.animEventLeap = false;
            //    break;
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

    public void PlayRandomSound(List<AudioClip> sounds)
    {
        if (sounds.Count == 0)
            return;

        var i = Random.Range(0, sounds.Count);
        audioSource.PlayOneShot(sounds[i]);
    }

    public Transform GetUnitPoint(UnitBodyPoints point)
    {
        switch (point)  
        {
            case UnitBodyPoints.chest:
                return animHandler.chestPoint;
            case UnitBodyPoints.head:
                return animHandler.headPoint;
            case UnitBodyPoints.handRight:
                return animHandler.handRightPoint;
            case UnitBodyPoints.handLeft:
                return animHandler.handLeftPoint;
            case UnitBodyPoints.overhead:
                return animHandler.overheadPoint;
            default:
                return animHandler.transform;
        }
    }
}


public enum DamageSources { common, fall, fire, frost, electro }