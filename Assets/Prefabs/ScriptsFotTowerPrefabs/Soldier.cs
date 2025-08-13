// Soldier.cs (Final Version)
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Soldier : TowerBase
{
    [Header("Soldier Specifics")]
    [Tooltip("Collider cho phạm vi tấn công cận chiến")]
    [SerializeField] private Collider2D meleeRangeCollider;
    [Tooltip("Collider cho phạm vi tấn công tầm xa")]
    [SerializeField] private Collider2D rangedRangeCollider;
    [Tooltip("Điểm bắn ra mũi tên")]
    [SerializeField] private Transform firePoint;


    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        DecideAndAct();
    }

    /// <summary>
    /// </summary>
    private void DecideAndAct()
    {
        EnemyBase meleeTarget = FindClosestEnemyInCollider(meleeRangeCollider);
        if (meleeTarget != null)
        {
            currentTarget = meleeTarget.transform;

            FaceTarget();

            if (abilityCooldowns[1] <= 0)
            {
                PerformAbility(towerData.abilities[1], 1);
                return;
            }
            if (abilityCooldowns[0] <= 0)
            {
                PerformAbility(towerData.abilities[0], 0);
                return;
            }
        }
        else
        {
            EnemyBase rangedTarget = FindClosestEnemyInCollider(rangedRangeCollider);
            if (rangedTarget != null)
            {
                currentTarget = rangedTarget.transform;
                FaceTarget();

                if (abilityCooldowns[2] <= 0)
                {
                    PerformAbility(towerData.abilities[2], 2);
                    return;
                }
            }
        }

        if (currentAnimWeight == 0)
        {
            RunAnimation("Idle", 0);
        }
    }

    /// <summary>
    /// </summary>
    private void PerformAbility(Ability ability, int abilityIndex)
    {
        RunAnimation(ability.animationName, 3);

        abilityCooldowns[abilityIndex] = ability.cooldownDuration;
    }

    private void FaceTarget()
    {
        if (currentTarget == null) return;

        if (currentTarget.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void AnimationEvent_DealMeleeDamage()
    {
        float damage = towerData.abilities[0].damage;

        Collider2D[] hits = Physics2D.OverlapCircleAll(meleeRangeCollider.transform.position, ((CircleCollider2D)meleeRangeCollider).radius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out IDamageable target) && hit.CompareTag("Enemy"))
            {
                target.TakeDamage(damage);
            }
        }
    }

    public void AnimationEvent_FireProjectile()
    {
        if (currentTarget == null) return;

        Ability bowAbility = towerData.abilities[2];

        Vector2 direction = (currentTarget.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        GameObject projectileGO = Instantiate(bowAbility.projectilePrefab, firePoint.position, rotation);
        Projectile projectile = projectileGO.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Seek(currentTarget, bowAbility.damage);
        }
    }

    private EnemyBase FindClosestEnemyInCollider(Collider2D rangeCollider)
    {
        List<Collider2D> hitColliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        rangeCollider.Overlap(filter, hitColliders);

        EnemyBase closestEnemy = null;
        float minDistance = float.MaxValue;

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Enemy") && hit.TryGetComponent<EnemyBase>(out EnemyBase enemy))
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }
        return closestEnemy;
    }
}