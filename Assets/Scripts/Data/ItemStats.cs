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
        {1, 5},   // 공격력
        {2, 0.1f}, // 공속
        {3, 1},    // 공격 범위
        {4, 0.05f}, // 크리티컬 확률
        {5, 0.5f}, // 크리티컬 데미지
        {6, 0.05f}, // 적 이동 속도 감소
        {7, 0.1f}  // 골드 획득 증가
    };

    public void ApplyItemStats(TowerStats towerStats)
    {
        if (baseStatBonuses.ContainsKey(itemType))
        {
            // 1단계 값을 기준으로 등급별 보너스를 2배씩 증가시키기
            float baseBonus = baseStatBonuses[itemType];
            float bonus = baseBonus * Mathf.Pow(2, itemGrade - 1); 
            ApplyBonus(bonus, towerStats);
        }
        else
        {
            Debug.LogWarning("알 수 없는 아이템 타입입니다.");
        }
    }

    private void ApplyBonus(float bonus, TowerStats towerStats)
    {
        if (towerStats == null)
        {
            Debug.LogError("towerStats가 null입니다!");
        }
        else
        {
            switch (itemType)
            {
                case 1: // 공격력 증가
                    towerStats.attackDamage += bonus * level;
                    break;
                case 2: // 공격 속도 증가
                    towerStats.attackSpeed += bonus * level;
                    break;
                case 3: // 공격 범위 증가
                    towerStats.attackRange += bonus * level;
                    break;
                case 4: // 크리티컬 확률 증가
                    towerStats.criticalChance += bonus * level;
                    break;
                case 5: // 크리티컬 데미지 증가
                    towerStats.criticalDamage += bonus * level;
                    break;
                case 6: // 적 이동 속도 감소
                    towerStats.enemySlowAmount += bonus * level;
                    break;
                case 7: // 골드 획득 확률 증가
                    towerStats.goldEarnAmount += bonus * level;
                    break;
                default:
                    break;
            }
            Debug.Log($"{GetStatName()} 증가: {bonus * level}");
        }
    }

    private string GetStatName()
    {
        switch (itemType)
        {
            case 1: return "공격력";
            case 2: return "공격 속도";
            case 3: return "공격 범위";
            case 4: return "크리티컬 확률";
            case 5: return "크리티컬 데미지";
            case 6: return "적 이동 속도";
            case 7: return "골드 획득 확률";
            default: return "알 수 없음";
        }
    }
}
