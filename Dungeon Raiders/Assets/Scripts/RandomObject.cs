using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObject : MonoBehaviour
{
    public RandomObjectLineBehaviours lineBehaviour;

    public List<RandomObjectType> typesList;

    public List<float> chances
    {
        get
        {
            var chances = new List<float>();
            foreach (var t in typesList)
                chances.Add(t.chance);
            return chances;
        }
    }

    public void Generate()
    {
        var index = MathUtils.GetRandomIndexFromListOfChances(chances);
        if (index >= 0)
        {
            var randomType = typesList[index];

            if (randomType != null)
            {
                // Влияем на указанные объекты
                foreach (var affected in randomType.affectedObjects)
                {
                    foreach (var t in affected.randomObject.typesList)
                    {
                        if (t.prefab == randomType.prefab)
                            switch (affected.affectionBehaviour)
                            {
                                case RandomObjectAffectBehaviours.exclude:
                                    t.SetChance(0);
                                    break;
                                case RandomObjectAffectBehaviours.decrease:
                                    t.IncreaseChance(affected.increaseRate);
                                    break;
                                case RandomObjectAffectBehaviours.increase:
                                    t.DecreaseChance(affected.decreaseRate);
                                    break;
                            }
                    }
                }

                // Покидаем квантовую суперпозицию
                var position = transform.position;

                if (lineBehaviour == RandomObjectLineBehaviours.random)
                {
                    var localPosition = transform.localPosition;
                    position -= localPosition;
                    position += Vector3.right * Random.Range(0, 3);
                }

                if (lineBehaviour == RandomObjectLineBehaviours.mimic)
                {
                    var localPosition = transform.localPosition;
                    position -= localPosition;
                    position += Vector3.right * Hero.singlton.line;
                }

                var obj = Instantiate(randomType.prefab, position, Quaternion.identity);

                obj.transform.SetParent(transform.parent);
            }
        }

        Destroy(this.gameObject);
    }

    List<float> GetChances()
    {
        var chances = new List<float>();
        foreach (var t in typesList)
            chances.Add(t.chance);
        return chances;
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var type in typesList)
        {
            foreach (var affected in type.affectedObjects)
            {
                if (affected.randomObject == null)
                    continue;

                var direction = affected.randomObject.transform.position - this.transform.position;
                direction.Normalize();

                var center = this.transform.position + 2 * Vector3.up;
                var target = affected.randomObject.transform.position + Vector3.up;

                switch (affected.affectionBehaviour)
                {
                    case RandomObjectAffectBehaviours.exclude:
                        Gizmos.color = Color.red;
                        break;
                    case RandomObjectAffectBehaviours.decrease:
                        Gizmos.color = Color.yellow;
                        break;
                    case RandomObjectAffectBehaviours.increase:
                        Gizmos.color = Color.green;
                        break;
                }
                Gizmos.DrawWireSphere(center, 0.15f);
                Gizmos.DrawLine(center, target);
            }
        }
    }
}

[System.Serializable]
public class RandomObjectType : object
{
    public GameObject prefab;
    [Range(0.01f, 1)]
    public float chance;

    public List<RandomObjectAffected> affectedObjects;

    public void IncreaseChance(float value)
    {
        SetChance(chance += value);
    }

    public void DecreaseChance(float value)
    {
        SetChance(chance -= value);
    }

    public void SetChance(float value)
    {
        chance = Mathf.Clamp(value, 0, 1);
    }
}

public enum RandomObjectLineBehaviours { none, random, mimic }

[System.Serializable]
public class RandomObjectAffected : object
{
    public RandomObject randomObject;

    public float decreaseRate;
    public float increaseRate;

    public RandomObjectAffectBehaviours affectionBehaviour;
}

public enum RandomObjectAffectBehaviours { exclude, decrease, increase }

