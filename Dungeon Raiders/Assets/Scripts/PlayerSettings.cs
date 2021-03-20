using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerSettings
{
    public static bool SoundsOn
    {
        get => PlayerPrefs.GetInt("sounds", 1) == 1 ? true : false;

        set
        {
            Player.EnableSounds(value);
            PlayerPrefs.SetInt("sounds", value ? 1 : 0);
        }
    }

    public static bool MusicOn
    {
        get => PlayerPrefs.GetInt("music", 1) == 1 ? true : false;

        set
        {
            Player.EnableMusic(value);
            PlayerPrefs.SetInt("music", value ? 1 : 0);
        }
    }

    public static bool ShowHints
    {
        get => PlayerPrefs.GetInt("hints", 0) == 1 ? true : false;

        set
        {
            //Debug.Log($"Показывать подсказки: {value}");
            PlayerPrefs.SetInt("hints", value ? 1 : 0);
        }
    }
}
