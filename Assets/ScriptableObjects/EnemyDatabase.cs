// EnemyDatabase.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Tower Defense/Enemy Database")]
public class EnemyDatabase : ScriptableObject
{
    [Tooltip("Kéo tất cả các asset EnemyData của bạn vào đây")]
    public List<EnemyData> allEnemies = new List<EnemyData>();

    /// <summary>
    /// </summary>
    public EnemyData GetEnemyDataByID(string enemyID)
    {
        return allEnemies.FirstOrDefault(enemy => enemy.name == enemyID);
    }

    /// <summary>
    /// </summary>
    public GameObject GetEnemyPrefabByID(string enemyID)
    {
        EnemyData data = GetEnemyDataByID(enemyID);
        if (data != null && data.enemyPrefab != null)
        {
            return data.enemyPrefab;
        }

        Debug.LogError($"Không tìm thấy Prefab cho Enemy có ID: {enemyID}");
        return null;
    }
}