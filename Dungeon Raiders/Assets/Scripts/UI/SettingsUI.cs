using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Button hintButton;
    public Button hintButtonDisabled;

    public Button musicButton;
    public Button musicButtonDisabled;

    public Button soundsButton;
    public Button soundsButtonDisabled;

    private void Start()
    {
        // Кнопка "Показывать подсказки"
        hintButton.gameObject.SetActive(PlayerSettings.ShowHints);
        hintButtonDisabled.gameObject.SetActive(!PlayerSettings.ShowHints);

        // Кнопка "Вкл. музыку"
        musicButton.gameObject.SetActive(PlayerSettings.MusicOn);
        musicButtonDisabled.gameObject.SetActive(!PlayerSettings.MusicOn);

        // Кнопка "Вкл. звуковые эффекты"
        soundsButton.gameObject.SetActive(PlayerSettings.SoundsOn);
        soundsButtonDisabled.gameObject.SetActive(!PlayerSettings.SoundsOn);
    }

    public void OnClickHints(bool mode)
    {
        hintButton.gameObject.SetActive(mode);
        hintButtonDisabled.gameObject.SetActive(!mode);

        PlayerSettings.ShowHints = mode;
    }

    public void OnClickMusic(bool mode)
    {
        musicButton.gameObject.SetActive(mode);
        musicButtonDisabled.gameObject.SetActive(!mode);

        PlayerSettings.MusicOn = mode;
    }

    public void OnClickSounds(bool mode)
    {
        soundsButton.gameObject.SetActive(mode);
        soundsButtonDisabled.gameObject.SetActive(!mode);

        PlayerSettings.SoundsOn = mode;
    }
}
