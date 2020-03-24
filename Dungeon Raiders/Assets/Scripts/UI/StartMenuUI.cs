using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    public Animator animator;

    public Text coinsTotalText;
    public Text stepsBestsText;

    private void Start()
    {
        coinsTotalText.text = Player.coinsTotal.ToString();
        stepsBestsText.text = Player.stepsBest.ToString() + " STEPS";
    }

    public void PopUp()
    {
        animator.SetTrigger("play");
    }

    public void Fade()
    {
        animator.SetTrigger("fade");
    }
}
