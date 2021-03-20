using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TextField = TMPro.TextMeshProUGUI;

public class UIManagerMainMenu : MonoBehaviour
{
    public MainMenuDirector director;

    public Image maskScene;
    public Image maskFull;
    public TextField logo;
    public Button toDungeonButton;

    private DoTweenGraphicColorizer maskColorizer;
    private DoTweenGraphicColorizer logoColorizer;

    private void Awake()
    {
        logoColorizer = new DoTweenGraphicColorizer(logo);
        maskColorizer = new DoTweenGraphicColorizer(maskScene);

        toDungeonButton.onClick.AddListener(OnClickToDungeon);
    }

    private void OnDestroy()
    {
        logoColorizer?.Stop();
        maskColorizer?.Stop();
    }

    private void OnClickToDungeon()
    {
        StartCoroutine(GoToDungeonRoutine());
    }

    IEnumerator GoToDungeonRoutine()
    {
        director.PlayScenarioToTheDungeon();

        yield return new WaitForSeconds(director.pathToTheDungeon.totalDuration);

        SceneManager.LoadScene("Game");
    }

    public void FadeLogo(float alpha = 0, float overTime = 0)
    {
        if (overTime > 0)
            logoColorizer.Transit(alpha, overTime, DG.Tweening.Ease.Flash);
        else
            logoColorizer.SetAlpha(alpha);
    }

    public void EnableSceneTint(float overTime = 0)
    {
        if (maskScene.color.a == 0.5f) return;

        maskColorizer.TargetGraphic = maskScene;
        if (overTime > 0)
            maskColorizer.Transit(0.5f, overTime, DG.Tweening.Ease.Flash);
        else
            maskColorizer.SetAlpha(0.5f);
    }
    public void DisableSceneTint(float overTime = 0)
    {
        if (maskScene.color.a == 0) return;

        maskColorizer.TargetGraphic = maskScene;
        if (overTime > 0)
            maskColorizer.Transit(0, overTime, DG.Tweening.Ease.Flash);
        else
            maskColorizer.SetAlpha(0);
    }

    public void EnableSceneMask(float overTime = 0)
    {
        if (maskScene.color.a == 1) return;

        maskColorizer.TargetGraphic = maskScene;
        if (overTime > 0)
            maskColorizer.Transit(1, overTime, DG.Tweening.Ease.Flash);
        else
            maskColorizer.SetAlpha(1);

    }
    public void DisableSceneMask(float overTime = 0)
    {
        if (maskScene.color.a == 0) return;

        maskColorizer.TargetGraphic = maskScene;
        if (overTime > 0)
            maskColorizer.Transit(0, overTime, DG.Tweening.Ease.Flash);
        else
            maskColorizer.SetAlpha(0);

    }
}
