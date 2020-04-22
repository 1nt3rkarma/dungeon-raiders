using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefeatUI : MonoBehaviour
{
    public GameObject newRecordLabel;
    public bool newRecord;

    public Text textProgress;
    public Text textEnemies;
    public Text textGold;

    public AudioClip progressJingle;
    public AudioClip enemiesJingle;
    public AudioClip goldJingle;

    public Animator animator;
    public string skipTag = "skip";

    public float timePerRoll = 0.5f;

    public void Show(bool showNewRecordLabel)
    {
        textProgress.text = "0";
        textEnemies.text = "0";
        textGold.text = "0";

        newRecordLabel.gameObject.SetActive(false);

        newRecord = showNewRecordLabel;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ShowRecordLabel()
    {
        newRecordLabel.gameObject.SetActive(newRecord);
    }

    public void RollProgress()
    {
        StartCoroutine(RollStatText(timePerRoll,Player.stepsTotal,textProgress));
    }

    public void PlayProgressJingle()
    {
        Player.PlaySound(progressJingle);
    }

    public void PlayEnemiesJingle()
    {
        Player.PlaySound(enemiesJingle);
    }

    public void RollEnemies()
    {
        StartCoroutine(RollStatText(timePerRoll, Player.enemies, textEnemies));
    }

    public void PlayGoldJingle()
    {
        Player.PlaySound(goldJingle);
    }

    public void RollGold()
    {
        StartCoroutine(RollStatText(timePerRoll, Player.coins, textGold));
    }

    public void Skip()
    {
        StopAllCoroutines();

        textProgress.text = Player.stepsTotal.ToString();
        textEnemies.text = Player.enemies.ToString();

        if(Player.coins > 0)
            textGold.text = $"+{Player.coins}";

        animator.SetTrigger(skipTag);
    }

    IEnumerator RollStatText(float duration, int finalValue, Text textObject)
    {
        //Debug.Log($"{textObject.name} final value: {finalValue}");

        float timer = 0;
        float speed = finalValue / duration;
        float value = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            value += speed * Time.deltaTime;

            if (textObject == textGold && finalValue > 0)
                textObject.text = $"+{(int)value}";
            else
                textObject.text = $"{(int)value}";

            yield return null;
        }
        value = finalValue;
        if (textObject == textGold && finalValue > 0)
            textObject.text = $"+{finalValue}";
        else
            textObject.text = $"{finalValue}";
    }
}
