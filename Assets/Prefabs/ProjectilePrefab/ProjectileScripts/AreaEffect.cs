using UnityEngine;

public class AreaEffect : MonoBehaviour
{
    private float damage;

    public void Initialize(float damageAmount)
    {
        this.damage = damageAmount;
    }

    public void AnimationEvent_DealDamage()
    {
        float radius = GetComponent<CircleCollider2D>().radius;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy") && hit.TryGetComponent<IDamageable>(out IDamageable target))
            {
                target.TakeDamage(damage);
            }
        }
    }

    public void AnimationEvent_DestroySelf()
    {
        Destroy(gameObject);
    }
}