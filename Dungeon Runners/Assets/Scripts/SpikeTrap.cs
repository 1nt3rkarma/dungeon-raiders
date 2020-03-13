using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : Trap
{
    [Header("SpikeTrap settings")]

    public List<Animator> spikesAnimators;
    public string spikeAnimTag = "play";

    public override void Activate()
    {
        base.Activate();
        foreach (var animator in spikesAnimators)
            animator.SetTrigger(spikeAnimTag);
    }

    private void Start()
    {
        OnStartCall();
    }

    private void Update()
    {
        OnUpdateCall();
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterCall(other);
    }

    private void OnTriggerStay(Collider other)
    {
        OnTriggerStayCall(other);
    }
}
