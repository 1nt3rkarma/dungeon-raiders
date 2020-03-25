using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    public InventoryUI parentUI;

    public Button button;

    public Image icon;
    
    public void Enable(Sprite image)
    {
        icon.sprite = image;
        icon.enabled = true;
        button.interactable = true;
    }

    public void Disable()
    {
        icon.enabled = false;
        icon.sprite = null;
        button.interactable = false;
    }

    public void OnClick()
    {
        parentUI.OnSlotClick(this);
    }

}
