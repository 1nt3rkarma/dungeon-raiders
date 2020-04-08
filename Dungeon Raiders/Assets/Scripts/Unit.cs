using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Common unit properties")]

    public float health = 1;
    public int healthMax = 1;

    public bool isDecaying = false;
    public float decayDelay = 3;

    public float meleeDamage = 1;
    public float attackInterval = 3;
    protected float attackTimer;
    public Transform attackPoint;
    public float attackAreaSize;
    public GameObject hitEffect;

    public Facing facing
    {
        get
        {
            if (transform.localEulerAngles.y == 0)
                return Facing.forward;
            else
                return Facing.backward;
        }
        set
        {
            Vector3 newEuler = new Vector3(0, 0, 0);
            if (value == Facing.backward)
                newEuler.y = 180;
            transform.localEulerAngles = newEuler;
        }
    }
    public int row { get => block.GetRowIndex(); }
    public int line { get => block.GetLineIndex(); }

    public Block block;
    public Block forwardBlock { get => Level.GetBlock(row + (int)facing, line); }
    public Block backwardBlock { get => Level.GetBlock(row - (int)facing, line); }
    public float leapDuration = 0.25f;

    public bool isAlive = true;
    public bool isBusy = false;
    public bool isFloating = false;
    public bool isLeaping = false;
    public bool isMoving = true;

    public Animator animator { get => animHandler.animator; }
    public UnitAnimationHandler animHandler;
    public AudioSource audioSource;

    public Coroutine castRoutine;
    public Coroutine leapRoutine;

    protected void OnStart()
    {
        block = Level.GetNearestBlock(transform.position);
    }

    #region Перемещение

    public virtual void Stop()
    {
        animHandler.SetMoveFlag(false);
        isMoving = false;
    }

    public virtual void Leap(LeapDirections direction)
    {
        if (!isBusy && isAlive)
            if (leapRoutine == null)
            {
                if (direction == LeapDirections.Left && line == 0)
                    return;
                if (direction == LeapDirections.Right && line == 2)
                    return;

                leapRoutine = StartCoroutine(LeapRoutine(direction));
                GameEvent.InvokeUnitLeap(this, direction);
            }
    }

    protected IEnumerator LeapRoutine(LeapDirections direction)
    {
        isBusy = true;

        if (direction == LeapDirections.Left)
            animHandler.PlayAnimation("leapL");
        if (direction == LeapDirections.Right)
            animHandler.PlayAnimation("leapR");

        // Ждем событие анимации - прыжок
        yield return WaitForAnimationEvent(AnimationEvents.jumpStart);
        isLeaping = true;

        StartCoroutine(LeapTransitionRoutine(direction));

        // Ждем событие анимации - конец прыжка
        yield return WaitForAnimationEvent(AnimationEvents.jumpEnd);
        isLeaping = false;

        leapRoutine = null;
        isBusy = false;

    }

    protected IEnumerator LeapTransitionRoutine(LeapDirections direction)
    {
        var speed = 1 / leapDuration;

        int sign = (int)direction;
        var targetPosition = transform.position + sign * transform.right;

        if (direction == LeapDirections.Left)
            while (transform.position.x > targetPosition.x)
            {
                transform.position -= transform.right * speed * Time.deltaTime;
                if (transform.position.x <= targetPosition.x)
                    break;
                yield return null;
            }
        else if (direction == LeapDirections.Right)
            while (transform.position.x < targetPosition.x)
            {
                transform.position += transform.right * speed * Time.deltaTime;
                if (transform.position.x >= targetPosition.x)
                    break;
                yield return null;
            }
        transform.position = targetPosition;
    }

    public void SwitchBlockTo(Block block)
    {
        this.block = block;
    }

    #endregion

    #region Нанесение и получение урона

    public void DecHealth(float amount)
    {
        SetHealth(health - amount);
    }

    public void AddHealth(float amount)
    {
        SetHealth(health + amount);
    }

    public void SetHealth(float amount)
    {
        health = amount;
        health = Mathf.Clamp(health, 0, healthMax);
    }

    public virtual void MeleeAttack()
    {
        if (!isBusy && isAlive && attackTimer <= 0)
            if (castRoutine == null)
            {
                Stop();
                GameEvent.InvokeUnitAttack(this);
                castRoutine = StartCoroutine(MeleeAttackRoutine());
                attackTimer = attackInterval;
            }
    }

    protected IEnumerator MeleeAttackRoutine()
    {
        isBusy = true;
        animHandler.PlayAnimation("attack");

        // Ждем событие анимации - начало анимации
        while (!animHandler.animEventCastStart)
            yield return null;
        animHandler.animEventCastStart = false;

        // Ждем событие анимации - применение
        while (!animHandler.animEventCast)
            yield return null;
        animHandler.animEventCast = false;
        CastMeleeDamage();

        // Ждем событие анимации - окончание анимации
        while (!animHandler.animEventCastEnd)
            yield return null;
        animHandler.animEventCastEnd = false;

        castRoutine = null;
        isBusy = false;
    }

    protected void CastMeleeDamage()
    {
        var objects = Physics.OverlapSphere(attackPoint.position,attackAreaSize);
        var impact = false;
        foreach (var obj in objects)
        {
            var enemy = obj.GetComponent<Unit>();
            if (enemy != null && enemy != this)
                enemy.TakeDamage(meleeDamage);

            if (!impact)
                impact = enemy != null;
        }
        if (impact)
        {
            animHandler.PlayImpactMeleeSound();
            var x = Random.Range(-0.3f, 0.3f);
            var y = Random.Range(-0.3f, 0.3f);
            var z = Random.Range(-0.3f, 0.3f);
            var randomShift = new Vector3(x, y, z);
            var effect = Instantiate(hitEffect, attackPoint.position + randomShift, Quaternion.identity);
        }
    }

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
        else
            StartCoroutine(HitRoutine());
    }

    protected IEnumerator HitRoutine()
    {
        isBusy = true;

        animHandler.ClearFalgs();

        animHandler.PlayAnimation("hit");

        // Ждем событие анимации - начало анимации
        yield return WaitForAnimationEvent(AnimationEvents.start);

        // Ждем событие анимации - конец анимации
        yield return WaitForAnimationEvent(AnimationEvents.end);

        isBusy = false;
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

        if (isDecaying)
            Destroy(gameObject, decayDelay);
    }

    #endregion

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

    protected virtual void InterruptRoutines()
    {
        if (castRoutine != null)
            StopCoroutine(castRoutine);
        castRoutine = null;

        if (leapRoutine != null)
            StopCoroutine(leapRoutine);
        leapRoutine = null;
    }

    public void PlayRandomSound(List<AudioClip> sounds)
    {
        if (sounds.Count == 0)
            return;
        audioSource.Stop();
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


public enum Facing { forward = 1, backward = -1 }

public enum DamageSources { common, fall, fire, frost, electro }