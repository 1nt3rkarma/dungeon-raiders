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

    DoTweenTransformer localMover;
    private bool fallPredicted;

    private void Awake()
    {
        localMover = new DoTweenTransformer(transform);
    }

    private void OnDestroy()
    {
        localMover?.Stop();
    }

    public void PlayBubbles()
    {
        if (!fallPredicted)
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
        if (block != null)
        {
            if (hit)
                PlayBounceEffect();
            var nextBlock = GetRandomNextBlock();
            if (nextBlock != null)
            {
                fallPredicted = nextBlock.isEmpty;
                if (fallPredicted)
                    animator.SetTrigger("fall");
                MoveTo(nextBlock);
            }
        }
        else
            Destroy(gameObject);
    }
    public void Fall()
    {
        collider.enabled = false;
        Destroy(gameObject, 1f);
    }

    void MoveTo(Block targetBlock)
    {
        SwitchBlock(targetBlock);
        localMover.MoveLocal(Vector3.zero, moveDuration);
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
        Destroy(effect.gameObject, 3);
        Destroy(this.gameObject);
    }
}
