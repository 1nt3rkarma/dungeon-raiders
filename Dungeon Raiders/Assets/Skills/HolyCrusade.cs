using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyCrusade : Skill
{
    [Header("Specific settings")]
    public float faithPerKill = 1;

    void Awake()
    {
        SubsribeToGameEvents();
    }

    void Update()
    {
        OnUpdate();
    }

    void OnDestroy()
    {
        Deinitialize();
        UnsubsribeToGameEvents();
    }

    protected override void OnUnitDie(Unit unit, DamageType type, Object source)
    {
        //Debug.Log($"HolyCrusade NOTICED KILL: target is {unit.name}, source is {source.name} (hash {source.GetHashCode()})");

        if (source == caster && caster.Faith != null)
            caster.Faith.Value += faithPerKill;
        //else
        //{
        //    if (source != caster)
        //        Debug.Log($"KILL SOURCE DOESN'T EQUAL CASTER (hash {caster.GetHashCode()})");

        //    if (caster.Faith == null)
        //        Debug.Log($"CASTER DOESN'T HAVE FAITH");
        //}
    }
}
