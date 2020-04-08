using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool isEmpty;
    public Row parent;

    public bool heroRegistred = false;

    public MeshRenderer model;

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

    public void MakeEmpty()
    {
        model.gameObject.SetActive(false);
        isEmpty = true;
    }

    public void MakeSolid()
    {
        model.gameObject.SetActive(true);
        isEmpty = false;
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

    private void OnTriggerEnter(Collider other)
    {
        if (!heroRegistred)
            ScanHeroEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!heroRegistred)
            ScanHeroEnter(other);
    }

    void ScanHeroEnter(Collider collider)
    {
        Hero hero = collider.GetComponent<Hero>();
        if (hero != null)
            hero.SwitchBlockTo(this);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(scanAreaPoint.position, scanAreaSize);
    }
}
