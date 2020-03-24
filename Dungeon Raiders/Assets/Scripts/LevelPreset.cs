using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPreset : MonoBehaviour
{
    [TextArea]
    public string description;

    public List<Row> rows;

    public void AlignChildren()
    {
        foreach (Transform child in transform)
        {
            var row = child.GetComponent<Row>();
            if (row == null)
            {
                var block = GetNearestBlock(child.position);
                child.position = block.transform.position;
                child.transform.SetParent(block.transform);
            }
        }
    }

    public Block GetNearestBlock(Vector3 serachPosition)
    {
        Block wanted = rows[1].blocks[1];
        var sqrDistanceMin = 100f;

        foreach (var row in rows)
        {
            foreach (var block in row.blocks)
            {
                var sqrDistance = Vector3.SqrMagnitude(serachPosition - block.scanAreaPoint.position);
                if (sqrDistance < sqrDistanceMin)
                {
                    wanted = block;
                    sqrDistanceMin = sqrDistance;
                    if (sqrDistanceMin < 1)
                        return wanted;
                }
            }
        }
        return wanted;
    }
}
