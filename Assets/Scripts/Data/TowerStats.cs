using UnityEngine;

[CreateAssetMenu(fileName = "New Tower Stats", menuName = "Tower Defense/Tower Stats", order = 1)]
public class TowerStats : ScriptableObject
{
    public string towerName;
    public float attackDamage;
    public float attackRange;
    public float attackSpeed;
    public float projectileSpeed;
    public float criticalChance;
    public float criticalDamage;
    public float defense;
}