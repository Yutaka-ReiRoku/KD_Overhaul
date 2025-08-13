// LevelData.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Tower Defense/Level Data")]
public class LevelData : ScriptableObject
{
    public List<WaveData> allItems = new List<WaveData>();

    [Header("Level Rewards")]
    [Tooltip("Tower sẽ được mở khóa khi hoàn thành màn chơi này")]
    public TowerData rewardTowerData;
}