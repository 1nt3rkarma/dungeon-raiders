using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviourExtended
{
    public static Player singlton;

    public static bool restrictControlls;

    public static List<Item> inventory;
    public const int inventorySize = 3;

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
        get => PlayerPrefs.GetInt("hints", 0) == 1 ? true : false;

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

    public static bool musicOn
    {
        get => PlayerPrefs.GetInt("music", 1) == 1 ? true : false;

        set
        {
            Debug.Log($"Вкл. музыку: {value}");
            EnableMusic(value);
            if (value)
                PlayerPrefs.SetInt("music", 1);
            else
                PlayerPrefs.SetInt("music", 0);
        }
    }

    public static bool soundsOn
    {
        get => PlayerPrefs.GetInt("sounds", 1) == 1 ? true : false;

        set
        {
            Debug.Log($"Вкл. звуки эффектов: {value}");
            EnableSounds(value);
            if (value)
                PlayerPrefs.SetInt("sounds", 1);
            else
                PlayerPrefs.SetInt("sounds", 0);
        }
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

    public AudioMixer audioMixer;
    public GameResources resourceFile;

    public AudioSource audioSourceMusic;
    public AudioSource audioSourceEffects;

    #region Реакции на события

    #region События движка Unity

    void Awake()
    {
        Init(this);

        SubsribeToGameEvents();
    }

    void Start()
    {
        Debug.Log($"Музыка вкл.: {musicOn}");
        EnableMusic(musicOn);
        Debug.Log($"Звуки эффектов вкл.: {soundsOn}");
        EnableSounds(soundsOn);
    }

    void OnDestroy()
    {
        UnsubsribeToGameEvents();
    }

    void Update()
    {
        stepsLevelView = stepsLevel;
    }

    #endregion

    #region Расширенные события

    #region События ввода

    protected override void OnTapPress()
    {
        if (controllEnabled && !restrictControlls)
            OrderJump();
        restrictControlls = false;
    }

    protected override void OnTapRelease()
    {

    }

    protected override void OnSingleTap()
    {

    }

    protected override void OnDoubleTap()
    {

    }

    protected override void OnTapHold()
    {

    }

    protected override void OnSwipe(SwipeDirections direction)
    {
        if (controllEnabled && !restrictControlls)
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
        restrictControlls = false;
    }

    #endregion

    #region События игровой логики

    #endregion

    #endregion

    #endregion

    #region Методы

    public static void Init(Player instance)
    {
        if (singlton != null)
            Destroy(singlton.gameObject);
        singlton = instance;

        coins = 0;
        stepsLevel = 0;
        stepsSession = 0;

        LoadInventory();
    }

    #region Работа с инвентарем

    public static void UseItem(int index)
    {
        UseItem(inventory[index]);
    }

    public static void UseItem(Item item)
    {
        item.Use();
    }

    public static void AddItem(Item item)
    {
        if (item != null)
        {
            inventory.Add(item);
            UI.UpdateInventoryUI();
        }
    }

    public static void RemoveItem(Item item)
    {
        inventory.Remove(item);
        UI.UpdateInventoryUI();
    }

    public static void ClearInventory()
    {
        inventory.Clear();
        UI.UpdateInventoryUI();
    }

    public static void LoadInventory()
    {
        inventory = new List<Item>();

        for (int i = 0; i < inventorySize; i++)
        {
            var itemID = PlayerPrefs.GetInt($"inventory{i}", -1);
            if (itemID > -1)
            {
                var item = singlton.resourceFile.GetItem(itemID);
                AddItem(item);
            }
        }
    }

    public static void SaveInventory()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            if (i < inventory.Count)
            {
                var item = inventory[i];
                var itemID = singlton.resourceFile.GetItemDataID(item);

                PlayerPrefs.SetInt($"inventory{i}", itemID);
            }
            else
            {
                PlayerPrefs.SetInt($"inventory{i}", -1);
            }
        }
    }

    #endregion

    #region Взаимодействие с Героем и Уровнем

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

    #endregion

    static void EnableMusic(bool mode)
    {
        if (mode)
            singlton.audioMixer.SetFloat("MusicVolume", 0);
        else
            singlton.audioMixer.SetFloat("MusicVolume", -80);
    }

    static void EnableSounds(bool mode)
    {
        if (mode)
            singlton.audioMixer.SetFloat("SoundsVolume", 0);
        else
            singlton.audioMixer.SetFloat("SoundsVolume", -80);
    }

    public static void PlaySound(AudioClip sound)
    {
        singlton.audioSourceEffects.PlayOneShot(sound);
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
