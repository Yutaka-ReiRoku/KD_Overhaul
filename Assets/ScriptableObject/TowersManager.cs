using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowersManager", menuName = "Scriptable Objects/TowersManager")]
public class TowersManager : ScriptableObject
{
    [System.Serializable]
    public class Tower
    {
        public int towerID;
        public Sprite itemIcon;
        public int towerCost;
    }

    public List<Tower> towers = new List<Tower>();
    public ItemData GetItemByID(int id)
    {
        return allItems.Find(item => item.itemID == id);
    }
}
