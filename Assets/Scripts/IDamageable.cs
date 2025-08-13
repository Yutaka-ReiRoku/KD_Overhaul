// IDamageable.cs
public interface IDamageable
{
    float Health { get; }

    void TakeDamage(float damageAmount);
}