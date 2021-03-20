using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesPanel : MonoBehaviour
{
    public Text coinCounterText;

    [Tooltip("Primary hero resource (health)")]
    public ResourceBar healthBar;
    private float lastHealth;

    [Tooltip("Secondary hero resource (mana/faith/fury etc.)")]
    public ResourceBar supplyBar;
    private float lastSupply;

    public float fillDuration = 0.1f;
    public float fillDelay = 0.7f;

    private Hero hero => Hero.singlton;

    private void Start()
    {
        lastHealth = hero.Health.Percentage;
        healthBar.SetValue(lastHealth);

        lastSupply = hero.Supply.Percentage;
        supplyBar.SetValue(lastSupply);
    }

    void Update()
    {
        coinCounterText.text = Player.coins.ToString();

        var health = hero.Health.Percentage;
        //if (health > lastHealth)
        //    healthBar.SetValue(health, fillDelay, fillDuration);
        //else
        //    healthBar.SetValue(health, fillDuration, fillDelay);
        healthBar.SetValue(health, fillDuration, fillDuration);
        lastHealth = health;

        var supply = hero.Supply.Percentage;
        //if (health > lastSupply)
        //    supplyBar.SetValue(supply, fillDelay, fillDuration);
        //else
        //    supplyBar.SetValue(supply, fillDuration, fillDelay);
        supplyBar.SetValue(supply, fillDuration, fillDuration);
        lastSupply = supply;
    }
}
