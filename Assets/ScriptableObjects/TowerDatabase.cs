using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerDatabase", menuName = "Tower Defense/Tower Database")]
public class TowerDatabase : ScriptableObject
{
    public List<TowerData> allTowers;

    public TowerData GetTowerDataByID(string towerID)
    {
        return allTowers.FirstOrDefault(tower => tower.name == towerID);
    }
}