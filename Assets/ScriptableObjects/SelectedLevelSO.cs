// SelectedLevelSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "SelectedLevel", menuName = "Tower Defense/Selected Level Data")]
public class SelectedLevelSO : ScriptableObject
{
    public LevelData selectedLevel;
}