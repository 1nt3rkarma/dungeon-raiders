using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    public Animator animator;

    public void PopUp()
    {
        animator.SetTrigger("show");
    }

    public void SlideAway()
    {
        animator.SetTrigger("hide");
    }

    public void NewLevel()
    {
        animator.SetTrigger("newLevel");
    }
}
