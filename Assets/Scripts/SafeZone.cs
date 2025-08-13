// SafeZone.cs
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<EnemyBase>(out EnemyBase enemy))
        {
            GameManager.Instance.TriggerLoss(enemy);
        }
    }
}