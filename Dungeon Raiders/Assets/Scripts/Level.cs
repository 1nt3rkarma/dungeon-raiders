using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    static Level singlton;

    public static int level;
    public static int levelSteps;

    public static bool generationEnabled = true;
    public static int generationCount;
    public static LevelSettings activeSettings
    {
        get
        {
            if (level <= 0)
                return singlton.settings[0];
            else if (level >= singlton.settings.Count)
                return singlton.settings[singlton.settings.Count - 1];
            else
                return singlton.settings[level - 1];
        }
    }

    public bool isMoving = true;

    public float moveSpeed = 1;
    private float shift = 0;

    public Row rowPref;
    public List<Row> rows;
    public LevelGenerator generator;

    public List<LevelSettings> settings;
    public int[] presetCounters;
    public LevelPreset lastPreset;

    public int generationCountView;
    public bool generationEnabledView;

    Coroutine flowRoutine;

    void Awake()
    {
        InitSinglton(this);
    }

    void Start()
    {
        SwitchLevel(1);
    }

    void Update()
    {
        //if (isMoving && flowRoutine == null)
        //    flowRoutine = StartCoroutine(FlowRoutine());

        if (isMoving)
        {
            if (shift > -1)
            {
                shift -= moveSpeed * Time.deltaTime;
            }
            else
            {
                GenerateNext();
                DestroyRow(0);
                shift += 1;
                Player.AddStep();
            }
            SetRowsPosition(shift);
        }

        generationCountView = generationCount;
        generationEnabledView = generationEnabled;
    }

    static void InitSinglton(Level instance)
    {
        if (singlton != null)
            Destroy(singlton.gameObject);

        singlton = instance;

        generationCount = 0;
        generationEnabled = true;
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

    void DestroyRow(int index)
    {
        var row = rows[index];
        rows.RemoveAt(index);
        Destroy(row.gameObject);
    }

    void Add(Row row)
    {
        row.transform.SetParent(this.transform);
        row.SetPosition(rows.Count);
        rows.Add(row);
    }

    Row CreateRow()
    {
        var row = Instantiate(rowPref, transform);
        Add(row);

        return row;
    }

    void GenerateNext()
    {
        if (generationEnabled)
        {
            generationCount++;
            if (generationCount >= 0)
            {
                var presetParams = GetPossiblePresets(generationCount);

                if (lastPreset != null && presetParams.Count > 1)
                {
                    var checkRepeat = new List<LevelPresetParams>(presetParams);
                    foreach (var param in checkRepeat)
                        if (param.presetPrefab == lastPreset)
                            presetParams.Remove(param);
                }

                if (presetParams.Count > 0)
                {
                    var random = Random.Range(0, presetParams.Count);
                    var presetParam = presetParams[random];
                    var preset = Instantiate(presetParam.presetPrefab);
                    lastPreset = presetParam.presetPrefab;

                    int index = activeSettings.presetsParams.IndexOf(presetParam);
                    if (!presetParam.infinite)
                        presetCounters[index]--;

                    generationCount = -preset.rows.Count;

                    foreach (var row in preset.rows)
                        Add(row);

                    foreach (var randomObject in preset.randomObjects)
                        randomObject.Generate();

                    Destroy(preset.gameObject);
                }
                else
                {
                    var row = CreateRow();

                    generator.GenerateGaps(row);
                    generator.GenerateRandomObjects(row);
                }
            }
        }
        else
        {
            CreateRow();
        }
    }

    /// <summary>
    /// Возвращает список преднастроенных участков для текущего уровня,
    /// которые удовлетворяют заданным условиям частоты и кратности
    /// </summary>
    List<LevelPresetParams> GetPossiblePresets(int generationCount)
    {
        var settings = activeSettings;

        var presetsPossible = new List<LevelPresetParams>();

        for (int i = 0; i < settings.presetsParams.Count; i++)
        {
            var presetParam = settings.presetsParams[i];
            if (presetCounters[i] > 0 || presetParam.infinite)
            {
                var delta = Mathf.Abs(presetParam.intervalMax - presetParam.intervalMin);
                var temp = Mathf.Abs(generationCount - presetParam.intervalMin);

                var chance = presetParam.basicChance + (float)temp / delta;
                if (generationCount < presetParam.intervalMin)
                    chance = 0;

                var roll = Random.Range(0f, 0.99f);

                if (chance > roll)
                    presetsPossible.Add(presetParam);
            }
        }

        return presetsPossible;
    }

    public static Block GetBlock(Vector3 serachPosition)
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
        if (row >= 0 && row < singlton.rows.Count)
            if (line >= 0 && line < singlton.rows[row].blocks.Count)
                return singlton.rows[row].blocks[line];
        return null;
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

    public static void SwitchLevel(int number)
    {
        level = number;
        levelSteps = activeSettings.levelSteps;

        singlton.presetCounters = new int[activeSettings.presetsParams.Count];
        for (int i = 0; i < singlton.presetCounters.Length; i++)
            singlton.presetCounters[i] = activeSettings.presetsParams[i].count;

        UI.MarkNewLevel();
    }

    public static void EnableGeneration()
    {
        generationEnabled = true;
    }

    public static void DisableGeneration()
    {
        generationEnabled = false;
    }
}

[System.Serializable]
public class LevelGenerator : object
{
    public LevelGenerationSettings generationSettings { get => Level.activeSettings.generationSettings; }

    public Coin coinPref;
    public Coin coinArgentPref;

    public int coinSpawnCounter = 0;

    public int gapSpawnCounter = 0;
    public int gapTempCounter = 0;
    public int gapTempWidth = 1;
    public int[] gapTempLineIndexes;

    public void GenerateRandomObjects(Row row)
    {
        coinSpawnCounter++;

        var blocks = row.GetSolidBlocks();
        if (blocks.Count == 0)
            return;

        if (coinSpawnCounter >= generationSettings.coinMinInterval
            && coinSpawnCounter <= generationSettings.coinMaxInterval)
        {
            var delta = Mathf.Abs(generationSettings.coinMaxInterval - generationSettings.coinMinInterval);
            var temp = Mathf.Abs(coinSpawnCounter - generationSettings.coinMinInterval);
            var chance = generationSettings.coinBasicChance + (float)temp / delta;
            var roll = Random.Range(0f, 0.99f);

            if (chance >= roll)
            {
                int rand = Random.Range(0, blocks.Count);
                var block = blocks[rand];

                chance = generationSettings.coinArgentChance;
                roll = Random.Range(0f, 0.99f);
                Coin coin;
                if (chance >= roll)
                    coin = Object.Instantiate(coinArgentPref, block.transform);
                else
                    coin = Object.Instantiate(coinPref, block.transform);

                coin.transform.localPosition = Vector3.zero;
                coinSpawnCounter = 0;
            }
        }
    }

    public void GenerateGaps(Row row)
    {
        if (gapTempCounter == 0)
        {
            gapSpawnCounter++;
            if (gapSpawnCounter >= generationSettings.gapMinInterval
                && gapSpawnCounter <= generationSettings.gapMaxInterval)
            {
                var delta = Mathf.Abs(generationSettings.gapMaxInterval - generationSettings.gapMinInterval);
                var temp = Mathf.Abs(gapSpawnCounter - generationSettings.gapMinInterval);
                var chance = generationSettings.gapBasicChance + temp / delta;
                var roll = Random.Range(0f, 0.99f);

                if (chance >= roll)
                {
                    var rollRowGap = generationSettings.gapRowChance > Random.Range(0, 0.99f);
                    if (rollRowGap && generationSettings.allowRowGap)
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
                        gapTempCounter = Random.Range(generationSettings.gapMinLength, 
                                                      generationSettings.gapMaxLength + 1);

                        var rollSplit = generationSettings.gapSplitChance > Random.Range(0, 0.99f);

                        if (rollSplit)
                            gapTempLineIndexes = new int[] { 0, 2 };
                        else
                            gapTempLineIndexes = new int[] { 1 };
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