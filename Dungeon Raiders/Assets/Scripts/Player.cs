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
            //Debug.Log($"Вкл. музыку: {value}");
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

    public static int steps = 0;
    public static int enemies = 0;
    public static int coins = 0;

    public static int stepsTotal = 0;

    public AudioMixer audioMixer;
    public GameResources resourceFile;
    public static GameResources resources { get => singlton.resourceFile; }

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
        LoadInventory();

        EnableMusic(musicOn);

        EnableSounds(soundsOn);

        var musicCollection = singlton.resourceFile.music;
        var rand = Random.Range(0, musicCollection.Count);
        PlayMusic(musicCollection[rand]);
    }

    void OnDestroy()
    {
        UnsubsribeToGameEvents();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            AddCoinsTotal(100);

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (Time.timeScale == 1)
                Time.timeScale = 0.5f;
            else if (Time.timeScale == 0.5f)
                Time.timeScale = 0.25f;
            else
                Time.timeScale = 1f;
        }
    }

    #endregion

    #region Расширенные события

    #region События ввода

    protected override void OnPress()
    {

    }

    protected override void OnRelease()
    {
        if (controllEnabled && !restrictControlls)
            OrderBreakJump();

        restrictControlls = false;
    }

    protected override void OnStationary()
    {
        if (controllEnabled && !restrictControlls)
            OrderJump();
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
                default:
                    break;
            }
    }

    #endregion

    #region События игровой логики

    protected override void OnUnitDie(Unit unit, DamageType type, Object source)
    {
        if (unit is Hero)
            Player.Defeat();

        if (unit is Monster && source is Hero)
            enemies++;
    }

    #endregion

    #endregion

    #endregion

    #region Методы

    public static void Init(Player instance)
    {
        if (singlton != null)
            Destroy(singlton.gameObject);
        singlton = instance;

        steps = 0;
        enemies = 0;
        coins = 0;
        stepsTotal = 0;
    }

    #region Работа с инвентарем

    public static void UseItem(int index)
    {
        if (index < inventory.Count && inventory.Count != 0)
            UseItem(inventory[index]);
    }

    public static void UseItem(Item item)
    {
        if (item != null)
            item.Use();
    }

    public static void AddItem(Item item)
    {
        if (item != null)
            inventory.Add(item);
    }

    public static void RemoveItem(Item item)
    {
        inventory.Remove(item);
    }

    public static void ClearInventory()
    {
        inventory.Clear();
    }

    public static void LoadInventory()
    {
        //Debug.Log("Инвентарь загружен");
        inventory = new List<Item>();

        for (int i = 0; i < inventorySize; i++)
        {
            var itemID = PlayerPrefs.GetInt($"inventory{i}", -1);
            if (itemID > -1)
            {
                var item = singlton.resourceFile.GetItem(itemID);
                //Debug.Log($" > {item.name} ID: {itemID}");
                AddItem(item);
            }
        }
    }

    public static void SaveInventory()
    {
        //Debug.Log("Инвентарь сохранен:");
        for (int i = 0; i < inventorySize; i++)
        {
            if (i < inventory.Count)
            {
                var item = inventory[i];
                var itemID = singlton.resourceFile.GetItemDataID(item);

                //Debug.Log($" > {item.name} ID: {itemID}");
                PlayerPrefs.SetInt($"inventory{i}", itemID);
            }
            else
            {
                //Debug.Log($" > Empty slot");
                PlayerPrefs.SetInt($"inventory{i}", -1);
            }
        }
    }

    public static ShopFeedback BuyItem(Item item)
    {
        if (coinsTotal < item.price)
            return ShopFeedback.NotEnoughMoney;

        if (inventory.Count >= inventorySize)
            return ShopFeedback.InventoryIsFull;

        AddItem(item);
        coinsTotal -= item.price;
        PlaySound(singlton.resourceFile.sellSound);
        return ShopFeedback.Success;
    }

    #endregion

    #region Взаимодействие с Героем и Уровнем

    public static void OrderAttack()
    {
        if (Hero.singlton.isListening)
            Hero.singlton.Attack();
    }

    public static void OrderJump()
    {
        //Debug.Log("Приказали прыгать");
        if (Hero.singlton.isListening)
            Hero.singlton.Jump();
    }

    public static void OrderBreakJump()
    {
        //Debug.Log("Приказали отменить прыжок");

        if (Hero.singlton.isListening)
            Hero.singlton.BreakJump();
    }

    public static void OrderLeap(LeapDirections direction)
    {
        if (Hero.singlton.isListening)
            Hero.singlton.Leap(direction);
    }

    public static void AddStep()
    {
        steps++;
        if (steps == Level.levelSteps)
        {
            Level.SwitchLevel();
            stepsTotal += steps;
            steps = 0;
        }
    }

    public static void AddCoinsTotal(int amount)
    {
        coinsTotal += amount;
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
        ClearInventory();
        SaveInventory();

        controllEnabled = false;
        CameraController.FocusHero(1f);

        coinsTotal += coins;

        stepsTotal += steps;

        UI.SlideAwayMainUI();

        var newBest = stepsTotal > stepsBest;
        if (newBest)
            stepsBest = stepsTotal;

        StopMusic();
        UI.ShowDefeatUI(newBest);
        PlayMusicOneShot(singlton.resourceFile.defeatSound);
    }

    #endregion

    #region Музыка и звуки

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

    public static void StopMusic()
    {
        singlton.audioSourceMusic.Stop();
    }

    public static void PlayMusic(AudioClip clip)
    {
        singlton.audioSourceMusic.loop = true;
        singlton.audioSourceMusic.clip = clip;
        singlton.audioSourceMusic.Play();
    }

    public static void PlayMusicOneShot(AudioClip clip)
    {
        singlton.audioSourceMusic.loop = false;
        singlton.audioSourceMusic.PlayOneShot(clip);
    }

    public static void PlaySound(AudioClip sound)
    {
        singlton.audioSourceEffects.PlayOneShot(sound);
    }

    #endregion

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
