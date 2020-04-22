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
        // Ограничение на предельный ресурс
        switch (resource)
        {
            case HeroResources.health:
                if (Hero.singlton.health >= Hero.singlton.healthMax)
                    return;
                break;
            case HeroResources.mana:
                //if (Hero.singlton.mana == Hero.singlton.manaMax)
                //    return;
                break;
        }

        base.Use();

        switch (resource)
        {
            case HeroResources.health:
                if (full)
                    Hero.singlton.Heal(10);
                else
                    Hero.singlton.Heal(1);
                break;
            case HeroResources.mana:
                break;
        }
    }
}
