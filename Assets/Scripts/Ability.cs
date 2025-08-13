// Ability.cs
using UnityEngine;

[System.Serializable]
public class Ability
{
    public string abilityName;
    public string animationName;
    public float cooldownDuration;
    public float damage;
    public GameObject projectilePrefab;
}