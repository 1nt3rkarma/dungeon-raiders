﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int price = 1;

    public Animator animator;

    void OnTriggerEnter(Collider other)
    {
        var hero = other.GetComponent<Hero>();
        if (hero != null)
        {
            Player.AddCoins(price);
            animator.SetTrigger("play");
            Destroy(gameObject, 1);
        }
    }
}