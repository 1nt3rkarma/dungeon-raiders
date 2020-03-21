using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefeatUI : MonoBehaviour
{
    public Text newRecordText;

    public Text stepsText;
    public Text coinText;

    public void Show(bool showNewRecordLabel)
    {
        coinText.text = Player.coins.ToString();
        stepsText.text = $"YOU'VE MADE\n{Player.stepsSession.ToString()} STEPS";

        newRecordText.gameObject.SetActive(showNewRecordLabel);

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
