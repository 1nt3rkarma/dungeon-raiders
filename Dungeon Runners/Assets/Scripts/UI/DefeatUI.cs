using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefeatUI : MonoBehaviour
{
    public Text stepsText;
    public Text coinCountText;

    public void OnClickRestart()
    {
        UI.ClickRestart();
    }

    public void OnClickExit()
    {
        UI.ClickExit();
    }
}
