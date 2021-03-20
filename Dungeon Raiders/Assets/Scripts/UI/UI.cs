using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI : MonoBehaviour
{
    static UI singleton;
    static bool fisrtLaunch = true;

    public DefeatUI defeatUI;
    public GameplayUI gameplayUI;
    public StartMenuUI startUI;
    public TutorialUI tutorialUI;
    public SettingsUI settingsUI;
    public ShopUI shopUI;

    public Image maskGameplay;
    public Image maskFull;
    private DoTweenGraphicColorizer maskGameplayColorizer;
    private DoTweenGraphicColorizer maskFullFader;

    private void Awake()
    {
        InitSinglton(this);

        startUI.gameObject.SetActive(true);

        gameplayUI.gameObject.SetActive(false);
        defeatUI.gameObject.SetActive(false);
        tutorialUI.gameObject.SetActive(false);
        settingsUI.gameObject.SetActive(false);
        shopUI.gameObject.SetActive(false);

        if (maskGameplay != null)
            maskGameplayColorizer = new DoTweenGraphicColorizer(maskGameplay);
        if (maskFull != null)
            maskFullFader = new DoTweenGraphicColorizer(maskFull);

    }

    private void Start()
    {
        if (fisrtLaunch)
        {
            startUI.PopUp();
            fisrtLaunch = false;
        }

        CameraController.FocusHero();

        DisableGameplayTint();
    }

    void InitSinglton(UI instance)
    {
        singleton = instance;
    }

    private void OnDestroy()
    {
        maskGameplayColorizer?.Stop();
        maskFullFader?.Stop();
    }

    public static void ShowGameplayUI()
    {
        singleton.gameplayUI.gameObject.SetActive(true);
        singleton.gameplayUI.PopUp();
    }
    public static void HideGameplayUI()
    {
        singleton.gameplayUI.gameObject.SetActive(false);
    }
    public static void SlideAwayMainUI()
    {
        singleton.gameplayUI.SlideAway();
    }
    public static void MarkNewLevel()
    {
        singleton.gameplayUI.NewLevel();
    }

    public static void ShowDefeatUI(bool newBest)
    {
        singleton.defeatUI.Show(newBest);
    }
    public static void HideDefeatUI()
    {
        singleton.defeatUI.Hide();
    }

    public void OnClickStart()
    {
        CameraController.ResetCamera(1);
        Player.ContinueMoving(0.2f);
        Player.controllEnabled = true;

        if (!Player.Data.tutorialPassed || PlayerSettings.ShowHints)
        {
            tutorialUI.Run();
            ShowGameplayUI();
        }
        else
        {
            Destroy(tutorialUI.gameObject);
            ShowGameplayUI();
        }

        startUI.Fade();
        Destroy(settingsUI.gameObject);
        Destroy(shopUI.gameObject);
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
        fisrtLaunch = true;
        Player.ExitDungeon();
    }

    public static void EnableGameplayTint(float overTime = 0)
    {
        if (singleton.maskGameplay.color.a == 0.5f) return;

        if (overTime > 0)
        {
            singleton.maskGameplayColorizer.Transit(0.5f, overTime, DG.Tweening.Ease.Flash);
        }
        else
        {
            singleton.maskGameplayColorizer.SetAlpha(0.5f);
        }
    }
    public static void DisableGameplayTint(float overTime = 0)
    {
        if (singleton.maskGameplay.color.a == 0) return;

        if (overTime > 0)
        {
            singleton.maskGameplayColorizer.Transit(0, overTime, DG.Tweening.Ease.Flash);
        }
        else
        {
            singleton.maskGameplayColorizer.SetAlpha(0);
        }
    }

    public static void EnableGameplayMask(float overTime = 0)
    {
        if (singleton.maskGameplay.color.a == 1) return;

        if (overTime > 0)
        {
            singleton.maskGameplayColorizer.Transit(1, overTime, DG.Tweening.Ease.Flash);
        }
        else
        {
            singleton.maskGameplayColorizer.SetAlpha(1);
        }
    }
    public static void DisableGameplayMask(float overTime = 0)
    {
        if (singleton.maskGameplay.color.a == 0) return;

        if (overTime > 0)
        {
            singleton.maskGameplayColorizer.Transit(0, overTime, DG.Tweening.Ease.Flash);
        }
        else
        {
            singleton.maskGameplayColorizer.SetAlpha(0);
        }
    }
}
