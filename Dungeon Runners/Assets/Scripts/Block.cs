using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool isEmpty;
    public Row parent;

    public bool fallRegistered = false;

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
        if (!fallRegistered)
            ScanHeroEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!fallRegistered)
            ScanHeroEnter(other);
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (heroIsHere)
    //        ScanHeroExit(other);
    //}

    void ScanHeroEnter(Collider collider)
    {
        Hero hero = collider.GetComponent<Hero>();
        if (hero != null)
        {
            if (!fallRegistered)
            {
                if (!hero.isFloating && isEmpty
                    && (!hero.isMoving || hero.transform.position.z - this.transform.position.z < 0))
                {
                    hero.TakeDamage(hero.health, DamageSources.fall);
                    fallRegistered = true;
                }
            }
            hero.SwitchBlockTo(this);
        }
    }

    //void ScanHeroExit(Collider collider)
    //{
    //    Hero hero = collider.GetComponent<Hero>();
    //    if (hero != null)
    //        heroIsHere = false;
    //}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(scanAreaPoint.position, scanAreaSize);
    }
}
