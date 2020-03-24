using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level N", menuName = "Level Settings")]
public class LevelSettings : ScriptableObject
{
    public int levelSteps = 100;

    //public LevelGenerator generationSettings;

    //[HideInInspector]
    public List<LevelPresetParams> presetsParams;
}

[System.Serializable]
public class LevelPresetParams : object
{
    public LevelPreset presetPrefab;

    public int intervalMin = 1;
    public int intervalMax = 12;
    public float basicChance = 0.1f;

    public bool infinite = false;
    public int count = 5;
    public bool allowItemsGeneration = true;
}

[System.Serializable]
public class LevelGenerationSettings : object
{
    [Header("Coin generation settings")]
    public Coin coinPref;
    [Range(0, 1)]
    public float coinBasicChance = 0.1f;
    public int coinMinInterval = 5;
    public int coinMaxInterval = 9;

    //[HideInInspector]
    public int coinSpawnCounter = 0;

    [Header("Trap generation settings")]
    public SpikeTrap spikeTrapPref;
    [Range(0, 1)]
    public float trapBasicChance = 0.2f;
    public int trapMinInterval = 4;
    public int trapMaxInterval = 10;

    //[HideInInspector]
    public int trapSpawnCounter = 0;

    [Header("Gaps generation settings")]
    public bool allowRowGap = true;

    [Range(0, 1)]
    public float gapBasicChance = 0.1f;
    public int gapMinInterval = 11;
    public int gapMaxInterval = 13;
    public int gapSpawnCounter = 0;

    public int gapMinLength = 2;
    public int gapMaxLength = 3;
    public int gapMinWigth = 1;
    public int gapMaxWigth = 2;

    //[HideInInspector]
    public int gapTempCounter = 0;
    //[HideInInspector]
    public int gapTempWidth = 1;
    //[HideInInspector]
    public int[] gapTempLineIndexes;

    public void GenerateRandomObjects(Row row)
    {
        coinSpawnCounter++;
        trapSpawnCounter++;

        var blocks = row.GetSolidBlocks();
        if (blocks.Count == 0)
            return;

        if (coinSpawnCounter >= coinMinInterval
            && coinSpawnCounter <= coinMaxInterval)
        {
            var delta = Mathf.Abs(coinMaxInterval - coinMinInterval);
            var temp = Mathf.Abs(coinSpawnCounter - coinMinInterval);
            var chance = coinBasicChance + (float)temp / delta;
            var roll = Random.Range(0f, 0.99f);

            if (chance >= roll)
            {
                int rand = Random.Range(0, blocks.Count);
                var block = blocks[rand];
                var coin = Object.Instantiate(coinPref, block.transform);
                coin.transform.localPosition = Vector3.zero;
                coinSpawnCounter = 0;
            }
        }

        if (trapSpawnCounter >= trapMinInterval
            && trapSpawnCounter <= trapMaxInterval)
        {
            var delta = Mathf.Abs(trapMaxInterval - trapMinInterval);
            var temp = Mathf.Abs(trapSpawnCounter - trapMinInterval);
            var chance = trapBasicChance + (float)temp / delta;
            var roll = Random.Range(0f, 0.99f);

            if (chance >= roll)
            {
                int rand = Random.Range(0, blocks.Count);
                var block = blocks[rand];
                var trap = Object.Instantiate(spikeTrapPref, block.transform);
                trap.transform.localPosition = Vector3.zero;
                trapSpawnCounter = 0;
            }
        }
    }

    public void GenerateGaps(Row row)
    {
        if (gapTempCounter == 0)
        {
            gapSpawnCounter++;
            if (gapSpawnCounter >= gapMinInterval
                && gapSpawnCounter <= gapMaxInterval)
            {
                var delta = Mathf.Abs(gapMaxInterval - gapMinInterval);
                var temp = Mathf.Abs(gapSpawnCounter - gapMinInterval);
                var chance = gapBasicChance + temp / delta;
                var roll = Random.Range(0f, 0.99f);

                if (chance >= roll)
                {
                    var rollSingle = Random.Range(0, 5) > 1;
                    if (rollSingle && allowRowGap)
                    {
                        gapSpawnCounter = 0;

                        // Генерация одного пустого ряда
                        foreach (var block in row.blocks)
                            block.MakeEmpty();
                    }
                    else
                    {
                        gapSpawnCounter = 0;

                        // Установка параметров генерации пропасти
                        // в длинну на несколько рядов и линий
                        gapTempCounter = Random.Range(gapMinLength, gapMaxLength + 1);
                        gapTempWidth = Random.Range(gapMinWigth, gapMaxWigth + 1);
                        var rollSplit = Random.Range(0, 8) > 1;

                        if (gapTempWidth == 2)
                        {
                            if (rollSplit)
                            {
                                gapTempLineIndexes = new int[] { 0, 2 };
                            }
                            else
                            {
                                var startIndex = Random.Range(0, 2);
                                gapTempLineIndexes = new int[] { startIndex, startIndex + 1 };
                            }
                        }
                        else
                        {
                            var rand = Random.Range(0, 3);
                            gapTempLineIndexes = new int[] { rand };
                        }
                    }
                }
            }
        }
        else
        {
            // Генерация очередного ряда
            // пропасти по заданным параметрам
            gapTempCounter--;
            for (int i = 0; i < gapTempLineIndexes.Length; i++)
            {
                int index = gapTempLineIndexes[i];
                row.blocks[index].MakeEmpty();
            }
        }
    }
}