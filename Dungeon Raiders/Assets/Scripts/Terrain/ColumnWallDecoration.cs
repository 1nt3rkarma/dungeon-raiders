using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnWallDecoration : TerrainDecoration
{
    [Header("Column wall settings")]


    public List<MeshVariation> torchlightVariations;

    public GameObject columnObject;

    public override void Randomize()
    {
        // Случайное наличие факела

        var chances = new List<float>();

        foreach (var variation in torchlightVariations)
            chances.Add(variation.basicChance);

        var index = MathUtils.GetRandomIndexFromListOfChances(chances);

        foreach (var variation in torchlightVariations)
            if (variation == torchlightVariations[index] && variation.meshObject != null)
                variation.meshObject.SetActive(true);
            else if (variation.meshObject != null)
                Destroy(variation.meshObject);

        // Случайный поворот колонны
    }
}
