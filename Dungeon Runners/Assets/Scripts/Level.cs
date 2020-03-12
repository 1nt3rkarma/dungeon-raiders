using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    static Level singlton;

    Coroutine animationRoutine;

    [Range(1,100)]
    public int coinChance = 40;
    public Coin coinPref;
    public Row rowPref;

    public bool isMoving = true;

    public List<Row> rows;

    public float moveSpeed = 1;
    public float distancePerCycle = 1;

    public Player player;

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

        GenerateRandomObjects(row);
    }

    void GenerateRandomObjects(Row row)
    {
        var roll = Random.Range(1, 100);
        if (roll <= coinChance)
            return;

        int l = Random.Range(0, row.blocks.Count);
        var coin = Instantiate(coinPref, row.blocks[l].transform);
        coin.transform.localPosition = Vector3.zero;
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
