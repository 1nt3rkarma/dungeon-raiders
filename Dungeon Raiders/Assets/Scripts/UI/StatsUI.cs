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
        coinCounterText.text = Player.coins.ToString();
        if (Hero.singlton.isAlive)
            healthBar.SetValue(Hero.singlton.health / Hero.singlton.healthMax);
        else
            healthBar.SetValue(0);
    }
}
