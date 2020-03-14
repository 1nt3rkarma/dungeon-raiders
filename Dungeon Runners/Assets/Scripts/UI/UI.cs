using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    static UI singlton;

    public DefeatUI defeatUI;
    public GameObject mainUI;

    private void Awake()
    {
        InitSinglton(this);
    }

    void InitSinglton(UI instance)
    {
        singlton = instance;
    }

    public static void HideMainUI()
    {
        singlton.mainUI.SetActive(false);
    }

    public static void ShowMainUI()
    {
        singlton.mainUI.SetActive(true);
    }

    public static void ShowDefeatUI()
    {
        singlton.defeatUI.gameObject.SetActive(true);
        singlton.defeatUI.coinCountText.text = Player.coins.ToString();
        singlton.defeatUI.stepsText.text = $"YOU'VE MADE\n{Player.stepsTotal.ToString()} STEPS";
    }
    public static void HideDefeatUI()
    {
        singlton.defeatUI.gameObject.SetActive(false);
    }

    public static void ClickRestart()
    {
        Player.ReloadLevel();
    }

    public static void ClickExit()
    {
        // Вы вышли из игры
    }

}
