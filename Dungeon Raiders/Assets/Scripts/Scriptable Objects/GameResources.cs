using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Game Resources",menuName ="Resources File")]
public class GameResources : ScriptableObject
{
    public List<Item> itemDatabase;

    public int GetItemDataID(Item item)
    {
        return itemDatabase.IndexOf(item);
    }

    public Item GetItem(int dataIndex)
    {
        if (dataIndex > 0 && dataIndex < itemDatabase.Count)
            return itemDatabase[dataIndex];
        else
            return null;
    }
}
