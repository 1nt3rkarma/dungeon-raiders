using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IObstacle
{
    public string unitType;
    public Unit UnitType => GameResourcesAccess.ResourceFile.GetUnitType(unitType);

    [Header("Common unit properties")]

    public List<Resource> resources;

    public Resource Health { get; private set; }
    public Resource Mana { get; private set; }
    public Resource Faith { get; private set; }
    public Resource Fury { get; private set; }
    public Resource Supply { get; private set; }
    public Resource GetResource(UnitResourceTypes ofType)
    {
        return resources.FirstOrDefault(r => r.type == ofType);
    }

    public bool isDecaying = false;
    public float decayDelay = 3;

    public float leapDuration = 0.25f;   

    [Space][Header("Attack settings")]

    public float meleeDamage = 1;
    public float attackInterval = 3;
    public Transform attackPoint;
    public float attackAreaSize;
    public GameObject hitEffect;
    public float attackRange { get => Mathf.Abs(attackPoint.localPosition.z); }

    public Projectile missilePref;
    public Transform shootPoint;
    public int ammoMax;
    public int ammo;
    public bool unlimitedAmmo;

    [Space][Header("Components")]
    public UnitAnimationHandler animHandler;
    public Animator animator { get => animHandler.animator; }
    public AudioSource audioSource;
    public Collider collider;

    public List<Skill> skills;
    [HideInInspector]
    public Attack attackSkill;
    [HideInInspector]
    public Shoot shootSkill;
    [HideInInspector]
    public Jump jumpSkill;
    [HideInInspector]
    public Leap leapSkill;

    [Header("Debugging")]
    public Block block;
    public Block forwardBlock { get => Level.GetBlock(row + (int)facing, line); }
    public Block backwardBlock { get => Level.GetBlock(row - (int)facing, line); }
    public Vector3 position { get => transform.position; }

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

    public bool isAlive = true;
    public bool isBusy = false;
    public bool isDefending = false;
    public bool isFloating = false;
    public bool isLeaping = false;
    public bool isMoving = true;

    protected void OnAwake()
    {
        foreach (var skill in skills)
        {
            skill.Initialize(this);

            if (skill is Attack)
                attackSkill = (Attack) skill;

            if (skill is Shoot)
                shootSkill = (Shoot)skill;

            if (skill is Jump)
                jumpSkill = (Jump) skill;

            if (skill is Leap)
                leapSkill = (Leap) skill;
        }

        ammo = ammoMax;

        foreach (var r in resources)
        {
            switch (r.type)
            {
                case UnitResourceTypes.Health:
                    Health = r;
                    break;
                case UnitResourceTypes.Mana:
                    Mana = r;
                    break;
                case UnitResourceTypes.Faith:
                    Faith = r;
                    break;
                case UnitResourceTypes.Fury:
                    Fury = r;
                    break;
                default:
                    Debug.LogWarning("Unknown resource type");
                    break;
            }

            if (r.type != UnitResourceTypes.Health && Supply == null)
                Supply = r;

            r.Value = r.InitialValue;
        }

        //if(Supply != null)
        //    Debug.LogWarning($"{name} supply resource type is {Supply.type}");
        //else
        //    Debug.LogWarning($"{name} doesn't have any secondary resorce");

    }

    protected void OnStart()
    {
        SetBlockPosition(Level.GetBlock(transform.position));
    }

    protected void OnFixedUpdate()
    {
        foreach (var r in resources)
            if (r.RegenerationRate > 0)
                r.Value += r.RegenerationRate * Time.fixedDeltaTime;
    }

    #region Перемещение

    public void SwitchBlockTo(Block block)
    {
        this.block = block;
    }

    public void SetBlockPosition(Block block)
    {
        SwitchBlockTo(block);
        transform.position = block.transform.position;
    }

    public virtual void Stop()
    {
        animHandler.SetMoveFlag(false);
        isMoving = false;
        //Debug.Log($"{name} остановился");
    }

    #endregion

    #region Взаимодействие со способностями

    public void Leap(LeapDirections direction)
    {
        if (!isBusy && isAlive && leapSkill != null)
        {
            if (direction == LeapDirections.Left && line == 0)
                return;
            if (direction == LeapDirections.Right && line == 2)
                return;

            leapSkill.direction = direction;
            leapSkill.Use();
        }
    }

    public void Jump()
    {
        Use(jumpSkill);
    }

    public void BreakJump()
    {
        Cancel(jumpSkill);
    }

    public void Use(Skill skill)
    {
        if (!isBusy && isAlive && skills.Contains(skill))
            skill.Use();
    }

    public void Cancel(Skill skill)
    {
        if (isAlive && skills.Contains(skill))
            if (skill.state == SkillStates.casting)
                skill.Cancel();
    }

    #endregion

    #region Нанесение и получение урона

    public void Heal(float amount)
    {
        Health.Value += amount;

        animHandler.ReleaseSticked();
    }

    public void Attack()
    {
        if (!isBusy && isAlive && attackSkill != null)
            attackSkill.Use();
    }

    public void CastDamage()
    {
        GameEvent.InvokeUnitAttack(this);

        var units = GetUnitsInRange();

        var withSound = true;
        var impact = false;
        foreach (var unit in units)
        {
            if (unit.isAlive)
            {
                unit.TakeDamage(meleeDamage, this);

                if (withSound && unit.isDefending)
                    withSound = false;

                if (!impact)
                    impact = unit != null;
            }
        }
        if (impact)
        {
            var x = Random.Range(-0.3f, 0.3f);
            var y = Random.Range(-0.3f, 0.3f);
            var z = Random.Range(-0.3f, 0.3f);
            var randomShift = new Vector3(x, y, z);
            var effect = Instantiate(hitEffect, attackPoint.position + randomShift, Quaternion.identity);

            var audioSource = effect.GetComponent<AudioSource>();
            if (audioSource && withSound)
            {
                var sound = GetRandomSound(animHandler.impactMeleeSounds);
                if (sound != null)
                    audioSource.PlayOneShot(sound);
            }
        }
    }

    public void Shoot()
    {
        if (!isBusy && isAlive && shootSkill != null && (ammo > 0 || unlimitedAmmo))
            shootSkill.Use();
    }

    public void CastMissile()
    {
        var missile = Instantiate(missilePref, shootPoint.position, Quaternion.identity);
        switch (facing)
        {
            case Facing.forward:
                missile.transform.localEulerAngles = new Vector3(0, 0, 0);
                break;
            case Facing.backward:
                missile.transform.localEulerAngles = new Vector3(0, 180, 0);
                break;
        }
        missile.caster = this;

        ammo--;
        animHandler.UpdateAmmoView();
    }

    public virtual void TakeDamage(float damage, Object source)
    {
        //Debug.Log($"{name} получает {damage} ед. урона от {source.name} (тип {source.GetType()})");

        TakeDamage(damage, DamageType.common, source);
    }

    public virtual void TakeDamage(float damage, DamageType type, Object source)
    {
        if (!isAlive)
            return;

        if (!(isDefending && source is Unit && type == DamageType.common))
            Health.Value -= damage;

        if (Health.Value > 0)
            StartCoroutine(HitRoutine());
        else
            Die(type, source);

        GameEvent.InvokeUnitDamage(this, damage, type, source);
    }

    protected IEnumerator HitRoutine()
    {
        InterruptRoutines();
        isBusy = true;

        animHandler.ClearEventFalgs();
        animHandler.PlayAnimation("hit");
        if (isDefending)
            PlayRandomSound(animHandler.impactBlockSounds);

        // Ждем событие анимации - начало анимации
        //yield return WaitForAnimationEvent(AnimationEvents.start);

        // Ждем событие анимации - конец анимации
        yield return WaitForAnimationEvent(AnimationEvents.end);

        isBusy = false;
    }

    public virtual void Die(DamageType type, Object source)
    {
        Stop();
        isAlive = false;

        collider.enabled = false;
        animHandler.ClearEventFalgs();
        switch (type)
        {
            case DamageType.fall:
                animHandler.PlayAnimation("dieFall");
                break;
            case DamageType.fire:
                animHandler.PlayAnimation("dieFire");
                break;
            case DamageType.frost:
                animHandler.PlayAnimation("dieFrost");
                break;
            case DamageType.electro:
                animHandler.PlayAnimation("dieElectro");
                break;
            default:
                animHandler.PlayAnimation("die");
                break;
        }
        GameEvent.InvokeUnitDie(this, type, source);

        if (isDecaying)
            Destroy(gameObject, decayDelay);
    }

    #endregion

    protected IEnumerator WaitForAnimationEvent(AnimationEvents eventTag)
    {
        switch (eventTag)
        {
            case AnimationEvents.start:
                animHandler.onAnimationStarted.AddListener(()=> AnimationEventHappened = true);
                break;
            case AnimationEvents.end:
                animHandler.onAnimationEnded.AddListener(() => AnimationEventHappened = true);
                break;
            case AnimationEvents.castStart:
                animHandler.onCastingStarted.AddListener(() => AnimationEventHappened = true);
                break;
            case AnimationEvents.cast:
                animHandler.onCast.AddListener(() => AnimationEventHappened = true);
                break;
            case AnimationEvents.castEnd:
                animHandler.onAnimationEnded.AddListener(() => AnimationEventHappened = true);
                break;
        }

        while (!AnimationEventHappened)
            yield return null;
        AnimationEventHappened = false;
    }
    private bool AnimationEventHappened = false;

    protected void InterruptRoutines()
    {
        foreach (var skill in skills)
            if (skill.isInterruptable)
                skill.Interrupt();
    }

    public AudioClip GetRandomSound(List<AudioClip> sounds)
    {
        if (sounds.Count == 0)
            return null;

        var i = Random.Range(0, sounds.Count);
        return sounds[i];
    }

    public void PlayRandomSound(List<AudioClip> sounds)
    {
        var sound = GetRandomSound(sounds);
        if (sound != null)
            audioSource.PlayOneShot(sound);
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

    public List<Unit> GetUnitsInRange()
    {
        var units = new List<Unit>();
        var objects = Physics.OverlapSphere(attackPoint.position, attackAreaSize);
        foreach (var obj in objects)
        {
            var unit = obj.GetComponent<Unit>();
            if (unit != null && unit != this)
                units.Add(unit);
        }
        return units;
    }

    public List<GameObject> GetObstaclesInRange()
    {
        var obstacles = new List<GameObject>();

        var colliders = Physics.OverlapSphere(attackPoint.position, attackAreaSize);
        foreach (var collider in colliders)
        {
            var obstacle = collider.GetComponent<IObstacle>();
            if (obstacle != null && collider != this.gameObject)
                obstacles.Add(collider.gameObject);
        }
        return obstacles;
    }
}


public enum Facing { forward = 1, backward = -1 }

public enum DamageType { common, fall, fire, frost, electro }

public interface IObstacle { }

public enum UnitResourceTypes { Health, Mana, Faith, Fury }

[System.Serializable]
public class Resource
{
    [SerializeField] float value = 1;

    [Tooltip("Purely for inspector")]
    public string label;

    public UnitResourceTypes type;
    public float Value
    {
        get => value;

        set => this.value = Mathf.Clamp(value, 0, MaxValue);
    }
    public int MaxValue = 1;
    public float Percentage => value / MaxValue;
    public float RegenerationRate = 0.1f;
    public float InitialValue = 1;
}