
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Tower Defense/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("General")]
    public string enemyName;
    public GameObject enemyPrefab;

    [Header("Base Stats")]
    public float health = 100f;
    public float moveSpeed = 1f;

    [Header("Abilities")]
    [Tooltip("Danh sách tất cả các kỹ năng/đòn tấn công của enemy này.")]
    public List<Ability> abilities;
}