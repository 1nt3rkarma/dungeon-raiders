using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickPoint : MonoBehaviour
{
    public Vector3 randomRotationDeltas;
    public Vector3 randomPositionDeltas;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        Gizmos.DrawCube(transform.position, randomPositionDeltas * 2);
    }
}
