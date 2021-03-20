
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviourExtended
{
    public static Player singleton { get; private set; }

    public static bool restrictControlls;

    public static List<Item> inventory;
    public const int inventorySize = 3;

    public static bool controllEnabled = false;

    public static PlayerData Data;

    public static int steps = 0;
    public static int enemies = 0;
    public static int coins = 0;

    public static int stepsTotal = 0;

    public AudioMixer audioMixer;
    public GameResources resourceFile;
    public static GameResources ResourceFile { get => singleton.resourceFile; }
    public AudioSource audioSourceMusic;
    public AudioSource audioSourceEffects;

    [Header("Debugging")]
    public PlayerData dataView;

    #region Реакции на события

    #region События движка Unity

    void Awake()
    {
        Init(this);

        SubsribeToGameEvents();
    }

    public static void Init(Player instance)
    {
        if (singleton != null)
            Destroy(singleton.gameObject);
        singleton = instance;

        Data = PlayerData.Load();

        steps = 0;
        enemies = 0;
        coins = 0;
        stepsTotal = 0;
    }

    void Start()
    {
        LoadInventory();

        EnableMusic(PlayerSettings.MusicOn);

        EnableSounds(PlayerSettings.SoundsOn);

        var musicCollection = singleton.resourceFile.music;
        var rand = Random.Range(0, musicCollection.Count);
        PlayMusic(musicCollection[rand]);
    }

    void OnDestroy()
    {
        PlayerData.Save(Data);

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

        if (Data != null && dataView != Data)
            dataView = Data;
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
            Defeat();

        if (unit is Monster && source is Hero)
            enemies++;
    }

    #endregion

    #endregion

    #endregion

    #region Методы

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
        {
            inventory.Add(item);
            SaveInventory();
        }
    }

    public static void RemoveItem(Item item)
    {
        inventory.Remove(item);
        SaveInventory();
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
            var itemID = Data.inventoryIDs[i];
            if (itemID > -1)
            {
                var item = singleton.resourceFile.GetItem(itemID);
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
                var itemID = ResourceFile.GetItemDataID(item);

                Data.inventoryIDs[i] = itemID;
            }
            else
            {
                Data.inventoryIDs[i] = -1;
            }
        }
    }

    public static ShopFeedback BuyItem(Item item)
    {
        if (Data.coinsTotal < item.price)
            return ShopFeedback.NotEnoughMoney;

        if (inventory.Count >= inventorySize)
            return ShopFeedback.InventoryIsFull;

        AddItem(item);
        Data.coinsTotal -= item.price;
        PlaySound(singleton.resourceFile.sellSound);
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
        Data.coinsTotal += amount;
    }

    public static void AddCoins(int amount)
    {
        coins += amount;
    }

    public static void StopMoving(float overTime = 0)
    {
        Level.StopFlow(overTime);
    }

    public static void ContinueMoving(float overTime = 0)
    {
        Level.RunFlow(overTime);
    }

    public void Defeat()
    {
        StartCoroutine(DefeatRoutine());
    }
    private IEnumerator DefeatRoutine()
    {
        ClearInventory();
        SaveInventory();

        controllEnabled = false;
        CameraController.FocusHero(1f);

        Data.coinsTotal += coins;

        stepsTotal += steps;

        UI.SlideAwayMainUI();

        var newBest = stepsTotal > Data.stepsBest;
        if (newBest)
            Data.stepsBest = stepsTotal;

        StopMusic();

        yield return new WaitForSecondsRealtime(1);

        UI.ShowDefeatUI(newBest);
        PlayMusicOneShot(resourceFile.defeatSound);
    }


    #endregion

    #region Музыка и звуки

    public static void EnableMusic(bool mode)
    {
        if (mode)
            singleton.audioMixer.SetFloat("MusicVolume", 0);
        else
            singleton.audioMixer.SetFloat("MusicVolume", -80);
    }

    public static void EnableSounds(bool mode)
    {
        if (mode)
            singleton.audioMixer.SetFloat("SoundsVolume", 0);
        else
            singleton.audioMixer.SetFloat("SoundsVolume", -80);
    }

    public static void StopMusic()
    {
        singleton.audioSourceMusic.Stop();
    }

    public static void PlayMusic(AudioClip clip)
    {
        singleton.audioSourceMusic.loop = true;
        singleton.audioSourceMusic.clip = clip;
        singleton.audioSourceMusic.Play();
    }

    public static void PlayMusicOneShot(AudioClip clip)
    {
        singleton.audioSourceMusic.loop = false;
        singleton.audioSourceMusic.PlayOneShot(clip);
    }

    public static void PlaySound(AudioClip sound)
    {
        singleton.audioSourceEffects.PlayOneShot(sound);
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

    public static void ExitDungeon()
    {
        SceneManager.LoadScene("MainMenu");
    }

    #endregion

}