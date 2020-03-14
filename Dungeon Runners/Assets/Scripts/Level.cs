using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    static Level singlton;

    public static int level;
    public static int levelSteps;

    public bool isMoving = true;

    public float moveSpeed = 1;
    public float distancePerCycle = 1;

    public LevelGenerator generator;

    public Row rowPref;
    public List<Row> rows;

    Coroutine flowRoutine;

    void Awake()
    {
        if (singlton != null)
            Destroy(singlton.gameObject);

        singlton = this;

        level = 1;
        levelSteps = 100;
    }

    void Update()
    {
        if (isMoving && flowRoutine == null)
            flowRoutine = StartCoroutine(FlowRoutine());
    }

    void SetRowsPosition(float z)
    {
        for (int i = 0; i < rows.Count; i++)
        {
            var newPosition = rows[i].transform.localPosition;
            newPosition.z = z + i * 1;                          // Ввести параметр "расстояние между рядами"?
            rows[i].transform.localPosition = newPosition;
            rows[i].name = $"Row [{i}]";
        }
    }

    IEnumerator FlowRoutine()
    {
        var z = 0f;

        // Создаем новый ряд
        AddRow();

        // Запускаем уничтожение старого
        rows[0].Fade();

        // Плавно сдвигаем ряды
        while (z > -1)
        {

            z -= moveSpeed * Time.deltaTime;
            SetRowsPosition(z);
            yield return null;
        }

        //Удаляем старый ряд
        DestroyRow(0);

        //Выравнивам новую позицию блоков
        SetRowsPosition(0);

        StartFlow();
    }

    void StartFlow()
    {
        if (flowRoutine != null)
            StopCoroutine(FlowRoutine());

        if (!isMoving)
            flowRoutine = null;
        else
            flowRoutine = StartCoroutine(FlowRoutine());
    }

    void DestroyRow(int index)
    {
        var row = rows[index];
        rows.RemoveAt(index);
        Destroy(row.gameObject);
    }

    void AddRow()
    {
        var row = Instantiate(rowPref, transform);
        row.SetPosition(rows.Count);
        rows.Add(row);

        if (generator.enabled)
        {
            generator.GenerateGaps(row);
            generator.GenerateRandomObjects(row);
        }
    }

    public static Block GetNearestBlock(Vector3 serachPosition)
    {
        Block wanted = singlton.rows[1].blocks[1];
        var sqrDistanceMin = 100f;

        foreach (var row in singlton.rows)
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

    public static int GetBlockRow(Block block)
    {
        for (int i = 0; i < singlton.rows.Count; i++)
            if (singlton.rows[i].blocks.Contains(block))
                return i;

        return -1;
    }

    public static Block GetBlock(int row, int line)
    {
        return singlton.rows[row].blocks[line];
    }

    public static void StopFlow()
    {
        singlton.isMoving = false;
    }

    public static void RunFlow()
    {
        singlton.isMoving = true;
    }

    public static void SwitchLevel()
    {
        SwitchLevel(level + 1);
    }

    public static void SwitchLevel(int level)
    {
        Level.level = level;
        levelSteps = 100 + level * 10;
    }
}

[System.Serializable]
public class LevelGenerator : object
{
    public bool enabled = true;

    [Header("Coin generation settings")]
    public Coin coinPref;
    [Range(0,1)]
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
                    var rollSingle = Random.Range(0, 2) > 1;
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
                        gapTempCounter = Random.Range(gapMinLength, gapMaxLength+1);
                        gapTempWidth = Random.Range(gapMinWigth, gapMaxWigth+1);
                        var rollSplit = Random.Range(0, 5) > 1;

                        if (gapTempWidth == 2)
                        {
                            if (rollSplit)
                            {
                                gapTempLineIndexes = new int[] { 0, 2 };
                            }
                            else
                            {
                                var startIndex = Random.Range(0, 2);
                                gapTempLineIndexes = new int[] { startIndex, startIndex+1 };
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