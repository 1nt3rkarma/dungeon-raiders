using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int price = 1;

    void OnTriggerEnter(Collider other)
    {
        var hero = other.GetComponent<Hero>();
        if (hero != null)
        {
            Player.AddCoins(price);
            Destroy(gameObject);
        }
    }
}
