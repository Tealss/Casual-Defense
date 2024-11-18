//using System.Collections.Generic;

//[System.Serializable]
//public class ItemStats
//{
//    public int itemType;  // ������ ���� (1: ���ݷ� ����, 2: ���� ���� ��)
//    public int itemGrade; // ������ ��� (1~7)
//    public int level;     // ������ ����

//    // ������ �ɷ�ġ �⺻�� ���� (1�ܰ� ����)
//    private static readonly Dictionary<int, float> baseStatBonuses = new Dictionary<int, float>
//    {
//        {1, 5},   // ���ݷ�
//        {2, 0.1f}, // ����
//        {3, 1},    // ���� ����
//        {4, 0.05f}, // ũ��Ƽ�� Ȯ��
//        {5, 0.5f}, // ũ��Ƽ�� ������
//        {6, 0.05f}, // �� �̵� �ӵ� ����
//        {7, 0.1f}  // ��� ȹ�� Ȯ�� ����
//    };

//    // ������ �ɷ�ġ�� ����Ͽ� ����
//    public void ApplyItemStats()
//    {
//        if (baseStatBonuses.ContainsKey(itemType))
//        {
//            // 1�ܰ� ���� �������� ��޺� ���ʽ��� 2�辿 ������Ű��
//            float baseBonus = baseStatBonuses[itemType];
//            float bonus = baseBonus * Mathf.Pow(2, itemGrade - 1); // 2�辿 ���� (1�ܰ�� 1��, 2�ܰ�� 2��, 3�ܰ�� 4�� ...)
//            ApplyBonus(bonus);
//        }
//        else
//        {
//            Debug.LogWarning("�� �� ���� ������ Ÿ���Դϴ�.");
//        }
//    }

//    // ���ʽ��� �����ϴ� �Լ� (Ÿ���� �ɷ�ġ�� ������Ʈ)
//    private void ApplyBonus(float bonus)
//    {
//        switch (itemType)
//        {
//            case 1: // ���ݷ� ����
//                towerStats.attackDamage += bonus * level;
//                break;
//            case 2: // ���� �ӵ� ����
//                towerStats.attackSpeed += bonus * level;
//                break;
//            case 3: // ���� ���� ����
//                towerStats.attackRange += bonus * level;
//                break;
//            case 4: // ũ��Ƽ�� Ȯ�� ����
//                towerStats.criticalChance += bonus * level;
//                break;
//            case 5: // ũ��Ƽ�� ������ ����
//                towerStats.criticalDamage += bonus * level;
//                break;
//            case 6: // �� �̵� �ӵ� ����
//                towerStats.enemySlowAmount += bonus * level;
//                break;
//            case 7: // ��� ȹ�� Ȯ�� ����
//                towerStats.goldEarnRate += bonus * level;
//                break;
//            default:
//                break;
//        }
//        Debug.Log($"{GetStatName()} ����: {bonus * level}");
//    }

//    // �ش� ������ Ÿ���� �ɷ�ġ�� �ĺ��ϱ� ���� �Լ�
//    private string GetStatName()
//    {
//        switch (itemType)
//        {
//            case 1: return "���ݷ�";
//            case 2: return "���� �ӵ�";
//            case 3: return "���� ����";
//            case 4: return "ũ��Ƽ�� Ȯ��";
//            case 5: return "ũ��Ƽ�� ������";
//            case 6: return "�� �̵� �ӵ�";
//            case 7: return "��� ȹ�� Ȯ��";
//            default: return "�� �� ����";
//        }
//    }
//}
