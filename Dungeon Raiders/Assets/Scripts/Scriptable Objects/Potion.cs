using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Items/Potion")]
public class Potion : Item
{
    public bool full;
    public UnitResourceTypes resourceType;

    private Hero hero => Hero.singlton;

    public override void Use()
    {
        var resource = hero.GetResource(resourceType);

        // Ограничение на предельный или отсутствующий ресурс ресурс
        if (resource == null || resource.Value >= resource.MaxValue)
            return;

        base.Use();

        switch (resourceType)
        {
            case UnitResourceTypes.Health:
                if (full)
                    hero.Heal(hero.Health.MaxValue);
                else
                    hero.Heal(1);
                break;
            case UnitResourceTypes.Mana:
            case UnitResourceTypes.Faith:
            case UnitResourceTypes.Fury:
                if (full)
                    resource.Value = resource.MaxValue;
                else
                    resource.Value += 1;
                break;
        }
    }
}
