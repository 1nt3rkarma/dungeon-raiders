using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : Interactable
{
    public Item item;

    void OnTriggerEnter(Collider other)
    {
        OnTrigger(other);
    }

    protected override void InteractWith(Hero hero)
    {
        if (Player.inventory.Count < Player.inventorySize)
        {
            base.InteractWith(hero);

            GameEvent.InvokeHeroPicksItem(hero, item);

            Player.AddItem(item);
        }
    }
}

