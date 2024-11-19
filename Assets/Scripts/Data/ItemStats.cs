using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemStats
{
    public int itemType;  
    public int itemGrade; 
    public int level;    

    private static readonly Dictionary<int, float> baseStatBonuses = new Dictionary<int, float>
    {
        {1, 5},   // ���ݷ�
        {2, 0.1f}, // ����
        {3, 1},    // ���� ����
        {4, 0.05f}, // ũ��Ƽ�� Ȯ��
        {5, 0.5f}, // ũ��Ƽ�� ������
        {6, 0.05f}, // �� �̵� �ӵ� ����
        {7, 0.1f}  // ��� ȹ�� ����
    };

    public void ApplyItemStats(TowerStats towerStats)
    {
        if (baseStatBonuses.ContainsKey(itemType))
        {
            // 1�ܰ� ���� �������� ��޺� ���ʽ��� 2�辿 ������Ű��
            float baseBonus = baseStatBonuses[itemType];
            float bonus = baseBonus * Mathf.Pow(2, itemGrade - 1); 
            ApplyBonus(bonus, towerStats);
        }
        else
        {
            Debug.LogWarning("�� �� ���� ������ Ÿ���Դϴ�.");
        }
    }

    private void ApplyBonus(float bonus, TowerStats towerStats)
    {
        if (towerStats == null)
        {
            Debug.LogError("towerStats�� null�Դϴ�!");
        }
        else
        {
            switch (itemType)
            {
                case 1: // ���ݷ� ����
                    towerStats.attackDamage += bonus * level;
                    break;
                case 2: // ���� �ӵ� ����
                    towerStats.attackSpeed += bonus * level;
                    break;
                case 3: // ���� ���� ����
                    towerStats.attackRange += bonus * level;
                    break;
                case 4: // ũ��Ƽ�� Ȯ�� ����
                    towerStats.criticalChance += bonus * level;
                    break;
                case 5: // ũ��Ƽ�� ������ ����
                    towerStats.criticalDamage += bonus * level;
                    break;
                case 6: // �� �̵� �ӵ� ����
                    towerStats.enemySlowAmount += bonus * level;
                    break;
                case 7: // ��� ȹ�� Ȯ�� ����
                    towerStats.goldEarnAmount += bonus * level;
                    break;
                default:
                    break;
            }
            Debug.Log($"{GetStatName()} ����: {bonus * level}");
        }
    }

    private string GetStatName()
    {
        switch (itemType)
        {
            case 1: return "���ݷ�";
            case 2: return "���� �ӵ�";
            case 3: return "���� ����";
            case 4: return "ũ��Ƽ�� Ȯ��";
            case 5: return "ũ��Ƽ�� ������";
            case 6: return "�� �̵� �ӵ�";
            case 7: return "��� ȹ�� Ȯ��";
            default: return "�� �� ����";
        }
    }
}
