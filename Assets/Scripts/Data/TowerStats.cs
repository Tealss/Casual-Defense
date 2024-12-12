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

        //Debug.Log($"{itemAttackDamageBonus}");

    }

    //public void ResetItemBonuses()
    //{
    //    itemAttackDamageBonus = 0;
    //    itemAttackSpeedBonus = 0;
    //    itemAttackRangeBonus = 0;
    //    itemCriticalChanceBonus = 0;
    //    itemCriticalDamageBonus = 0;
    //    itemGoldEarnAmountBonus = 0;
    //    itemEnemySlowAmountBonus = 0;
    //}

}
