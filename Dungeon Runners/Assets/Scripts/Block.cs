using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Row parent;

    public Transform scanAreaPoint;
    public Vector3 scanAreaSize;

    public int GetLineIndex()
    {
        return parent.blocks.IndexOf(this);
    }

    public int GetRowIndex()
    {
        return Level.GetBlockRow(this);
    }

    public GameObject[] ScanObjects()
    {
        List<GameObject> objects = new List<GameObject>() ;
        var colliders = Physics.OverlapBox(scanAreaPoint.position, scanAreaSize / 2);

        foreach (var collider in colliders)
        {
            if (collider.gameObject != gameObject)
                objects.Add(collider.gameObject);
        }
        return objects.ToArray();
    }

    void OnTriggerEnter(Collider other)
    {
        Hero hero = other.GetComponent<Hero>();
        if (hero != null)
            hero.SwitchBlockTo(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(scanAreaPoint.position, scanAreaSize);
    }
}
