using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaTrap : MonoBehaviour, IObstacle
{
    public float enableDelay = 4;
    public float activationInterval = 3;
    public float activationTimer = 0;

    public GameObject spawnPrefab;
    public Transform spawnPoint;

    public TrapAnimationHandler animHandler;

    public TrapStates state;

    void Start()
    {
        activationTimer = enableDelay;
    }

    void Update()
    {
        switch (state)
        {
            case TrapStates.idling:
                if (activationTimer > 0)
                    activationTimer -= Time.deltaTime;
                else
                    Begin();
                break;

            case TrapStates.casting:
                if (CatchFlag(AnimationEvents.castStart))
                    StartCast();

                if (CatchFlag(AnimationEvents.cast))
                    Cast();

                if (CatchFlag(AnimationEvents.castEnd))
                    EndCast();

                break;
        }
    }

    public void Begin()
    {
        animHandler.PlayActivationAnimation();
        state = TrapStates.casting;
    }

    public void StartCast()
    {

    }

    public void Cast()
    {
        var spawn = Instantiate(spawnPrefab, spawnPoint.position, Quaternion.identity);
        var ball = spawn.GetComponent<LightningBall>();
        if (ball)
        {
            ball.direction = Facing.backward;
            ball.SwitchBlock(Level.GetBlock(transform.position));
            spawn.transform.localPosition = Vector3.zero;
        }
        Destroy(spawn, 10);
    }

    public void EndCast()
    {
        Finish();
    }

    public void Finish()
    {
        activationTimer = activationInterval;
        state = TrapStates.idling;
    }

    bool CatchFlag(AnimationEvents flag)
    {
        switch (flag)   
        {
            case AnimationEvents.start:
                break;

            case AnimationEvents.castStart:
                if (animHandler.animEventCastStart)
                {
                    animHandler.animEventCastStart = false;
                    return true;
                }
                break;

            case AnimationEvents.cast:
                if (animHandler.animEventCast)
                {
                    animHandler.animEventCast = false;
                    return true;
                }
                break;

            case AnimationEvents.castEnd:
                if (animHandler.animEventCastEnd)
                {
                    animHandler.animEventCastEnd = false;
                    return true;
                }
                break;

            case AnimationEvents.end:
                break;
        }
        return false;
    }
}

public enum TrapStates { idling, casting }
