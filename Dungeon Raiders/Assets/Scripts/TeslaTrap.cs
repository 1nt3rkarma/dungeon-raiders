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

    void Awake()
    {
        animHandler.onCastingStarted.AddListener(OnCastStarted);
        animHandler.onCast.AddListener(OnCast);
        animHandler.onCastingEnded.AddListener(OnCastEnded);
    }

    void OnDestroy()
    {
        animHandler.onCastingStarted.RemoveListener(OnCastStarted);
        animHandler.onCast.RemoveListener(OnCast);
        animHandler.onCastingEnded.RemoveListener(OnCastEnded);
    }

    void Start()
    {
        activationTimer = enableDelay;
    }

    void Update()
    {
        if (state == TrapStates.idling)
            if (activationTimer > 0)
                activationTimer -= Time.deltaTime;
            else
                Begin();
    }

    public void Begin()
    {
        animHandler.PlayActivationAnimation();
        state = TrapStates.casting;
    }

    private void OnCastStarted()
    {
        if (state == TrapStates.casting)
            StartCast();
    }
    public void StartCast()
    {

    }

    private void OnCast()
    {
        if (state == TrapStates.casting)
            Cast();
    }
    public void Cast()
    {
        var spawn = Instantiate(spawnPrefab, spawnPoint.position, Quaternion.identity);
        var ball = spawn.GetComponent<LightningBall>();
        if (ball)
        {
            ball.direction = Facing.backward;
            var block = Level.GetBlock(transform.position);
            //if (block == null)
            //{
            //    var colliders = Physics.OverlapSphere(transform.position, 0.5f);
            //    Debug.LogWarning($"CHEKING {colliders.Length} COLLIDER(S) NEARBY...");
            //    foreach (var c in colliders)
            //    {
            //        block = c.GetComponent<Block>();
            //        if (block != null)
            //            break;
            //    }
            //}
            if (block)
                ball.SwitchBlock(block);
            //else
            //    Debug.LogWarning($"COULDN'T FIND ANY BLOCKS AT {name} {transform.position}");
            spawn.transform.localPosition = Vector3.zero;
        }
        Destroy(spawn, 10);
    }

    private void OnCastEnded()
    {
        if (state == TrapStates.casting)
            EndCast();
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
}

public enum TrapStates { idling, casting }
