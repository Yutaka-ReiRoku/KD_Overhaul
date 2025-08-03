// Yutaka ReiRoku
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Item
{
    public int itemID;
    public string itemName;
    public string description;
    public int price;
}

[CreateAssetMenu(fileName = "NewItemDatabase", menuName = "Data/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> allItems = new List<Item>(); // "allItems"
}

/*csv horizontal file

for instance:

itemID, itemName, description, price
1, Sword,"A basic sword.",100
2, Shield,"A wooden shield.",75
3, Potion,"Restores 50 HP.",25*/