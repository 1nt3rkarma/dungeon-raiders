using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Button hintButton;
    public Button hintButtonDisabled;

    private void Start()
    {
        bool hints = Player.showHints;

        hintButton.gameObject.SetActive(hints);
        hintButtonDisabled.gameObject.SetActive(!hints);
    }

    public void OnClickHints(bool mode)
    {
        hintButton.gameObject.SetActive(mode);
        hintButtonDisabled.gameObject.SetActive(!mode);

        Player.showHints = mode;
    }
}
