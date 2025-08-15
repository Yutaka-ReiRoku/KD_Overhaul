using UnityEngine;

public class AreaEffect : MonoBehaviour
{
    private float effectValue;

    public void Initialize(float damageAmount)
    {
        this.effectValue = damageAmount;
    }

    public void AnimationEvent_DealEffect()
    {
        float radius = GetComponent<CircleCollider2D>().radius;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy") && hit.TryGetComponent<IDamageable>(out IDamageable enemyTarget))
            {
                enemyTarget.TakeDamage(effectValue);
            }
            else if (hit.CompareTag("Tower") && hit.TryGetComponent<TowerBase>(out TowerBase allyTarget))
            {
                float healAmount = allyTarget.Data.health * (effectValue / 100f);
                allyTarget.Heal(healAmount);
            }
        }
    }

    public void AnimationEvent_DestroySelf()
    {
        Destroy(gameObject);
    }
}