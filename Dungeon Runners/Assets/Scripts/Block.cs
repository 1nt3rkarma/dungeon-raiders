using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Row parent;

    public Transform checkPoint;

    public int GetLineIndex()
    {
        return parent.blocks.IndexOf(this);
    }

    public int GetRowIndex()
    {
        return Level.GetBlockRow(this);
    }

    void OnTriggerEnter(Collider other)
    {
        Hero hero = other.GetComponent<Hero>();
        if (hero != null)
            hero.block = this;
    }
}
