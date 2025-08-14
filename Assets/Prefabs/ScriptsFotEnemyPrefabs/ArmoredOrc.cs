using UnityEditor.Playables;
using UnityEngine;

public class ArmoredOrc : EnemyBase
{
    private Ability currentAbility;

    protected override void Update()
    {
        base.Update();

        if (isAttackingBase) return;

        if (currentTarget != null && currentTarget.Health > 0)
        {
            TryToAttack();
        }
        else
        {
            currentTarget = null;
            Move();
        }
    }

    private void TryToAttack()
    {
        for (int i = 0; i < enemyData.abilities.Count; i++)
        {
            if (abilityCooldowns[i] <= 0 && !isAttacking)
            {
                isAttacking = true;
                PerformAbility(enemyData.abilities[i], i);
                return;
            }
        }
        RunAnimation("Idle", 0);
    }

    private void PerformAbility(Ability ability, int abilityIndex)
    {
        attackIndex = abilityIndex;
        currentAbility = ability;
        RunAnimation(ability.animationName, 3);
    }

    private void Move()
    {
        RunAnimation("Walk", 1);
        transform.Translate(Vector2.left * enemyData.moveSpeed * Time.deltaTime, Space.World);
    }

    public void AnimationEvent_DealDamage()
    {
        if (currentTarget != null && currentAbility != null)
        {
            abilityCooldowns[attackIndex] = enemyData.abilities[attackIndex].cooldownDuration;
            currentTarget.TakeDamage(currentAbility.damage);
        }
    }
}