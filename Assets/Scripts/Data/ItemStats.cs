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
    public int grade; // ������ ��� (1~7)
    public int level; // ������ ����
    public ItemType itemType; // ������ �ɷ�ġ Ÿ��

    // �ɷ�ġ ����
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

    // ������ Ÿ�Կ� ���� ���ʽ� �� ��ȯ
    private float GetBonusForItemType(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.AttackDamageIncrease:
                return 5f; // ����: ���ݷ� ���� 5%
            case ItemType.AttackSpeedIncrease:
                return 0.1f; // ����: ���� �ӵ� ���� 10%
            case ItemType.AttackRangeIncrease:
                return 2f; // ����: ���� ���� ���� 2
            case ItemType.CriticalChanceIncrease:
                return 2f; // ����: ũ��Ƽ�� Ȯ�� ���� 2%
            case ItemType.CriticalDamageIncrease:
                return 10f; // ����: ũ��Ƽ�� ������ ���� 10%
            case ItemType.EnemySlowAmountIncrease:
                return 0.05f; // ����: �� �̵� �ӵ� ���� 5%
            case ItemType.GoldEarnAmountIncrease:
                return 0.05f; // ����: ��� ȹ�� Ȯ�� ���� 5%
            default:
                return 0f;
        }
    }
}
