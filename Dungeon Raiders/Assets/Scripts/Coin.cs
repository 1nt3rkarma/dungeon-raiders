using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Interactable
{
    public int price = 1;

    void OnTriggerEnter(Collider other)
    {
        OnTrigger(other);
    }

    protected override void InteractWith(Hero hero)
    {
        base.InteractWith(hero);

        Player.AddCoins(price);
    }
}
