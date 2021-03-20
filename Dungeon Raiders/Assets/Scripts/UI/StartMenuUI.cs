using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    public Animator animator;

    public Text coinsTotalText;
    public Text stepsBestsText;

    private void Update()
    {
        coinsTotalText.text = Player.Data.coinsTotal.ToString();
        stepsBestsText.text = Player.Data.stepsBest.ToString() + " STEPS";
    }

    public void PopUp()
    {
        animator.SetTrigger("play");
    }

    public void Fade()
    {
        animator.SetTrigger("fade");
    }

    public void ForceDisable()
    {
        gameObject.SetActive(false);
    }

    public void RequireGameplayMaskEnable()
    {
        UI.EnableGameplayMask();
    }
    public void RequireGameplayTintEnable()
    {
        UI.EnableGameplayTint(1f);
    }
    public void RequireGameplayTintDisable()
    {
        UI.DisableGameplayTint(0.25f);
    }
}
