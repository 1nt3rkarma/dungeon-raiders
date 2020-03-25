using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI : MonoBehaviour
{
    static UI singlton;
    static bool fisrtLaunch = true;

    public DefeatUI defeatUI;
    public GameplayUI gameplayUI;
    public StartMenuUI startUI;
    public TutorialUI tutorialUI;

    private void Awake()
    {
        InitSinglton(this);
    }

    private void Start()
    {
        if (fisrtLaunch)
        {
            startUI.PopUp();
            fisrtLaunch = false;
        }
    }

    void InitSinglton(UI instance)
    {
        singlton = instance;
    }

    public static void ShowGameplayUI()
    {
        singlton.gameplayUI.gameObject.SetActive(true);
        singlton.gameplayUI.PopUp();
    }
    public static void HideGameplayUI()
    {
        singlton.gameplayUI.gameObject.SetActive(false);
    }
    public static void SlideAwayMainUI()
    {
        singlton.gameplayUI.SlideAway();
    }
    public static void MarkNewLevel()
    {
        singlton.gameplayUI.NewLevel();
    }

    public static void ShowDefeatUI(bool newBest)
    {
        singlton.defeatUI.Show(newBest);
    }
    public static void HideDefeatUI()
    {
        singlton.defeatUI.Hide();
    }

    public static void UpdateInventoryUI()
    {
        singlton.gameplayUI.inventoryUI.UpdateUI();
    }

    public void OnClickStart()
    {
        CameraController.ResetCamera(1);
        Player.ContinueMoving();
        Player.controllEnabled = true;
        startUI.Fade();

        if (Player.needTutorial || Player.showHints)
        {
            tutorialUI.Run();
        }
        else
        {
            Destroy(tutorialUI.gameObject);
            gameplayUI.gameObject.SetActive(true);
        }
    }

    public static GameObject GetUIElement(Vector2 screenPosition)
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);

        pointer.position = screenPosition;

        List<RaycastResult> hitObjects = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointer, hitObjects);

        if (hitObjects.Count > 0)
            return hitObjects[0].gameObject;
        else
            return null;
    }

    public void OnClickRestart()
    {
        Player.ReloadLevel();
    }

    public void OnClickExit()
    {
        Player.ExitGame();
    }
}
