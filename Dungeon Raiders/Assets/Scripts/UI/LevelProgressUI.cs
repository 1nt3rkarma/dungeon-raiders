using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressUI : MonoBehaviour
{
    public ResourceBar bar;
    public Text label;

    public int m, s;


    private void Update()
    {
        label.text = $"LEVEL {Level.level}";
        var max = Level.levelSteps;
        m = max;
        var steps = Player.steps;
        s = steps;

        bar.SetValue((float)steps / max);
    }
}
