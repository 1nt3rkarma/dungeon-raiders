using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 1;
    public int healthMax = 1;

    public bool isAlive = true;

    public void TakeDamage()
    {
        if (!isAlive)
            return;

        health--;
        if (health <= 0)
            Die();
    }

    public void Die()
    {
        isAlive = false;

        StartCoroutine(DieRoutine());
    }

    IEnumerator DieRoutine()
    {
        yield return null;
        Destroy(gameObject, 1);
    }
}
