// TowerData.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewTowerData", menuName = "Tower Defense/Tower Data")]
public class TowerData : ScriptableObject
{
    public string towerName;
    public Sprite towerIcon;
    public int cost;
    public string description;
    public float cooldownTime;

    [Header("Gameplay Prefab & Stats")]
    public GameObject towerPrefab;
    public float health = 100f;
    public List<Ability> abilities;
}