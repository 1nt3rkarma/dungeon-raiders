using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPhase : MonoBehaviour
{
    [TextArea]
    public string text;
    public int actionsCount;
    public bool forceHeroIngnorance;
    public PlayerActions catchAction;

    public string hintAnimationTag;
    public bool needTint;
    public Hero forHeroOnly;
    public Hero ignoreHero;

    public bool IgnoreHeroType(Unit hero)
    {
        if (ignoreHero != null && hero.unitType == ignoreHero.unitType)
            return true;
        else
            return false;

        //if (forHeroTypeOnly == null)
        //    return false;

        //if (hero.unitType == forHeroTypeOnly)
        //    return false;
        //else
        //    return true;

    }
}
