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
        hintButton.gameObject.SetActive(Player.showHints);
        hintButtonDisabled.gameObject.SetActive(!Player.showHints);

        // Кнопка "Вкл. музыку"
        musicButton.gameObject.SetActive(Player.musicOn);
        musicButtonDisabled.gameObject.SetActive(!Player.musicOn);

        // Кнопка "Вкл. звуковые эффекты"
        soundsButton.gameObject.SetActive(Player.soundsOn);
        soundsButtonDisabled.gameObject.SetActive(!Player.soundsOn);
    }

    public void OnClickHints(bool mode)
    {
        hintButton.gameObject.SetActive(mode);
        hintButtonDisabled.gameObject.SetActive(!mode);

        Player.showHints = mode;
    }

    public void OnClickMusic(bool mode)
    {
        musicButton.gameObject.SetActive(mode);
        musicButtonDisabled.gameObject.SetActive(!mode);

        Player.musicOn = mode;
    }

    public void OnClickSounds(bool mode)
    {
        soundsButton.gameObject.SetActive(mode);
        soundsButtonDisabled.gameObject.SetActive(!mode);

        Player.soundsOn = mode;
    }
}
