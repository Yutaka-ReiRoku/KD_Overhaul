// WaveData.cs
using UnityEngine;

[System.Serializable]
public class WaveData
{
    public int WaveID;
    public float TimeAfterWave;

    [Tooltip("Thời gian chờ (giây) giữa mỗi lần spawn enemy trong wave này")]
    public float TimeBetweenSpawns;

    public string EnemyGroups;
}