using UnityEngine;

[CreateAssetMenu(fileName = "TowerStats", menuName = "Tower/TowerStats")]
public class TowerStats : ScriptableObject
{
    [Header("Base Stats")]
    public float baseAttackDamage;
    public float baseAttackSpeed;
    public float baseAttackRange;
    public float baseCriticalChance;
    public float baseCriticalDamage;
    public float baseGoldEarnAmount;
    public float baseEnemySlowAmount;

    [Header("Projectile")]
    public float projectileSpeed;

    [HideInInspector] public float attackDamage;
    [HideInInspector] public float attackSpeed;
    [HideInInspector] public float attackRange;
    [HideInInspector] public float criticalChance;
    [HideInInspector] public float criticalDamage;
    [HideInInspector] public float goldEarnAmount;
    [HideInInspector] public float enemySlowAmount;

    public void ResetStats()
    {
        attackDamage = baseAttackDamage;
        attackSpeed = baseAttackSpeed;
        attackRange = baseAttackRange;
        criticalChance = baseCriticalChance;
        criticalDamage = baseCriticalDamage;
        goldEarnAmount = baseGoldEarnAmount;
        enemySlowAmount = baseEnemySlowAmount;
    }
}
