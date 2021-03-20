using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDecoration : MonoBehaviour
{
    [Header("Common settings")]

    public int blockSize;

    [Tooltip("Can this decoration be placed right next to its copy?")]
    public bool allowRepeats;
    [Tooltip("Can this decoration be placed at the row which contains empty blocks near wall?")]
    public bool avoidEmptyRowBorder = true;
    [Tooltip("Can this decoration be placed at the row which follows one contains empty blocks near wall?")]
    public bool avoidEmptyNeighborBorder = true;
    [Tooltip("This object should be displayed only if there is a non empty row in front of it")]
    public MeshRenderer frontFace;

    public virtual void Randomize()
    {

    }

}
