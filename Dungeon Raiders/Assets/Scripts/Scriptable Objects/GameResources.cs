using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Game Resources", menuName = "Resources File")]
public class GameResources : ScriptableObject
{
    public AudioClip sellSound;
    public AudioClip defeatSound;
    public List<AudioClip> music;

    public List<Item> itemDatabase;
    public int GetItemDataID(Item item)
    {
        return itemDatabase.IndexOf(item);
    }
    public Item GetItem(int dataIndex)
    {
        if (dataIndex > -1 && dataIndex < itemDatabase.Count)
            return itemDatabase[dataIndex];
        else
            return null;
    }

    public List<Unit> unitTypes;
    public Unit GetUnitType(string key)
    {
        return unitTypes.FirstOrDefault(u => u.unitType == key);
    }
}

