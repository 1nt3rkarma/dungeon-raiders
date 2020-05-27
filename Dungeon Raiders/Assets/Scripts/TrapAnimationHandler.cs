using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapAnimationHandler : MonoBehaviour
{
    public string activationAnimTrigger = "activate";

    public Animator animator;
    public ParticleSystem particleSystem;

    public List<AudioClip> activationSounds;
    public AudioSource audioSource;

    public bool animEventCastStart;
    public bool animEventCast;
    public bool animEventCastEnd;

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
        animEventCastStart = true;
    }
    public void EventCast()
    {
        animEventCast = true;
    }
    public void EventCastEnd()
    {
        animEventCastEnd = true;
    }
}
