using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    static Player singlton;

    public Hero hero;

    public bool controllEnabled = true;

    public int steps = 0;
    public int coins = 0;

    void Awake()
    {
        if (singlton != null)
            Destroy(singlton.gameObject);
        singlton = this;

        hero.player = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OrderLeapHero(MoveDirections direction)
    {
        hero.Leap(direction);
    }

    public void CountStep()
    {

    }

    public static void AddCoins(int amount)
    {
        singlton.coins += amount;
    }

}

public enum MoveDirections { None = 0, Right = 1, Left = -1}
