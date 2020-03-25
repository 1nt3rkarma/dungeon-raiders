using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public List<AudioClip> interactionSounds;

    public Animator animator;
    public AudioSource audioSource;
    public ParticleSystem particleSystem;

    protected void OnTrigger(Collider other)
    {
        var hero = other.GetComponent<Hero>();
        if (hero != null)
            if (hero.isAlive)
                InteractWith(hero);
    }

    protected virtual void InteractWith(Hero hero)
    {
        if (particleSystem)
            particleSystem.Stop();
        PlayRandomSound(interactionSounds);
        animator.SetTrigger("pick");

        Destroy(gameObject, 1);
    }

    protected void PlayRandomSound(List<AudioClip> sounds)
    {
        if (sounds.Count == 0)
            return;

        var i = Random.Range(0, sounds.Count);
        audioSource.PlayOneShot(sounds[i]);
    }
}
