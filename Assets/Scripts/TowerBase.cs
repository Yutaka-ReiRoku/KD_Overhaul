// TowerBase.cs (Final Version)
using UnityEngine;
using System.Collections.Generic;

public abstract class TowerBase : MonoBehaviour, IDamageable
{
    [Header("Base References")]
    [SerializeField] protected TowerData towerData;
    protected Animator animator;

    public TowerData Data => towerData;

    // Stats
    protected float currentHealth;
    protected Transform currentTarget;

    // Ability Cooldowns
    protected List<float> abilityCooldowns;

    // Animation Management
    protected int currentAnimWeight = 0;

    // --- IDamageable Implementation ---
    public float Health => currentHealth;

    protected bool isAttacking = false;
    public virtual void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        SoundManager.Instance.PlaySound("Hurt");
        RunAnimation("Hurt", 5);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // --- Unity Lifecycle ---
    protected virtual void Start()
    {
        currentHealth = towerData.health;
        animator = GetComponent<Animator>();

        abilityCooldowns = new List<float>();
        foreach (var ability in towerData.abilities)
        {
            abilityCooldowns.Add(0f);
        }
    }

    protected virtual void Update()
    {
        for (int i = 0; i < abilityCooldowns.Count; i++)
        {
            if (abilityCooldowns[i] > 0)
            {
                abilityCooldowns[i] -= Time.deltaTime;
            }
        }
    }

    // --- Core Logic ---
    protected virtual void Die()
    {
        RunAnimation("Death", 10);
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 2f);
    }

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
        isAttacking = false;
        currentAnimWeight = 0;
    }
}