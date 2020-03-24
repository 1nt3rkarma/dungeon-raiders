using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviourExtended
{
    public static Player singlton;

    public static bool controllEnabled = false;

    public static bool needTutorial
    {
        get => PlayerPrefs.GetInt("tutorial", 1) == 1 ? true : false;

        set
        {
            if (value)
                PlayerPrefs.SetInt("tutorial", 1);
            else
                PlayerPrefs.SetInt("tutorial", 0);
        }
    }

    public static bool showHints
    {
        get => PlayerPrefs.GetInt("hints", 1) == 1 ? true : false;

        set
        {
            Debug.Log($"Показывать подсказки: {value}");
            if (value)
                PlayerPrefs.SetInt("hints", 1);
            else
                PlayerPrefs.SetInt("hints", 0);
        }
    }

    public static int coinsTotal
    {
        get => PlayerPrefs.GetInt("coins", 0);

        set => PlayerPrefs.SetInt("coins", value);
    }

    public static int stepsBest
    {
        get => PlayerPrefs.GetInt("steps", 0);

        set => PlayerPrefs.SetInt("steps", value);
    }

    public static int coins = 0;
    public static int stepsLevel = 0;
    public int stepsLevelView;
    public static int stepsSession = 0;

    #region Реакции на события

    #region События движка Unity

    void Awake()
    {
        if (singlton != null)
            Destroy(singlton.gameObject);
        singlton = this;

        coins = 0;
        stepsLevel = 0;
        stepsSession = 0;

        SubsribeToGameEvents();
    }

    void OnDestroy()
    {
        UnsubsribeToGameEvents();
    }

    private void Update()
    {
        stepsLevelView = stepsLevel;
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
        if (controllEnabled)
            OrderJump();
    }

    protected override void OnDoubleTap()
    {

    }

    protected override void OnTapHold()
    {

    }

    protected override void OnSwipe(SwipeDirections direction)
    {
        if (controllEnabled)
            switch (direction)  
            {
                case SwipeDirections.Right:
                    OrderLeap(LeapDirections.Right);
                    break;
                case SwipeDirections.Left:
                    OrderLeap(LeapDirections.Left);
                    break;
                case SwipeDirections.Up:
                    OrderAttack();
                    break;
                default:
                    break;
            }
    }

    #endregion

    #endregion

    #region Методы

    public static void OrderAttack()
    {
        if (Hero.singlton.isListening)
            Hero.singlton.MeleeAttack();
    }

    public static void OrderJump()
    {
        if (Hero.singlton.isListening)
            Hero.singlton.Jump();
    }

    public static void OrderLeap(LeapDirections direction)
    {
        if (Hero.singlton.isListening)
            Hero.singlton.Leap(direction);
    }

    public static void AddStep()
    {
        stepsLevel++;
        if (stepsLevel == Level.levelSteps)
        {
            Level.SwitchLevel();
            stepsSession += stepsLevel;
            stepsLevel = 0;
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
        controllEnabled = false;
        CameraController.FocusHero(1f);

        coinsTotal += coins;

        stepsSession += stepsLevel;

        UI.SlideAwayMainUI();

        if (stepsSession > stepsBest)
        {
            stepsBest = stepsSession;
            UI.ShowDefeatUI(true);
        }
        else
        {
            UI.ShowDefeatUI(false);
        }

    }

    public static void ReloadLevel()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index);
    }

    public static void ExitGame()
    {
        Application.Quit();
    }

    #endregion

}

public enum LeapDirections { None = 0, Right = 1, Left = -1}
