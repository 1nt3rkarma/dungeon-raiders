using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row : MonoBehaviour
{
    public List<Block> blocks;
    public List<MeshRenderer> renderers;
    public Transform decorationPivot;

    /// <summary>
    /// Does this row have any empty blocks?
    /// </summary>
    public bool ContainsEmpty
    {
        get
        {
            foreach (var block in blocks)
                if (block.isEmpty)
                    return true;
            return false;
        }
    }

    /// <summary>
    /// Is block which borders wall empty?
    /// </summary>
    public bool ContainsEmptyBorder => blocks[0].isEmpty;

    public void SetPosition(int z)
    {
        transform.localPosition = new Vector3(0, 0, z);
    }

    public List<Block> GetSolidBlocks()
    {
        var returnBlocks = new List<Block>();
        foreach (var block in blocks)
            if (!block.isEmpty)
                returnBlocks.Add(block);

        return returnBlocks;
    }

    void SetBlocksPosition(float y)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            var newPosition = blocks[i].transform.localPosition;
            newPosition.y = y;
            blocks[i].transform.localPosition = newPosition;
        }
    }

    void SetBlocksAlpha(float alpha)
    {
        Mathf.Clamp(alpha, 0, 1);

        for (int i = 0; i < renderers.Count; i++)
        {
            var newColor = renderers[i].material.color;
            newColor.a = alpha;
            renderers[i].material.color = newColor;
        }
    }

    void SetBlocksColor(Color color)
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].material.color = color;
        }
    }
}
