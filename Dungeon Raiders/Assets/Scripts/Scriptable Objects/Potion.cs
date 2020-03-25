using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Items/Potion")]
public class Potion : Item
{
    public bool full;

    public HeroResources resource;

    public override void Use()
    {
        base.Use();

        switch (resource)
        {
            case HeroResources.health:
                if (full)
                    Hero.singlton.AddHealth(10);
                else
                    Hero.singlton.AddHealth(1);
                break;
            case HeroResources.mana:
                break;
        }
    }
}
