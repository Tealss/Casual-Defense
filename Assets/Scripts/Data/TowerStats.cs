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

    // 타워 능력치를 초기화하거나 갱신할 수 있는 메서드
    public void ApplyItemStats(ItemStats itemStats)
    {
        itemStats.ApplyItemStats(this);
    }
}