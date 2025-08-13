// LevelDatabase.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "Tower Defense/Level Database")]
public class LevelDatabase : ScriptableObject
{
    public List<LevelData> allLevels = new List<LevelData>();
}