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
    public float enemySlowAmount;
    public float goldEarnAmount;

    //public void ModifyStat(ItemManager.ItemStatType statType, float value)
    //{
    //    switch (statType)
    //    {
    //        case ItemManager.ItemStatType.attackDamageIncrease:
    //            attackDamage += value;
    //            break;
    //        case ItemManager.ItemStatType.attackSpeedIncrease:
    //            attackSpeed += value;
    //            break;
    //        case ItemManager.ItemStatType.attackRangeIncrease:
    //            attackRange += value;
    //            break;
    //        case ItemManager.ItemStatType.criticalChanceIncrease:
    //            criticalChance += value;
    //            break;
    //        case ItemManager.ItemStatType.criticalDamageIncrease:
    //            criticalDamage += value;
    //            break;
    //        case ItemManager.ItemStatType.enemySlowAmountIncrease:
    //            enemySlowAmount += value;
    //            break;
    //        case ItemManager.ItemStatType.goldEarnAmountIncrease:
    //            goldEarnAmount += value;
    //            break;
    //    }
    //}

}