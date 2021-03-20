using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickWallDecoration : TerrainDecoration
{
    [Header("Brick wall settings")]


    public List<GameObject> brickLayers;
    public List<Transform> brickPivots;

    public List<MeshVariation> wallVariations;

    public override void Randomize()
    {
        // Случайны виж стены

        var chances = new List<float>();

        foreach (var variation in wallVariations)
            chances.Add(variation.basicChance);

        var index = MathUtils.GetRandomIndexFromListOfChances(chances);

        foreach (var variation in wallVariations)
            if (variation == wallVariations[index] && variation.meshObject != null)
                variation.meshObject.SetActive(true);
            else if (variation.meshObject != null)
                Destroy(variation.meshObject);

        // Случайны порядок слоев блоков

        var pivots = new List<Transform>(brickPivots);
        foreach (var brick in brickLayers)
        {
            var randomIndex = Random.Range(0, pivots.Count);
            brick.transform.localPosition = pivots[randomIndex].localPosition;
            pivots.Remove(pivots[randomIndex]);
        }

    }
}
