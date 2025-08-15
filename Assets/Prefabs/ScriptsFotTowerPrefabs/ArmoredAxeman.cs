
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArmoredAxeman : TowerBase
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
        if (meleeTarget != null && !isAttacking)
        {
            currentTarget = meleeTarget.transform;

            if (abilityCooldowns[2] <= 0)
            {
                isAttacking = true;
                PerformAbility(towerData.abilities[2], 2);
                return;
            }
            if (abilityCooldowns[1] <= 0)
            {
                isAttacking = true;
                PerformAbility(towerData.abilities[1], 1);
                return;
            }
            if (abilityCooldowns[0] <= 0)
            {
                isAttacking = true;
                PerformAbility(towerData.abilities[0], 0);
                return;
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
    }

    public void AnimationEvent_DealMeleeDamage1()
    {
        SoundManager.Instance.PlaySound("MeleeAttack");
        int index = 0;
        float damage = towerData.abilities[index].damage;
        abilityCooldowns[index] = towerData.abilities[index].cooldownDuration;

        Collider2D[] hits = Physics2D.OverlapCircleAll((Vector2)meleeRangeCollider.transform.position + meleeRangeCollider.offset, ((CircleCollider2D)meleeRangeCollider).radius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out IDamageable target) && hit.CompareTag("Enemy"))
            {
                target.TakeDamage(damage);
            }
        }
    }
    public void AnimationEvent_DealMeleeDamage2()
    {
        SoundManager.Instance.PlaySound("MeleeAttack");
        int index = 1;
        float damage = towerData.abilities[index].damage;
        abilityCooldowns[index] = towerData.abilities[index].cooldownDuration;

        Collider2D[] hits = Physics2D.OverlapCircleAll((Vector2)meleeRangeCollider.transform.position + meleeRangeCollider.offset, ((CircleCollider2D)meleeRangeCollider).radius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out IDamageable target) && hit.CompareTag("Enemy"))
            {
                target.TakeDamage(damage);
            }
        }
    }
    public void AnimationEvent_DealMeleeDamage3()
    {
        SoundManager.Instance.PlaySound("MeleeAttack");
        int index = 2;
        float damage = towerData.abilities[index].damage;
        abilityCooldowns[index] = towerData.abilities[index].cooldownDuration;

        Collider2D[] hits = Physics2D.OverlapCircleAll((Vector2)meleeRangeCollider.transform.position + meleeRangeCollider.offset, ((CircleCollider2D)meleeRangeCollider).radius);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out IDamageable target) && hit.CompareTag("Enemy"))
            {
                target.TakeDamage(damage);
            }
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