using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public Sprite icon;

    public AudioClip useSound;

    public int price;

    public bool isExpendable = true;

    public virtual void Use()
    {
        Player.PlaySound(useSound);

        if (isExpendable)
            Player.RemoveItem(this);
    }
}
