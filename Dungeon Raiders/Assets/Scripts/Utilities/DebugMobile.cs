using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMobile : MonoBehaviour
{
    static DebugMobile singlton;

    public Text label1;
    public Text label2;
    public Text label3;

    public Text messageText;

    private void Awake()
    {
        singlton = this;
    }

    private void Start()
    {
        Log("Test message");
    }

    public static void PrintLabel1(string message)
    {
        if (singlton)
            singlton.label1.text = message;
    }

    public static void PrintLabel2(string message)
    {
        if (singlton)
            singlton.label2.text = message;
    }

    public static void PrintLabel3(string message)
    {
        if (singlton)
            singlton.label3.text = message;
    }

    public static void Log(string message)
    {
        if (singlton)
            singlton.messageText.text = message;
    }

    public void NormalTime()
    {
        Time.timeScale = 1;
    }

    public void SlowTime()
    {
        Time.timeScale = 0.5f;
    }

    public void SwitchSwipeMode()
    {
        UserMobileController.IsoMode = !UserMobileController.IsoMode;
        DebugMobile.Log($"Isometric swipe: {UserMobileController.IsoMode}");
    }
}
