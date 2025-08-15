// Wizard.cs (Final Version - Multi-Ability)
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Wizard : TowerBase
{
    [SerializeField] private Collider2D attackRangeCollider;
    [SerializeField] private Transform firePoint;

    private Ability currentAbility;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        DecideAndAct();
    }

    private void DecideAndAct()
    {
        if (currentTarget == null || !IsTargetStillValid(currentTarget, attackRangeCollider))
        {
            currentTarget = FindClosestEnemyInCollider(attackRangeCollider)?.transform;
        }

        if (currentTarget != null)
        {
            if (isAttacking) return;
            for (int i = 0; i < towerData.abilities.Count; i++)
            {
                if (abilityCooldowns[i] <= 0)
                {
                    isAttacking = true;
                    PerformAbility(towerData.abilities[i], i);
                    return;
                }
            }
        }

        if (currentAnimWeight == 0)
        {
            RunAnimation("Idle", 0);
        }
    }

    private void PerformAbility(Ability ability, int abilityIndex)
    {
        currentAbility = ability;
        RunAnimation(ability.animationName, 3);
    }

    public void AnimationEvent_CastSpell()
    {
        if (currentTarget == null || currentAbility == null || currentAbility.projectilePrefab == null) return;

        if (currentAbility.projectilePrefab.GetComponent<AreaEffect>() != null)
        {
            GameObject effectGO = Instantiate(currentAbility.projectilePrefab, currentTarget.position, Quaternion.identity);

            AreaEffect effectScript = effectGO.GetComponent<AreaEffect>();
            if (effectScript != null)
            {
                effectScript.Initialize(currentAbility.damage);
            }
        }
        else if (currentAbility.projectilePrefab.GetComponent<Projectile>() != null)
        {
            GameObject projectileGO = Instantiate(currentAbility.projectilePrefab, firePoint.position, Quaternion.identity);

            Projectile projectile = projectileGO.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Seek(currentTarget, currentAbility.damage);
            }
        }

        int abilityIndex = towerData.abilities.IndexOf(currentAbility);
        if (abilityIndex != -1)
        {
            abilityCooldowns[abilityIndex] = currentAbility.cooldownDuration;
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
                if (enemy.Health > 0)
                {
                    float distance = Vector2.Distance(transform.position, hit.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestEnemy = enemy;
                    }
                }
            }
        }
        return closestEnemy;
    }

    private bool IsTargetStillValid(Transform target, Collider2D rangeCollider)
    {
        if (target == null) return false;
        if (target.GetComponent<EnemyBase>().Health <= 0) return false;
        return rangeCollider.OverlapPoint(target.position);
    }
}