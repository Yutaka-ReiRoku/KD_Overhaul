
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Archer : TowerBase
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

        }
        else
        {
            EnemyBase rangedTarget = FindClosestEnemyInCollider(rangedRangeCollider);
            if (rangedTarget != null)
            {
                currentTarget = rangedTarget.transform;

                if (abilityCooldowns[0] <= 0)
                {
                    isAttacking = true;
                    PerformAbility(towerData.abilities[0], 0);
                    return;
                }
                if (abilityCooldowns[1] <= 0)
                {
                    isAttacking = true;
                    PerformAbility(towerData.abilities[1], 1);
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



    public void AnimationEvent_FireProjectile1()
    {
        if (currentTarget == null) return;

        abilityCooldowns[0] = towerData.abilities[0].cooldownDuration;
        Ability bowAbility = towerData.abilities[0];

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

    public void AnimationEvent_FireProjectile2()
    {
        if (currentTarget == null) return;

        abilityCooldowns[1] = towerData.abilities[1].cooldownDuration;
        Ability bowAbility = towerData.abilities[1];

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
                if (distance < minDistance && transform.position.x < hit.transform.position.x)
                {
                    minDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }
        return closestEnemy;
    }
}