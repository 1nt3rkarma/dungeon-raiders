using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecroGatesAnimationHandler : GatesAnimationHandler
{
    public ParticleSystem tourchL;
    public Light lightL;

    public ParticleSystem tourchR;
    public Light lightR;

    public override void PlayToTheDungeonAnimation()
    {
        base.PlayToTheDungeonAnimation();

        tourchL.Play();
        lightL.enabled = true;

        tourchR.Play();
        lightR.enabled = true;
    }
}
