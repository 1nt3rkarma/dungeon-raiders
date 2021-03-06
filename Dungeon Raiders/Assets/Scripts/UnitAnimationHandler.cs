﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitAnimationHandler : MonoBehaviour
{
    public Unit unit;

    public Animator animator;

    public TrailRenderer weaponTrail;

    public string jumpTag = "jumpStart";
    public AnimationClipGroup jumpClips;

    [Header("Sounds")]

    public List<AudioClip> impactMeleeSounds;
    public List<AudioClip> impactBlockSounds;
    public List<AudioClip> attackSounds;

    public List<AudioClip> shootSounds;

    public List<AudioClip> deathSounds;

    public List<AudioClip> fallSounds;

    public List<AudioClip> footstepsSounds;

    [Header("Body parts and key points")]

    public List<MeshVariationGroup> randomMeshGroups;

    public List<GameObject> ammoView;

    [Tooltip("Части тела персонажа, которые отвалятся, например, при анимации смерти")]
    public List<Rigidbody> bodyparts;

    public GameObject remains;
    public float remainsDecay = 7;

    [Tooltip("Points to which projectiles stick when unit is defended by shield for example")]
    public List<StickPoint> defendStickPoints;
    [Tooltip("Points to which projectiles stick")]
    public List<StickPoint> defaultStickPoints;
    public List<Transform> stickedProjectiles;

    public UnityEvent onAnimationStarted;
    public UnityEvent onCastingStarted;
    public UnityEvent onCast;
    public UnityEvent onCastingEnded;
    public UnityEvent onAnimationEnded;

    [Header("Debugging")]

    public Transform chestPoint;
    public Transform headPoint;
    public Transform handRightPoint;
    public Transform handLeftPoint;
    public Transform overheadPoint;

    public void EventStart()
    {
        onAnimationStarted.Invoke();
    }
    public void EventCastStart()
    {
        onCastingStarted.Invoke();
    }
    public void EventCast()
    {
        onCast.Invoke();
    }
    public void EventCastEnd()
    {
        onCastingEnded.Invoke();
    }
    public void EventEnd()
    {
        onAnimationEnded.Invoke();
    }

    private void Start()
    {
        RandomizeAppearance();
    }

    public void UpdateAmmoView()
    {
        if (ammoView.Count > 0)
            for (int i = 0; i < ammoView.Count; i++)
                if (i < unit.ammo)
                    ammoView[i].SetActive(true);
                else
                    ammoView[i].SetActive(false);
    }

    void RandomizeAppearance()
    {
        foreach (var group in randomMeshGroups)
        {
            var chances = new List<float>();

            foreach (var variation in group.meshes)
                chances.Add(variation.basicChance);

            var index = MathUtils.GetRandomIndexFromListOfChances(chances);

            foreach (var variation in group.meshes)
                if (variation == group.meshes[index] && variation.meshObject != null)
                    variation.meshObject.SetActive(true);
                else if (variation.meshObject != null)
                    Destroy(variation.meshObject);
        }
    }

    public void ClearEventFalgs()
    {
        DisableWeaponTrail();
    }

    public void PlayAnimation(string tag)
    {
        AnimationClipGroup clips;

        switch (tag)    
        {
            case "jumpStart":
                clips = jumpClips;
                break;
            default:
                clips = null;
                break;
        }

        if (clips != null)
        {
            int index;
            if (clips.allowRepeat)
            {
                index = Random.Range(0, clips.count);
            }
            else
            {
                var indexes = new List<int>();
                for (int i = 0; i < clips.count; i++)
                    if (i != clips.excluded)
                        indexes.Add(i);
                index = indexes[Random.Range(0, indexes.Count)];
                clips.excluded = index;
            }
            animator.SetInteger("index", index);
        }

        //Debug.Log($"SETTING {tag} TRIGGER ON ANIMATOR {transform.root.name}");
        animator.SetTrigger(tag);
    }

    public void SetFlag(string flag, bool value = true)
    {
        //Debug.Log($"SETTING {flag} BOOL to {value} ON ANIMATOR {transform.root.name}");

        animator.SetBool(flag, value);
    }
    public void ClearFlag(string flag)
    {
        SetFlag(flag, false);
    }

    public void SetMoveFlag(bool flag)
    {
        animator.SetBool("isMoving", flag);
    }

    public void SetDefendFlag(bool flag)
    {
        animator.SetBool("isDefending", flag);
    }

    public void ShowRemains()
    {
        if (remains)
        {
            remains.gameObject.SetActive(true);

            if (unit is Hero)
                remains.transform.SetParent(null);
            else
                remains.transform.SetParent(unit.block.transform);

            Destroy(remains.gameObject, remainsDecay);
        }
    }

    public void ReleaseBodyparts()
    {
        foreach (var sticked in stickedProjectiles)
        {
            var body = sticked.GetComponent<Rigidbody>();
            if (body)
                Destroy(body);
        }

        foreach (var part in bodyparts)
        {
            if (part == null)
                continue;

            if (unit is Hero)
                part.transform.SetParent(null);
            else
                part.transform.SetParent(unit.block.transform);

            part.isKinematic = false;

            if(unit.isDecaying)
                Destroy(part.gameObject, remainsDecay);
        }
    }

    public void ReleaseSticked()
    {
        foreach (var sticked in stickedProjectiles)
        {
            sticked.SetParent(unit.block.transform);
            var body = sticked.GetComponent<Rigidbody>();
            if (body)
                body.isKinematic = false;
        }
        stickedProjectiles.Clear();
    }

    public void EnableWeaponTrail()
    {
        if (weaponTrail)
            weaponTrail.emitting = true;
    }

    public void DisableWeaponTrail()
    {
        if (weaponTrail)
            weaponTrail.emitting = false;
    }

    public void PlayImpactMeleeSound()
    {
        unit.PlayRandomSound(impactMeleeSounds);
    }

    public void PlayBlockSound()
    {
        unit.PlayRandomSound(impactBlockSounds);
    }

    public void PlayFootstepSound()
    {
        unit.PlayRandomSound(footstepsSounds);
    }

    public void PlayAttackSound()
    {
        unit.PlayRandomSound(attackSounds);
    }

    public void PlayShootSound()
    {
        unit.PlayRandomSound(shootSounds);
    }

    public void PlayFallSound()
    {
        unit.PlayRandomSound(fallSounds);
    }

    public void PlayDeathSound()
    {
        unit.PlayRandomSound(deathSounds);
    }
}

public enum UnitBodyPoints { origin, chest, head, handRight, handLeft, overhead}

public enum AnimationEvents { start, end, castStart, cast, castEnd, none }

[System.Serializable]
public class AnimationClipGroup : object
{
    [Range(1,5)][Tooltip("Количество анимационных клипов в группе")]
    public int count;

    [Tooltip("Повторы одинаковых анимаций разрешены?")]
    public bool allowRepeat;

    [HideInInspector] // Индекс последней проигранной анимации из этой группы
    public int excluded = -1;
}

[System.Serializable]
public class MeshVariationGroup
{
    public string label = "Body Variation";
    public List<MeshVariation> meshes;
}

[System.Serializable]
public class MeshVariation
{
    public string label = "A";
    public GameObject meshObject;
    [Range(0,1)]
    public float basicChance;
}
