// Priest.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Priest : TowerBase
{
    [Header("Priest Specifics")]
    [SerializeField] private Collider2D attackRangeCollider;
    [SerializeField] private Collider2D healRangeCollider;

    private Ability currentAbility;
    private Transform healTarget;


    protected override void Update()
    {
        base.Update();
        DecideAndAct();
    }

    private void DecideAndAct()
    {
        if (isAttacking) return;

        if (abilityCooldowns[0] <= 0)
        {
            healTarget = FindWoundedAllyInRange(healRangeCollider)?.transform;
            if (healTarget != null)
            {
                isAttacking = true;
                PerformAbility(towerData.abilities[0]);
                return;
            }
        }

        if (abilityCooldowns[1] <= 0)
        {
            currentTarget = FindClosestEnemyInCollider(attackRangeCollider)?.transform;
            if (currentTarget != null)
            {
                isAttacking = true;
                PerformAbility(towerData.abilities[1]);
                return;
            }
        }

        RunAnimation("Idle", 0);
    }

    private void PerformAbility(Ability ability)
    {
        currentAbility = ability;


        RunAnimation(ability.animationName, 3);
    }

    public void AnimationEvent_CastSpell()
    {
        if (currentAbility == null || currentAbility.projectilePrefab == null) return;

        Transform targetTransform = (currentTarget != null) ? currentTarget : healTarget;
        if (targetTransform == null) return;

        GameObject effectGO = Instantiate(currentAbility.projectilePrefab, targetTransform.position, Quaternion.identity);
        AreaEffect effectScript = effectGO.GetComponent<AreaEffect>();
        if (effectScript != null)
        {
            effectScript.Initialize(currentAbility.damage);
        }

        int abilityIndex = towerData.abilities.IndexOf(currentAbility);
        if (abilityIndex != -1)
        {
            abilityCooldowns[abilityIndex] = currentAbility.cooldownDuration;
        }
    }


    private TowerBase FindWoundedAllyInRange(Collider2D rangeCollider)
    {
        List<Collider2D> hitColliders = new List<Collider2D>();
        rangeCollider.Overlap(new ContactFilter2D().NoFilter(), hitColliders);

        TowerBase mostWoundedAlly = null;
        float lowestHealthPercentage = 1f;

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Tower") && hit.gameObject != this.gameObject && hit.TryGetComponent<TowerBase>(out TowerBase ally))
            {
                float healthPercentage = ally.Health / ally.Data.health;
                if (healthPercentage < 1f && healthPercentage < lowestHealthPercentage)
                {
                    lowestHealthPercentage = healthPercentage;
                    mostWoundedAlly = ally;
                }
            }
        }
        return mostWoundedAlly;
    }

    private EnemyBase FindClosestEnemyInCollider(Collider2D rangeCollider)
    {
        List<Collider2D> hitColliders = new List<Collider2D>();
        rangeCollider.Overlap(new ContactFilter2D().NoFilter(), hitColliders);

        EnemyBase closestEnemy = null;
        float minDistance = float.MaxValue;

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Enemy") && hit.TryGetComponent<EnemyBase>(out EnemyBase enemy))
            {
                if (enemy.Health > 0)
                {
                    float distance = Vector2.Distance(transform.position, hit.transform.position);
                    if (distance < minDistance && transform.position.x < hit.transform.position.x)
                    {
                        minDistance = distance;
                        closestEnemy = enemy;
                    }
                }
            }
        }
        return closestEnemy;
    }
}