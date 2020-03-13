using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressUI : MonoBehaviour
{
    public ResourceBar bar;
    public Text label;

    private void Update()
    {
        label.text = $"LEVEL {Level.level}";
        var max = Level.levelSteps;
        var steps = Player.stepsOnLevel;

        bar.SetValue((float)steps / max);
    }
}
