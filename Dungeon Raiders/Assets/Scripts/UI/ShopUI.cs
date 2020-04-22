using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShopUI : MonoBehaviour
{
    public GameResources resourceFile;

    public ShopButton buttonPrefab;

    public RectTransform buttonPanelTransform;

    public Text coinsText;

    void Awake()
    {
        Init(resourceFile.itemDatabase);
    }

    void Update()
    {
        coinsText.text = Player.coinsTotal.ToString();
    }

    public void Init(List<Item> items)
    {
        foreach (var item in items)
        {
            if (item.canBeSold)
            {
                var button = Instantiate(buttonPrefab, buttonPanelTransform);
                button.Init(item);
            }
        }
    }
}
