using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TextField = TMPro.TextMeshPro;


public class GatesAnimationHandler : MonoBehaviour
{
    public TextField dungeonLogo;

    DoTweenGraphicColorizer dungeonLogoFader;

    public void Awake()
    {
        dungeonLogoFader = new DoTweenGraphicColorizer(dungeonLogo);
    }

    public void FadeDungeonLogo(float value = 0, float overTime = 0)
    {
        if (overTime > 0)
            dungeonLogoFader.Transit(value, overTime);
        else
            dungeonLogoFader.SetAlpha(value);
    }

    public virtual void PlayToTheDungeonAnimation()
    {

    }
}
