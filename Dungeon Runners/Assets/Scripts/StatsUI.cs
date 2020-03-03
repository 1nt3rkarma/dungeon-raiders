using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    public Player player;

    public Text coinCounterText;

    void Update()
    {
        coinCounterText.text = player.coins.ToString();
    }
}
