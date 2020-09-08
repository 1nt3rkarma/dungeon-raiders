using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float basicHieght = 0.35f;

    public float speed = 3;

    public float damage = 1;

    public float lifetime = 5;

    public GameObject hitEffect;
    public AudioClip hitSound;

    public GameObject model;
    public bool canStick;
    public float stickSizeDecrease = 0.15f;

    public bool canBreak;
    public Shards shardsPrefab;

    [Range(0,1f)]
    public float breakChance = 0.5f;

    [HideInInspector]
    public Unit caster;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        var unit = other.GetComponent<Unit>();
        if (unit)
            if (unit.isAlive && unit != caster)
            {
                var effect = Instantiate(hitEffect,
                    unit.GetUnitPoint(UnitBodyPoints.chest).position, transform.rotation);

                var audioSource = effect.GetComponent<AudioSource>();
                if (audioSource && hitSound != null && !unit.isDefending)
                    audioSource.PlayOneShot(hitSound);

                if (!unit.isDefending)
                {
                    if (canStick)
                        StickTo(unit);
                }
                else
                {
                    if (canStick && !canBreak)
                        StickTo(unit);
                    if (!canStick && canBreak)
                        BreakOff(unit);
                    if (canStick && canBreak)
                        if (!RandomExtensions.Bool(breakChance))
                            StickTo(unit);
                        else
                            BreakOff(unit);
                }


                unit.TakeDamage(damage, caster);

                Destroy(gameObject);
            }
    }

    void StickTo(Unit unit)
    {
        List<StickPoint> stickPoints = null;
        if (unit.isDefending)
        {
            if (unit.animHandler.defendStickPoints.Count > 0)
                stickPoints = unit.animHandler.defendStickPoints;
        }
        else
        {
            if (unit.animHandler.defaultStickPoints.Count > 0)
                stickPoints = unit.animHandler.defaultStickPoints;
        }

        if (stickPoints != null)
        {
            int index = Random.Range(0, stickPoints.Count);
            var targetPoint = stickPoints[index];

            DecreaseSize(model);

            model.transform.SetParent(targetPoint.transform);
            model.transform.position = targetPoint.transform.position;
            AddRandomRotation(model, targetPoint.randomRotationDeltas);
            SetRandomPosition(model, targetPoint.randomPositionDeltas);
            unit.animHandler.stickedProjectiles.Add(model.transform);
        }
    }

    void BreakOff(Unit unit)
    {
        var shards = Instantiate(shardsPrefab, transform.position, transform.rotation);
        shards.transform.SetParent(unit.block.transform);
        shards.SimulateExplosion();
    }

    void DecreaseSize(GameObject model)
    {
        var x = model.transform.localScale.x * (1 - stickSizeDecrease);
        var y = model.transform.localScale.y * (1 - stickSizeDecrease);
        var z = model.transform.localScale.z * (1 - stickSizeDecrease);

        model.transform.localScale = new Vector3(x, y, z);
    }

    void AddRandomRotation(GameObject model, Vector3 randomRotationDeltas)
    {
        var x = Random.Range(-randomRotationDeltas.x, randomRotationDeltas.x);
        var y = Random.Range(-randomRotationDeltas.y, randomRotationDeltas.y);
        var z = Random.Range(-randomRotationDeltas.z, randomRotationDeltas.z);

        model.transform.localEulerAngles += new Vector3(x, y, z);
    }

    void SetRandomPosition(GameObject model, Vector3 randomPositionDeltas)
    {
        var x = Random.Range(-randomPositionDeltas.x, randomPositionDeltas.x);
        var y = Random.Range(-randomPositionDeltas.y, randomPositionDeltas.y);
        var z = Random.Range(-randomPositionDeltas.z, randomPositionDeltas.z);

        model.transform.position += new Vector3(x, y, z);
    }
}
