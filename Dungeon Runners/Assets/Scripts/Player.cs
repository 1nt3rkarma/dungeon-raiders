using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviourExtended
{
    public static Player singlton;

    public Hero hero;

    public static bool controllEnabled = true;

    public static int coins = 0;
    public static int stepsOnLevel = 0;
    public static int stepsTotal = 0;

    #region Реакции на события

    #region События движка Unity

    void Awake()
    {
        if (singlton != null)
            Destroy(singlton.gameObject);
        singlton = this;

        coins = 0;
        stepsOnLevel = 0;
        stepsTotal = 0;

        SubsribeToGameEvents();
    }

    private void Start()
    {
        CameraController.ResetCamera(1);
    }

    void OnDestroy()
    {
        UnsubsribeToGameEvents();
    }

    #endregion

    #region Игровые события

    protected override void OnTapPress()
    {

    }

    protected override void OnTapRelease()
    {

    }

    protected override void OnSingleTap()
    {
        OrderJump();
    }

    protected override void OnDoubleTap()
    {
        OrderAttack();
    }

    protected override void OnTapHold()
    {

    }

    protected override void OnSwipe(SwipeDirections direction)
    {
        switch (direction)  
        {
            case SwipeDirections.Right:
                OrderLeap(LeapDirections.Right);
                break;
            case SwipeDirections.Left:
                OrderLeap(LeapDirections.Left);
                break;
            default:
                break;
        }
    }

    #endregion

    #endregion

    #region Методы

    public void OrderAttack()
    {
        hero.MeleeAttack();
    }

    public void OrderJump()
    {
        hero.Jump();
    }

    public void OrderLeap(LeapDirections direction)
    {
        hero.Leap(direction);
    }

    public static void AddStep()
    {
        stepsOnLevel++;
        if (stepsOnLevel == Level.levelSteps)
        {
            Level.SwitchLevel();
            stepsTotal += stepsOnLevel;
            stepsOnLevel = 0;
        }
    }

    public static void AddCoins(int amount)
    {
        coins += amount;
    }

    public static void StopMoving()
    {
        Level.StopFlow();
    }

    public static void ContinueMoving()
    {
        Level.RunFlow();
    }

    public static void Defeat()
    {
        CameraController.FocusHero(1f);
        stepsTotal += stepsOnLevel;
        UI.ShowDefeatUI();
        UI.HideMainUI();
    }

    public static void ReloadLevel()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index);
    }

    #endregion

}

public enum SwipeDirections { None, Right, Left, Up, Down }

public enum LeapDirections { None = 0, Right = 1, Left = -1}
