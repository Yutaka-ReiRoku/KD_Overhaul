
using UnityEngine;

public class AreaEffect : MonoBehaviour
{
    private float effectValue;

    public void Initialize(float value)
    {
        this.effectValue = value;
    }

    public void AnimationEvent_DealEffect()
    {
        float radius = GetComponent<CircleCollider2D>().radius;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hit in hits)
        {
            if (effectValue > 0)
            {
                if (hit.CompareTag("Enemy") && hit.TryGetComponent<IDamageable>(out IDamageable enemyTarget))
                {
                    enemyTarget.TakeDamage(effectValue);
                }
            }
            else if (effectValue < 0)
            {
                if (hit.CompareTag("Tower") && hit.TryGetComponent<TowerBase>(out TowerBase allyTarget))
                {
                    float healPercentage = Mathf.Abs(effectValue);
                    float healAmount = allyTarget.Data.health * (healPercentage / 100f);
                    allyTarget.Heal(healAmount);
                }
            }
        }
    }

    public void AnimationEvent_DestroySelf()
    {
        Destroy(gameObject);
    }
}