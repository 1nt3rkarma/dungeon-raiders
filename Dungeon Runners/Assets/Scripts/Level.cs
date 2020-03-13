using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    static Level singlton;

    Coroutine animationRoutine;

    public bool isMoving = true;

    public Row rowPref;
    public List<Row> rows;

    public float moveSpeed = 1;
    public float distancePerCycle = 1;

    public LevelGenerator generator;

    void Awake()
    {
        if (singlton != null)
            Destroy(singlton.gameObject);

        singlton = this;
    }

    void Update()
    {
        if (isMoving && animationRoutine == null)
            animationRoutine = StartCoroutine(FlowRoutine());
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
        // Исходная позиция Игрока
        //var r = player.hero.row;
        //var l = player.hero.line;

        //Удаляем старый ряд
        DestroyRow(0);

        // Засчитываем перемещение Игроку
        //player.steps++;
        //player.hero.block = rows[r].blocks[l];

        //Выравнивам новую позицию блоков
        SetRowsPosition(0);

        if (!isMoving)
            animationRoutine = null;
        else
            animationRoutine = StartCoroutine(FlowRoutine());
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
            GenerateRandomObjects(row);
    }

    void GenerateRandomObjects(Row row)
    {
        generator.coinSpawnCounter++;
        if (generator.coinSpawnCounter >= generator.coinMinInterval
            && generator.coinSpawnCounter <= generator.coinMaxInterval)
        {
            var delta = Mathf.Abs(generator.coinMaxInterval - -generator.coinMinInterval);
            var temp = Mathf.Abs(generator.coinSpawnCounter - generator.coinMinInterval);
            var chance = generator.coinBasicChance + temp / delta;
            var roll = Random.Range(0f, 1f);

            if (chance >= roll)
            {
                int l = Random.Range(0, row.blocks.Count);
                var coin = Instantiate(generator.coinPref, row.blocks[l].transform);
                coin.transform.localPosition = Vector3.zero;
                generator.coinSpawnCounter = 0;
            }
        }
        else if (generator.coinSpawnCounter > generator.coinMaxInterval)
        {
            int l = Random.Range(0, row.blocks.Count);
            var coin = Instantiate(generator.coinPref, row.blocks[l].transform);
            coin.transform.localPosition = Vector3.zero;
            generator.coinSpawnCounter = 0;
        }

        generator.trapSpawnCounter++;
        if (generator.trapSpawnCounter >= generator.trapMinInterval
            && generator.trapSpawnCounter <= generator.trapMaxInterval)
        {
            var delta = Mathf.Abs(generator.trapMaxInterval - -generator.trapMinInterval);
            var temp = Mathf.Abs(generator.trapSpawnCounter - generator.trapMinInterval);
            var chance = generator.trapBasicChance + temp / delta;
            var roll = Random.Range(0f, 1f);

            if (chance >= roll)
            {
                int l = Random.Range(0, row.blocks.Count);
                var trap = Instantiate(generator.spikeTrapPref, row.blocks[l].transform);
                trap.transform.localPosition = Vector3.zero;
                generator.trapSpawnCounter = 0;
            }
        }
        else if (generator.trapSpawnCounter > generator.trapMaxInterval)
        {
            int l = Random.Range(0, row.blocks.Count);
            var trap = Instantiate(generator.spikeTrapPref, row.blocks[l].transform);
            trap.transform.localPosition = Vector3.zero;
            generator.trapSpawnCounter = 0;
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
}

[System.Serializable]
public class LevelGenerator : object
{
    public bool enabled = true;

    public Coin coinPref;
    [Range(0,1)]
    public float coinBasicChance = 0.1f;
    public int coinMinInterval = 5;
    public int coinMaxInterval = 9;
    public int coinSpawnCounter = 0;

    public SpikeTrap spikeTrapPref;
    [Range(0, 1)]
    public float trapBasicChance = 0.2f;
    public int trapMinInterval = 4;
    public int trapMaxInterval = 10;
    public int trapSpawnCounter = 0;
}