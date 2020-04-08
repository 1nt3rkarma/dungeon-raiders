using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimationHandler : MonoBehaviour
{
    public Unit unit;

    public Animator animator;

    public TrailRenderer weaponTrail;

    public AnimationClipGroup jumpClips;

    [Header("Sounds")]

    public List<AudioClip> impactMeleeSounds;

    public List<AudioClip> attackSounds;

    public List<AudioClip> deathSounds;

    public List<AudioClip> fallSounds;

    [Header("Body parts and key points")]

    public List<MeshVariationGroup> randomMeshGroups;

    [Tooltip("Части тела персонажа, которые отвалятся, например, при анимации смерти")]
    public List<Rigidbody> bodyparts;

    [Header("Debugging")]

    public Transform chestPoint;
    public Transform headPoint;
    public Transform handRightPoint;
    public Transform handLeftPoint;
    public Transform overheadPoint;

    public bool animEventStart = false;
    public bool animEventEnd = false;

    //public bool animEventLeap = false;

    public bool animEventJumpStart = false;
    public bool animEventJumpEnd = false;

    public bool animEventCastStart = false;
    public bool animEventCast= false;
    public bool animEventCastEnd = false;

    public void EventStart()
    {
        animEventStart = true;
    }
    public void EventEnd()
    {
        animEventEnd = true;
    }

    public void EventJumpStart()
    {
        //Debug.Log("Прыжок начался");
        animEventJumpStart = true;
    }
    public void EventJumpEnd()
    {
        //Debug.Log("Прыжок окончен");
        animEventJumpEnd = true;
    }

    public void EventCastStart()
    {
        //Debug.Log("НАЧАЛАСЬ анимация применения способности");
        animEventCastStart = true;
    }
    public void EventCast()
    {
        //Debug.Log("Анимация применения способности СРАБОТАЛА");
        animEventCast = true;
    }
    public void EventCastEnd()
    {
        //Debug.Log("Анимация применения способности ЗАКОНЧИЛАСЬ");
        animEventCastEnd = true;
    }

    private void Start()
    {
        RandomizeAppearance();
    }

    void RandomizeAppearance()
    {
        foreach (var group in randomMeshGroups)
        {
            var chances = new List<float>();

            foreach (var variation in group.meshes)
                chances.Add(variation.basicChance);

            var index = MathUtilities.GetRandomIndexFromListOfChances(chances);

            foreach (var variation in group.meshes)
                if (variation == group.meshes[index])
                    variation.meshObject.SetActive(true);
                else
                    Destroy(variation.meshObject);
        }
    }

    public void ClearFalgs()
    {
        animEventStart = false;
        animEventEnd = false;
        animEventJumpStart = false;
        animEventJumpEnd = false;
        //animEventLeap = false;
        animEventCastStart = false;
        animEventCast = false;
        animEventCastEnd = false;
    }

    public void PlayAnimation(string tag)
    {
        AnimationClipGroup clips;

        switch (tag)    
        {
            case "jump":
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

        animator.SetTrigger(tag);
    }

    public void SetMoveFlag(bool flag)
    {
        animator.SetBool("isMoving", flag);
    }

    public void ReleaseBodyparts()
    {
        foreach (var part in bodyparts)
        {
            part.transform.SetParent(null);
            part.isKinematic = false;
        }
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

    public void PlayAttackSound()
    {
        unit.PlayRandomSound(attackSounds);
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

public enum AnimationEvents { start, end, leap, jumpStart, jumpEnd, castStart, cast, castEnd }

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
