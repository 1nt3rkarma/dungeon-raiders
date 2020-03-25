using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public List<InventoryButton> slots;

    public void UpdateUI()
    {
        for (int i = 0; i < slots.Count; i++)
            if (i < Player.inventory.Count)
                slots[i].Enable(Player.inventory[i].icon);
            else
                slots[i].Disable();
    }

    public void OnSlotClick(InventoryButton slot)
    {
        int index = slots.IndexOf(slot);
        Player.UseItem(index);
    }
}
