// EnemyBase.cs (Final Version)
using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    public static event Action<EnemyBase> OnStaticDeath;

    [Header("Base References")]
    [SerializeField] protected EnemyData enemyData;
    protected Animator animator;

    // Stats
    protected float currentHealth;
    protected IDamageable currentTarget;

    // Ability Cooldowns
    protected List<float> abilityCooldowns;

    // State
    private bool isAttackingBase = false;

    // Animation Management
    protected int currentAnimWeight = 0;

    // --- IDamageable Implementation ---
    public float Health => currentHealth;

    public virtual void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        RunAnimation("Hurt", 5);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // --- Unity Lifecycle ---
    protected virtual void Start()
    {
        currentHealth = enemyData.health;
        animator = GetComponent<Animator>();

        abilityCooldowns = new List<float>();
        foreach (var ability in enemyData.abilities)
        {
            abilityCooldowns.Add(0f);
        }
    }

    protected virtual void Update()
    {
        if (isAttackingBase) return;

        for (int i = 0; i < abilityCooldowns.Count; i++)
        {
            if (abilityCooldowns[i] > 0)
            {
                abilityCooldowns[i] -= Time.deltaTime;
            }
        }
        Debug.Log(currentAnimWeight);
    }

    // --- Core Logic ---
    public void StartAttackingBase()
    {
        isAttackingBase = true;
        RunAnimation("Attack", 6);
    }

    protected virtual void Die()
    {
        OnStaticDeath?.Invoke(this);
        RunAnimation("Death", 10);
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        Destroy(gameObject, 2f);
    }

    // --- Animation Management ---
    protected void RunAnimation(string animation, int weight)
    {
        if (animator == null) return;
        if (weight >= currentAnimWeight)
        {
            currentAnimWeight = weight;
            animator.Play(animation);
        }
    }

    public void AnimationEvent_ResetAnimWeight()
    {
        currentAnimWeight = 0;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out IDamageable target))
        {
            if (other.GetComponent<TowerBase>() != null)
            {
                currentTarget = target;
            }
        }
    }
}