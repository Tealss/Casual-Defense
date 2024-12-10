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
    [HideInInspector] public float itemAttackDamageBonus;
    [HideInInspector] public float itemAttackSpeedBonus;
    [HideInInspector] public float itemAttackRangeBonus;
    [HideInInspector] public float itemCriticalChanceBonus;
    [HideInInspector] public float itemCriticalDamageBonus;
    [HideInInspector] public float itemGoldEarnAmountBonus;
    [HideInInspector] public float itemEnemySlowAmountBonus;

    public void InitializeBaseStats()
    {
        baseLevel = level;
        baseAttackDamage = attackDamage;
        baseAttackSpeed = attackSpeed;
        baseAttackRange = attackRange;
        baseCriticalChance = criticalChance;
        baseCriticalDamage = criticalDamage;
        baseGoldEarnAmount = goldEarnAmount;
        baseEnemySlowAmount = enemySlowAmount;

    }

    public void ResetItemBonuses()
    {
        itemAttackDamageBonus = 0;
        itemAttackSpeedBonus = 0;
        itemAttackRangeBonus = 0;
        itemCriticalChanceBonus = 0;
        itemCriticalDamageBonus = 0;
        itemGoldEarnAmountBonus = 0;
        itemEnemySlowAmountBonus = 0;
    }

    //public void ApplyItemBonuses()
    //{
    //    attackDamage = baseAttackDamage + itemAttackDamageBonus;
    //    attackSpeed = baseAttackSpeed + itemAttackSpeedBonus;
    //    attackRange = baseAttackRange + itemAttackRangeBonus;
    //    criticalChance = baseCriticalChance + itemCriticalChanceBonus;
    //    criticalDamage = baseCriticalDamage + itemCriticalDamageBonus;
    //    goldEarnAmount = baseGoldEarnAmount + itemGoldEarnAmountBonus;
    //    enemySlowAmount = baseEnemySlowAmount + itemEnemySlowAmountBonus;

    //    //Debug.Log($"Attack Damage After Items: {attackDamage},{baseAttackDamage},{itemAttackDamageBonus}");
    //}

    public void InitializeStats()
    {
        InitializeBaseStats();
        //ApplyItemBonuses();
    }

    public void AddItemBonus(int itemType, float effect)
    {
        switch (itemType)
        {
            case 0: itemAttackDamageBonus += effect; break;
            case 1: itemAttackSpeedBonus += effect; break;
            case 2: itemAttackRangeBonus += effect; break;
            case 3: itemCriticalChanceBonus += effect; break;
            case 4: itemCriticalDamageBonus += effect; break;
            case 5: itemGoldEarnAmountBonus += effect; break;
            case 6: itemEnemySlowAmountBonus += effect; break;
        }
        Debug.Log($"At: {attackDamage},{baseAttackDamage},{itemAttackDamageBonus},{effect}");
    }

    public void RemoveItemBonus(int itemType, float effect)
    {
        switch (itemType)
        {
            case 0: itemAttackDamageBonus -= effect; break;
            case 1: itemAttackSpeedBonus -= effect; break;
            case 2: itemAttackRangeBonus -= effect; break;
            case 3: itemCriticalChanceBonus -= effect; break;
            case 4: itemCriticalDamageBonus -= effect; break;
            case 5: itemGoldEarnAmountBonus -= effect; break;
            case 6: itemEnemySlowAmountBonus -= effect; break;
        }
        Debug.Log($"At: {attackDamage},{baseAttackDamage},{itemAttackDamageBonus},{effect}");
    }
}
