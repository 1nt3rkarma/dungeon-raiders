using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    public Text coinCounterText;

    public ResourceBar healthBar;
    public ResourceBar manaBar;

    void Update()
    {
        coinCounterText.text = Player.singlton.hero.ToString();
        if (Player.singlton.hero.isAlive)
            healthBar.SetValue(Player.singlton.hero.health / Player.singlton.hero.healthMax);
        else
            healthBar.SetValue(0);
    }
}
