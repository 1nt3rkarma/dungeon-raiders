using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressUI : MonoBehaviour
{
    public ResourceBar bar;
    public Text label;

    public TutorialUI tutorialUI;

    private void Update()
    {
        if (tutorialUI != null)
        {
            label.text = $"TUTORIAL";
            var max = tutorialUI.actionsMax;
            var steps = tutorialUI.actionsTotal;
            bar.SetValue((float)steps / max);
        }
        else
        {
            label.text = $"LEVEL {Level.level}";
            var max = Level.levelSteps;
            var steps = Player.steps;
            bar.SetValue((float)steps / max);
        }
    }
}
