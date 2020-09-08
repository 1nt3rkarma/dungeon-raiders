using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shards : MonoBehaviour
{
    public float decayTime = 5;

    public float esplosionForceMin = 100;
    public float esplosionForceMax = 1000;

    public float torqueMin = -3;
    public float torqueMax = 3;

    public float dispersionRadius = 0.25f;

    private void Start()
    {
        Destroy(this.gameObject, decayTime);
    }

    public void SimulateExplosion()
    {
        foreach (Transform shard in transform)
        {
            var shardBody = shard.GetComponent<Rigidbody>();

            var force = Random.Range(esplosionForceMin, esplosionForceMax);
            shardBody.AddExplosionForce(force, transform.position, 1);

            var x = Random.Range(torqueMin, torqueMax);
            var y = Random.Range(torqueMin, torqueMax);
            var z = Random.Range(torqueMin, torqueMax);
            shardBody.AddTorque(new Vector3(x,y,z), ForceMode.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dispersionRadius);
    }
}
