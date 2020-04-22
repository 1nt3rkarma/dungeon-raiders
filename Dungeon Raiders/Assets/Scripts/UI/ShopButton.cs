using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public Text price;
    public Image icon;

    public Item item;

    public void Init(Item item)
    {
        name = "Item: " + item.name;

        icon.sprite = item.icon;
        price.text = item.price.ToString();

        this.item = item;
    }

    public void OnClick()
    {
        var feedback = Player.BuyItem(item);

        switch (feedback)
        {
            case ShopFeedback.Success:
                Debug.Log("Купили предмет");
                break;
            case ShopFeedback.NotEnoughMoney:
                Debug.Log("Не хватает денег");
                break;
            case ShopFeedback.InventoryIsFull:
                Debug.Log("Инвентарь заполнен");
                break;
        }
    }
}

public enum ShopFeedback { Success, NotEnoughMoney, InventoryIsFull }
