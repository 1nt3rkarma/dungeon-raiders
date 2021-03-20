using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public static Level singleton;

    public static int level;
    public static int levelSteps;

    public static bool generationEnabled = true;
    public static int generationCount;
    public static LevelSettings ActiveSettings
    {
        get
        {
            if (level <= 0)
                return singleton.settings[0];
            else if (level >= singleton.settings.Count)
                return singleton.settings[singleton.settings.Count - 1];
            else
                return singleton.settings[level - 1];
        }
    }

    public float moveSpeedDefault = 1;
    [HideInInspector]
    public float moveSpeed = 1;
    private float shift = 0;

    public Row rowPref;
    public List<Row> rows;
    public LevelGenerator generator;

    public LevelDecorator decorator;

    public List<LevelSettings> settings;
    public int[] presetCounters;
    public LevelPreset lastPreset;


    public bool IsMoving => moveSpeed > 0;
    public int generationCountView;
    public bool generationEnabledView;

    Coroutine flowRoutine;
    DoTweenLevelSpeedHandler speedHandler;

    void Awake()
    {
        if (singleton != null)
            Destroy(singleton.gameObject);

        singleton = this;
        speedHandler = new DoTweenLevelSpeedHandler(this);

        foreach (var row in rows)
            decorator.Decorate(row);


        moveSpeed = 0;
        generationCount = 0;
        generationEnabled = true;
    }

    void OnDestroy()
    {
        speedHandler?.Stop();
    }

    void Start()
    {
        SwitchLevel(1);
    }

    void FixedUpdate()
    {
        if (!Mathf.Approximately(moveSpeed,0))
        {
            if (shift > -1)
            {
                shift -= moveSpeed * Time.fixedDeltaTime;
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
                    var instance = Instantiate(presetParam.presetPrefab);
                    lastPreset = presetParam.presetPrefab;

                    int index = ActiveSettings.presetsParams.IndexOf(presetParam);
                    if (!presetParam.infinite)
                        presetCounters[index]--;

                    generationCount = -instance.rows.Count;

                    foreach (var row in instance.rows)
                    {
                        Add(row);
                        decorator.Decorate(row);
                    }

                    foreach (var randomObject in instance.randomObjects)
                        randomObject.Generate();

                    Destroy(instance.gameObject);
                }
                else
                {
                    var row = CreateRow();

                    if (decorator.spawnCounter > -2 || generator.IsLineGapping)
                        generator.GenerateGaps(row);
                    generator.GenerateRandomObjects(row);
                    decorator.Decorate(row);
                }
            }
        }
        else
        {
            var row = CreateRow();
            decorator.Decorate(row);
        }
    }

    /// <summary>
    /// Возвращает список преднастроенных участков для текущего уровня,
    /// которые удовлетворяют заданным условиям частоты и кратности
    /// </summary>
    List<LevelPresetParams> GetPossiblePresets(int generationCount)
    {
        var settings = ActiveSettings;

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
        if (singleton == null) return null;

        Block wanted = singleton.rows[1].blocks[1];
        var sqrDistanceMin = 100f;

        foreach (var row in singleton.rows)
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
        for (int i = 0; i < singleton.rows.Count; i++)
            if (singleton.rows[i].blocks.Contains(block))
                return i;

        return -1;
    }

    public static Block GetBlock(int row, int line)
    {
        if (row >= 0 && row < singleton.rows.Count)
            if (line >= 0 && line < singleton.rows[row].blocks.Count)
                return singleton.rows[row].blocks[line];
        return null;
    }

    public static void StopFlow(float overTime = 0)
    {
        //singleton.isMoving = false;
        if (overTime > 0)
            singleton.speedHandler.SpeedMove(0, overTime);
        else
            singleton.moveSpeed = 0;
    }

    public static void RunFlow(float overTime = 0)
    {
        if (overTime > 0)
            singleton.speedHandler.SpeedMove(singleton.moveSpeedDefault, overTime);
        else
            singleton.moveSpeed = singleton.moveSpeedDefault;
        //singleton.isMoving = true;
    }

    public static void SwitchLevel()
    {
        SwitchLevel(level + 1);
    }

    public static void SwitchLevel(int number)
    {
        level = number;
        levelSteps = ActiveSettings.levelSteps;

        singleton.presetCounters = new int[ActiveSettings.presetsParams.Count];
        for (int i = 0; i < singleton.presetCounters.Length; i++)
            singleton.presetCounters[i] = ActiveSettings.presetsParams[i].count;

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
    public LevelGenerationSettings generationSettings { get => Level.ActiveSettings.generationSettings; }

    public Coin coinPref;
    public Coin coinArgentPref;

    public int coinSpawnCounter = 0;

    public int gapSpawnCounter = 0;
    public int gapTempCounter = 0;
    public int gapTempWidth = 1;
    public int[] gapTempLineIndexes;

    public bool IsSplitting => gapTempLineIndexes.Length > 1 && gapTempCounter > 0;
    public bool IsLineGapping => gapTempLineIndexes.Length == 1 && gapTempCounter > 0;

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

[System.Serializable] 
public class LevelDecorator
{
    public int spawnCounter = 0;

    public List<TerrainDecoration> decorationsPrefabs;

    public TerrainDecoration lastVariation;
    public bool repeatsAllowed = false;

    public void Decorate(Row row)
    {
        // Инкрементируем счетчик генерации
        if (spawnCounter < 0)
            spawnCounter++;

        // Если он все еще меньше 0 (например, последняя декорация
        // была размером в 2 и более блоков), то ничего не делаем больше
        if (spawnCounter < 0) return;

        // Определяем предыдущий ряд
        var thisIndex = Level.singleton.rows.IndexOf(row);
        var previousIndex = thisIndex - 1;
        var previousRow = previousIndex > 0 ?Level.singleton.rows[thisIndex - 1] : null;
        var thisEmpty = row.ContainsEmptyBorder;
        var previousEmpty = previousRow ? previousRow.ContainsEmptyBorder : false;

        if (previousRow != null && previousEmpty)
            repeatsAllowed = true;

        // Ищем возможные варианты декораций исходя из условий текущего и предыдущего ряда
        var options = GetPossibleDecorations(thisEmpty, previousEmpty);

        //Debug.Log($"Decorating a row: thisEmpty: {thisEmpty}, previousEmpty: {previousEmpty}, options: {options.Count} (repeats allowed: {repeatsAllowed})");

        // Если доступных вариантов декораций нет то ничего не делаем
        if (options.Count <= 0)
        {
            spawnCounter = 0;
            return;
        }

        // Если есть, то выбираем случайный из них
        var randomIdex = Random.Range(0, options.Count);

        // Запоминаем параметры последней вариации
        var variation = options[randomIdex];
        spawnCounter -= variation.blockSize;
        repeatsAllowed = variation.allowRepeats;
        lastVariation = variation;

        // Создаем ее копию, привязываем ее к ряду и рандомизируем внещний облик
        var instance = GameObject.Instantiate(variation, row.decorationPivot);
        instance.Randomize();
        if (!previousEmpty && instance.frontFace != null)
            GameObject.Destroy(instance.frontFace.gameObject);

        GameObject.Destroy(instance);
    }

    public List<TerrainDecoration> GetPossibleDecorations(bool thisRowEmpty, bool previousRowEmpty)
    {
        var options = new List<TerrainDecoration>(decorationsPrefabs);

        foreach (var option in decorationsPrefabs)
        {
            if (option.avoidEmptyRowBorder && thisRowEmpty)
                options.Remove(option);
            if (option.avoidEmptyNeighborBorder && previousRowEmpty)
                options.Remove(option);
            if (!repeatsAllowed && option == lastVariation)
                options.Remove(option);
        }

        return options;
    }
}