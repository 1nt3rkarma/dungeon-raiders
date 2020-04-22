using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    public Animator animator;

    public InventoryUI inventoryUI;
    public SkillUI skillUI;

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
