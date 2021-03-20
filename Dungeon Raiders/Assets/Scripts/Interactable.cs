using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public List<AudioClip> interactionSounds;

    public InteractableAnimator animator;
    public AudioSource audioSource;
    public ParticleSystem particleSystem;

    private bool interactionHappened = false;

    protected void OnTrigger(Collider other)
    {
        var hero = other.GetComponent<Hero>();
        if (hero != null && !interactionHappened)
            if (hero.isAlive)
                InteractWith(hero);
    }

    protected virtual void InteractWith(Hero hero)
    {
        interactionHappened = true;

        if (particleSystem)
            particleSystem.Stop();
        PlayRandomSound(interactionSounds);

        animator.RunPickUpAnimation(hero.transform);
        Destroy(gameObject, animator.jumpDuration);
    }

    protected void PlayRandomSound(List<AudioClip> sounds)
    {
        if (sounds.Count == 0)
            return;

        var i = Random.Range(0, sounds.Count);
        audioSource.PlayOneShot(sounds[i]);
    }
}
