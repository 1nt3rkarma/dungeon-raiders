using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBall : MonoBehaviour
{
    public float damage = 1;

    public float moveDuration = 1;
    public Facing direction = Facing.backward;

    public Block block;

    [Space]
    public Transform model;
    public ParticleSystem bubblesParticles;
    public Animator animator;
    public Collider collider;

    [Space]
    public ParticleSystem hitEffect;
    public AudioClip hitSound;
    [Range(0, 1)]
    public float hitSoundVolume = 0.3f;

    [Space]
    public AudioClip bounceSound;
    [Range(0,1)]
    public float bounceSoundVolume = 0.3f;
    public ParticleSystem bounceParticles;
    public Vector3 bounceParticlesPosition;
    public Vector3 bounceParticlesRotation;

    Coroutine moveRoutine;

    public void PlayBubbles()
    {
        bubblesParticles.Play();
    }

    public void PlayBounceEffect()
    {
        var hitEffect = Instantiate(bounceParticles,
            transform.position + bounceParticlesPosition,
            Quaternion.identity);
        hitEffect.transform.localEulerAngles = bounceParticlesRotation;
        hitEffect.Play();
        hitEffect.transform.SetParent(block.transform);

        var audioSource = hitEffect.GetComponent<AudioSource>();
        if (audioSource)
        {
            audioSource.volume = bounceSoundVolume;
            audioSource.PlayOneShot(bounceSound);
        }
    }

    public void BounceNoHit()
    {
        Bounce(false);
    }
    public void BounceHit()
    {
        Bounce(true);
    }
    public void Bounce(bool hit)
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        if (block != null)
        {
            if (!block.isEmpty)
            {
                if (hit)
                    PlayBounceEffect();
                var nextBlock = GetRandomNextBlock();
                if (nextBlock != null)
                    moveRoutine = StartCoroutine(MoveRoutine(nextBlock));
            }
            else
            {
                collider.enabled = false;
                animator.SetTrigger("fall");
                Destroy(gameObject, 0.5f);
            }
        }
        else
            Destroy(gameObject);
    }

    IEnumerator MoveRoutine(Block targetBlock)
    {
        SwitchBlock(targetBlock);
        Vector3 direction = (int)this.direction * transform.localPosition;
        float distance = Vector3.Magnitude(direction);
        float speed = distance / moveDuration;

        while (distance > 0.1f && targetBlock != null)
        {
            yield return null;
            direction = (int)this.direction * transform.localPosition;
            distance = Vector3.Magnitude(transform.localPosition);
            direction.Normalize();
            transform.localPosition += direction * speed * Time.deltaTime;
        }

        if (targetBlock == null)
            Destroy(gameObject);

        transform.localPosition = Vector3.zero;
        moveRoutine = null;
    }
    
    Block GetRandomNextBlock()
    {
        int rowIndex = this.block.GetRowIndex() + (int)direction;
        int lineIndex = 1;
        switch (this.block.GetLineIndex())  
        {
            case 0:
                lineIndex = Random.Range(0, 1+1);
                break;
            case 1:
                lineIndex = Random.Range(0, 2+1);
                break;
            case 2:
                lineIndex = Random.Range(1, 2+1);
                break;
        }

        Block block = Level.GetBlock(rowIndex, lineIndex);
        return block;
    }

    public void SwitchBlock(Block block)
    {
        this.block = block;
        transform.SetParent(block.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        var unit = other.GetComponent<Unit>();
        if (unit)
            Damage(unit);

    }

    void Damage(Unit unit)
    {
        var effect = Instantiate(hitEffect, model.position, Quaternion.identity);
        effect.Play();
        effect.transform.SetParent(block.transform);
        var audioSource = effect.GetComponent<AudioSource>();
        if (audioSource)
        {
            audioSource.volume = hitSoundVolume;
            audioSource.PlayOneShot(hitSound);
        }

        unit.TakeDamage(damage, DamageType.electro, this);
        Destroy(effect, 3);
        Destroy(gameObject);
    }
}
