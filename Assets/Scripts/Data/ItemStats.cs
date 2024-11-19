public enum ItemType
{
    AttackDamageIncrease = 1,
    AttackSpeedIncrease = 2,
    AttackRangeIncrease = 3,
    CriticalChanceIncrease = 4,
    CriticalDamageIncrease = 5,
    EnemySlowAmountIncrease = 6,
    GoldEarnAmountIncrease = 7
}

public class Item
{
    public int grade; // 아이템 등급 (1~7)
    public int level; // 아이템 레벨
    public ItemType itemType; // 아이템 능력치 타입

    // 능력치 적용
    public void ApplyEffect(TowerStats towerStats)
    {
        float bonus = GetBonusForItemType(itemType);
        switch (itemType)
        {
            case ItemType.AttackDamageIncrease:
                towerStats.attackDamage += bonus * level;
                break;
            case ItemType.AttackSpeedIncrease:
                towerStats.attackSpeed += bonus * level;
                break;
            case ItemType.AttackRangeIncrease:
                towerStats.attackRange += bonus * level;
                break;
            case ItemType.CriticalChanceIncrease:
                towerStats.criticalChance += bonus * level;
                break;
            case ItemType.CriticalDamageIncrease:
                towerStats.criticalDamage += bonus * level;
                break;
            case ItemType.EnemySlowAmountIncrease:
                towerStats.enemySlowAmount += bonus * level;
                break;
            case ItemType.GoldEarnAmountIncrease:
                towerStats.goldEarnAmount += bonus * level;
                break;
            default:
                break;
        }
    }

    // 아이템 타입에 따른 보너스 값 반환
    private float GetBonusForItemType(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.AttackDamageIncrease:
                return 5f; // 예시: 공격력 증가 5%
            case ItemType.AttackSpeedIncrease:
                return 0.1f; // 예시: 공격 속도 증가 10%
            case ItemType.AttackRangeIncrease:
                return 2f; // 예시: 공격 범위 증가 2
            case ItemType.CriticalChanceIncrease:
                return 2f; // 예시: 크리티컬 확률 증가 2%
            case ItemType.CriticalDamageIncrease:
                return 10f; // 예시: 크리티컬 데미지 증가 10%
            case ItemType.EnemySlowAmountIncrease:
                return 0.05f; // 예시: 적 이동 속도 감소 5%
            case ItemType.GoldEarnAmountIncrease:
                return 0.05f; // 예시: 골드 획득 확률 증가 5%
            default:
                return 0f;
        }
    }
}
