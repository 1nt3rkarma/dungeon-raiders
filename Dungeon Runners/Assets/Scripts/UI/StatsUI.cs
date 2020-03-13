using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    public Player player;

    public Text coinCounterText;

    public ResourceBar healthBar;
    public ResourceBar manaBar;

    void Update()
    {
        coinCounterText.text = Player.coins.ToString();
        if (player.hero.isAlive)
            healthBar.SetValue(player.hero.health / player.hero.healthMax);
        else
            healthBar.SetValue(0);
    }
}
