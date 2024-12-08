using UnityEngine;

[CreateAssetMenu(fileName = "TowerStats", menuName = "Tower/TowerStats")]
public class TowerStats : ScriptableObject
{
    [Header("Base Stats")]
    public float baseLevel;
    public float baseAttackDamage;
    public float baseAttackSpeed;
    public float baseAttackRange;
    public float baseCriticalChance;
    public float baseCriticalDamage;
    public float baseGoldEarnAmount;
    public float baseEnemySlowAmount;

    [Header("Projectile")]
    public float projectileSpeed;

    [HideInInspector] public float level;
    [HideInInspector] public float attackDamage;
    [HideInInspector] public float attackSpeed;
    [HideInInspector] public float attackRange;
    [HideInInspector] public float criticalChance;
    [HideInInspector] public float criticalDamage;
    [HideInInspector] public float goldEarnAmount;
    [HideInInspector] public float enemySlowAmount;

    public void InitializeStats()
    {
        // 기본 능력치 초기화
        level = baseLevel;
        attackDamage = baseAttackDamage;
        attackSpeed = baseAttackSpeed;
        attackRange = baseAttackRange;
        criticalChance = baseCriticalChance;
        criticalDamage = baseCriticalDamage;
        goldEarnAmount = baseGoldEarnAmount;
        enemySlowAmount = baseEnemySlowAmount;

        //ApplyLevelBasedStats();
    }

    //private void ApplyLevelBasedStats()
    //{
    //    if (level > 1)
    //    {        
    //        attackDamage *= Mathf.Pow(2.1f, level - 1);  // 예시: 레벨에 따라 공격력 증가
    //        attackSpeed += (level - 1) * 0.05f;         // 예시: 레벨에 따라 공격속도 증가
    //        enemySlowAmount += (level - 1) * 0.1f;      // 예시: 레벨에 따라 적 느리게 하는 효과 증가
    //    }
    //}
}
