using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObject : MonoBehaviour
{
    [HideInInspector]
    public List<RandomObject> affects;
    [HideInInspector]
    public List<bool> exclude;
    [HideInInspector]
    public List<bool> decrease;
    [HideInInspector]
    public List<float> decreaseRate;
    [HideInInspector]
    public List<bool> increase;
    [HideInInspector]
    public List<float> increaseRate;


    [HideInInspector]
    public List<GameObject> types;
    [HideInInspector]
    public List<float> chances;
    [HideInInspector]
    public List<bool> randomLines;

    public void Generate()
    {
        var index = MathUtilities.GetRandomIndexFromListOfChances(chances);
        if (index >= 0)
        {
            var type = types[index];

            int count = affects.Count;
            for (int i = 0; i < count; i++)
            {
                var affectedTypeIndex = affects[i].types.IndexOf(type);
                if (affectedTypeIndex != -1)
                {
                    if (increase[i])
                        affects[i].IncreaseChance(affectedTypeIndex, increaseRate[i]);

                    if (decrease[i])
                        affects[i].DecreaseChance(affectedTypeIndex, decreaseRate[i]);

                    if (exclude[i])
                        affects[i].SetChance(affectedTypeIndex, 0);
                }
            }

            if (type != null)
            {
                var position = transform.position;

                if (randomLines[index])
                {
                    var localPosition = transform.localPosition;
                    position -= localPosition;
                    position += Vector3.right * Random.Range(0, 3);
                }

                var obj = Instantiate(type, position, Quaternion.identity);

                obj.transform.SetParent(transform.parent);
            }
        }

        Destroy(this.gameObject);
    }

    void IncreaseChance(int index, float value)
    {
        SetChance(index, chances[index] += value);
    }

    void DecreaseChance(int index, float value)
    {
        SetChance(index, chances[index] -= value);
    }

    void SetChance(int index, float value)
    {
        chances[index] = value;
        chances[index] = Mathf.Clamp(chances[index], 0, 1);
    }

    private void OnDrawGizmosSelected()
    {

        for (int i = 0; i < affects.Count; i++)
        {
            if (affects[i] == null)
                continue;

            if (increase[i])
                Gizmos.color = Color.green;
            if (decrease[i])
                Gizmos.color = Color.yellow;
            if (exclude[i])
                Gizmos.color = Color.red;

            var direction = affects[i].transform.position - this.transform.position;
            direction.Normalize();
            var shift = 1f;

            var center = this.transform.position + 2*Vector3.up;
            var target = affects[i].transform.position + Vector3.up;

            Gizmos.DrawWireSphere(center, 0.15f);
            Gizmos.DrawLine(center, target);
        }
    }
}
