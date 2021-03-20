using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrapAnimationHandler : MonoBehaviour
{
    public string activationAnimTrigger = "activate";

    public Animator animator;
    public ParticleSystem particleSystem;

    public List<AudioClip> activationSounds;
    public AudioSource audioSource;

    public UnityEvent onAnimationStarted;
    public UnityEvent onCastingStarted;
    public UnityEvent onCast;
    public UnityEvent onCastingEnded;
    public UnityEvent onAnimationEnded;

    public void PlayActivationAnimation()
    {
        PlayAnimation(activationAnimTrigger);
    }
    public void PlayActivationSound()
    {
        PlayRandomSound(activationSounds);
    }

    public void PlayAnimation(string trigger)
    {
        animator.SetTrigger(trigger);
    }
    public void PlayParticles()
    {
        if (particleSystem)
            particleSystem.Play();
    }
    public void PlayRandomSound(List<AudioClip> sounds)
    {
        if (sounds.Count == 0)
            return;

        var i = Random.Range(0, sounds.Count);
        audioSource.PlayOneShot(sounds[i]);
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
}
