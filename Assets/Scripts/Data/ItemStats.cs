using UnityEngine;

[CreateAssetMenu(fileName = "ItemStats", menuName = "Item/ItemStats")]
public class ItemStats : ScriptableObject
{
    [Header("Stats")]
    public float level;
    public float itemAttackDamage;
    public float itemAttackSpeed;
    public float itemAttackRange;
    public float itemCriticalChance;
    public float itemCriticalDamage;
    public float itemGoldEarnAmount;
    public float itemEnemySlowAmount;

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
        baseAttackDamage = itemAttackDamage;
        baseAttackSpeed = itemAttackSpeed;
        baseAttackRange = itemAttackRange;
        baseCriticalChance = itemCriticalChance;
        baseCriticalDamage = itemCriticalDamage;
        baseGoldEarnAmount = itemGoldEarnAmount;
        baseEnemySlowAmount = itemEnemySlowAmount;

    }

    public void itemReset()
    {

        itemAttackDamage = 0;
        itemAttackSpeed = 0;
        itemAttackRange = 0;
        itemCriticalChance = 0;
        itemCriticalDamage = 0;
        itemGoldEarnAmount = 0;
        itemEnemySlowAmount = 0;

    }

}
