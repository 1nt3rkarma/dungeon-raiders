using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    public Sprite icon;

    public AudioClip useSound;

    public bool expendable;

    public virtual void Use()
    {

    }
}
