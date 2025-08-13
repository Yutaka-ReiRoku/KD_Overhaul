// WaveSpawner.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WaveSpawner : MonoBehaviour
{
    private enum SpawnerState { Waiting, Spawning }
    private SpawnerState state;

    private bool isSpawningActive = false;


    [Header("References")]
    [SerializeField] private SelectedLevelSO selectedLevel;
    [SerializeField] private EnemyDatabase enemyDatabase;
    [SerializeField] private List<Transform> spawnPoints;

    // State Tracking
    private int currentWaveIndex = 0;
    private float waveCountdown;
    private List<EnemyBase> activeEnemies = new List<EnemyBase>();
    private float totalHealthOfLastWave;


    private System.Random rng = new System.Random();

    private void OnEnable()
    {
        EnemyBase.OnStaticDeath += OnEnemyDied;
    }

    private void OnDisable()
    {
        EnemyBase.OnStaticDeath -= OnEnemyDied;
    }

    void Update()
    {
        if (!isSpawningActive) return;

        if (state == SpawnerState.Waiting)
        {
            if (waveCountdown > 0)
            {
                waveCountdown -= Time.deltaTime;
            }
            else
            {
                StartCoroutine(SpawnWave());
            }
        }
    }

    public void BeginSpawning()
    {
        LevelData currentLevelData = selectedLevel.selectedLevel;
        if (currentLevelData == null || currentLevelData.allItems.Count == 0)
        {
            Debug.LogError("Không có Level Data hoặc Level Data rỗng!");
            return;
        }

        waveCountdown = currentLevelData.allItems[0].TimeAfterWave;
        state = SpawnerState.Waiting;
        isSpawningActive = true;
        Debug.Log("Wave Spawner has been activated!");
    }

    private IEnumerator SpawnWave()
    {
        state = SpawnerState.Spawning;
        WaveData currentWave = selectedLevel.selectedLevel.allItems[currentWaveIndex];
        Debug.Log($"Spawning Wave {currentWave.WaveID}...");

        totalHealthOfLastWave = 0;

        string[] enemyGroups = currentWave.EnemyGroups.Split(',');
        foreach (string group in enemyGroups)
        {
            string[] pair = group.Split(':');
            string enemyID = pair[0].Trim();
            int quantity = int.Parse(pair[1].Trim());

            EnemyData enemyData = enemyDatabase.GetEnemyDataByID(enemyID);
            if (enemyData == null)
            {
                Debug.LogError($"Không tìm thấy EnemyData với ID: {enemyID}");
                continue;
            }

            totalHealthOfLastWave += enemyData.health * quantity;

            List<int> laneIndices = Enumerable.Range(0, spawnPoints.Count).ToList();

            Shuffle(laneIndices);

            for (int i = 0; i < quantity; i++)
            {
                int spawnIndex = laneIndices[i % spawnPoints.Count];
                Transform currentSpawnPoint = spawnPoints[spawnIndex];

                // Spawn enemy
                GameObject enemyGO = Instantiate(enemyDatabase.GetEnemyPrefabByID(enemyID), currentSpawnPoint.position, Quaternion.identity);
                EnemyBase newEnemy = enemyGO.GetComponent<EnemyBase>();

                
                activeEnemies.Add(newEnemy);

                yield return new WaitForSeconds(currentWave.TimeBetweenSpawns);
            }
        }

        currentWaveIndex++;

        if (currentWaveIndex < selectedLevel.selectedLevel.allItems.Count)
        {
            waveCountdown = selectedLevel.selectedLevel.allItems[currentWaveIndex].TimeAfterWave;
            state = SpawnerState.Waiting;
        }
        else
        {
            Debug.Log("All waves have been spawned! Waiting for remaining enemies to be defeated.");
            state = SpawnerState.Waiting;
        }
    }

    private void OnEnemyDied(EnemyBase diedEnemy)
    {
        
        activeEnemies.Remove(diedEnemy);


        CheckForWinCondition(diedEnemy);

        if (state != SpawnerState.Waiting) return;

        float currentTotalHealth = 0;
        foreach (var enemy in activeEnemies)
        {
            currentTotalHealth += enemy.Health;
        }

        if (currentTotalHealth < totalHealthOfLastWave * 0.5f)
        {
            Debug.Log("Health threshold met! Spawning next wave immediately.");
            waveCountdown = 0;
        }
    }

    private void CheckForWinCondition(EnemyBase recentlyDefeatedEnemy)
    {
        bool isLastWave = currentWaveIndex >= selectedLevel.selectedLevel.allItems.Count;
        bool allEnemiesDefeated = activeEnemies.Count == 0;

        if (isLastWave && allEnemiesDefeated)
        {
            GameManager.Instance.TriggerWin(recentlyDefeatedEnemy.transform.position);
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}