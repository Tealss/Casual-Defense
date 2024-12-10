using UnityEngine;

[CreateAssetMenu(fileName = "TowerStats", menuName = "Tower/TowerStats")]
public class TowerStats : ScriptableObject
{
    [Header("Stats")]
    public float level;
    public float attackDamage;
    public float attackSpeed;
    public float attackRange;
    public float criticalChance;
    public float criticalDamage;
    public float goldEarnAmount;
    public float enemySlowAmount;

    public float projectileSpeed;

    [Header("Base Stats")]
    [HideInInspector] public float baseLevel;
    [HideInInspector] public float baseAttackDamage;
    [HideInInspector] public float baseAttackSpeed;
    [HideInInspector] public float baseAttackRange;
    [HideInInspector] public float baseCriticalChance;
    [HideInInspector] public float baseCriticalDamage;
    [HideInInspector] public float baseGoldEarnAmount;
    [HideInInspector] public float baseEnemySlowAmount;

    [Header("Item Stats")]
    [HideInInspector] public float itemAttackDamage;
    [HideInInspector] public float itemAttackSpeed;
    [HideInInspector] public float itemAttackRange;
    [HideInInspector] public float itemCriticalChance;
    [HideInInspector] public float itemCriticalDamage;
    [HideInInspector] public float itemGoldEarnAmount;
    [HideInInspector] public float itemEnemySlowAmount;

    public void InitializeBaseStats()
    {
        level = baseLevel;
        attackDamage = baseAttackDamage;
        attackSpeed = baseAttackSpeed;
        attackRange = baseAttackRange;
        criticalChance = baseCriticalChance;
        criticalDamage = baseCriticalDamage;
        goldEarnAmount = baseGoldEarnAmount;
        enemySlowAmount = baseEnemySlowAmount;
    }

    public void ApplyItemBonuses()
    {
        attackDamage += itemAttackDamage;
        attackSpeed += itemAttackSpeed;
        attackRange += itemAttackRange;
        criticalChance += itemCriticalChance;
        criticalDamage += itemCriticalDamage;
        goldEarnAmount += itemGoldEarnAmount;
        enemySlowAmount += itemEnemySlowAmount;
    }

    public void InitializeStats()
    {
        InitializeBaseStats();
        ApplyItemBonuses();
    }

    public void AddItemBonus(int itemType, float effect)
    {
        switch (itemType)
        {
            case 0: itemAttackDamage += effect; break;
            case 1: itemAttackSpeed += effect; break;
            case 2: itemAttackRange += effect; break;
            case 3: itemCriticalChance += effect; break;
            case 4: itemCriticalDamage += effect; break;
            case 5: itemGoldEarnAmount += effect; break;
            case 6: itemEnemySlowAmount += effect; break;
        }
    }

    public void RemoveItemBonus(int itemType, float effect)
    {
        switch (itemType)
        {
            case 0: itemAttackDamage -= effect; break;
            case 1: itemAttackSpeed -= effect; break;
            case 2: itemAttackRange -= effect; break;
            case 3: itemCriticalChance -= effect; break;
            case 4: itemCriticalDamage -= effect; break;
            case 5: itemGoldEarnAmount -= effect; break;
            case 6: itemEnemySlowAmount -= effect; break;
        }
    }
}
