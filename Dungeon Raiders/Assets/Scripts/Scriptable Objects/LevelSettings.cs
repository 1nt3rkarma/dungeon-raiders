using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level N", menuName = "Level Settings")]
public class LevelSettings : ScriptableObject
{
    public int levelSteps = 100;

    public LevelGenerationSettings generationSettings;

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
}

[System.Serializable]
public class LevelGenerationSettings : object
{
    [Header("Coin generation settings")]
    [Range(0, 1)]
    public float coinBasicChance = 0.1f;
    [Range(0, 1)]
    public float coinArgentChance = 0.1f;
    public int coinMinInterval = 5;
    public int coinMaxInterval = 9;

    [Header("Gaps generation settings")]

    [Range(0, 1)]
    public float gapBasicChance = 0.1f;
    public bool allowRowGap = true;
    [Range(0, 1)]
    public float gapRowChance = 0.5f;

    public int gapMinInterval = 11;
    public int gapMaxInterval = 13;

    [Range(0, 1)]
    public float gapSplitChance = 0.5f;
    public int gapMinLength = 2;
    public int gapMaxLength = 3;
}