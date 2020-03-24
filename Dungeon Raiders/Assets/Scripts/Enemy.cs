using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    public override void Die()
    {
        base.Die();

        StartCoroutine(DieRoutine());
    }

    IEnumerator DieRoutine()
    {
        yield return null;
        Destroy(gameObject, 1);
    }
}
