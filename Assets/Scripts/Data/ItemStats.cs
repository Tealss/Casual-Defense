//using System.Collections.Generic;

//[System.Serializable]
//public class ItemStats
//{
//    public int itemType;  // 아이템 종류 (1: 공격력 증가, 2: 공속 증가 등)
//    public int itemGrade; // 아이템 등급 (1~7)
//    public int level;     // 아이템 레벨

//    // 아이템 능력치 기본값 설정 (1단계 기준)
//    private static readonly Dictionary<int, float> baseStatBonuses = new Dictionary<int, float>
//    {
//        {1, 5},   // 공격력
//        {2, 0.1f}, // 공속
//        {3, 1},    // 공격 범위
//        {4, 0.05f}, // 크리티컬 확률
//        {5, 0.5f}, // 크리티컬 데미지
//        {6, 0.05f}, // 적 이동 속도 감소
//        {7, 0.1f}  // 골드 획득 확률 증가
//    };

//    // 아이템 능력치를 계산하여 적용
//    public void ApplyItemStats()
//    {
//        if (baseStatBonuses.ContainsKey(itemType))
//        {
//            // 1단계 값을 기준으로 등급별 보너스를 2배씩 증가시키기
//            float baseBonus = baseStatBonuses[itemType];
//            float bonus = baseBonus * Mathf.Pow(2, itemGrade - 1); // 2배씩 증가 (1단계는 1배, 2단계는 2배, 3단계는 4배 ...)
//            ApplyBonus(bonus);
//        }
//        else
//        {
//            Debug.LogWarning("알 수 없는 아이템 타입입니다.");
//        }
//    }

//    // 보너스를 적용하는 함수 (타워의 능력치를 업데이트)
//    private void ApplyBonus(float bonus)
//    {
//        switch (itemType)
//        {
//            case 1: // 공격력 증가
//                towerStats.attackDamage += bonus * level;
//                break;
//            case 2: // 공격 속도 증가
//                towerStats.attackSpeed += bonus * level;
//                break;
//            case 3: // 공격 범위 증가
//                towerStats.attackRange += bonus * level;
//                break;
//            case 4: // 크리티컬 확률 증가
//                towerStats.criticalChance += bonus * level;
//                break;
//            case 5: // 크리티컬 데미지 증가
//                towerStats.criticalDamage += bonus * level;
//                break;
//            case 6: // 적 이동 속도 감소
//                towerStats.enemySlowAmount += bonus * level;
//                break;
//            case 7: // 골드 획득 확률 증가
//                towerStats.goldEarnRate += bonus * level;
//                break;
//            default:
//                break;
//        }
//        Debug.Log($"{GetStatName()} 증가: {bonus * level}");
//    }

//    // 해당 아이템 타입의 능력치를 식별하기 위한 함수
//    private string GetStatName()
//    {
//        switch (itemType)
//        {
//            case 1: return "공격력";
//            case 2: return "공격 속도";
//            case 3: return "공격 범위";
//            case 4: return "크리티컬 확률";
//            case 5: return "크리티컬 데미지";
//            case 6: return "적 이동 속도";
//            case 7: return "골드 획득 확률";
//            default: return "알 수 없음";
//        }
//    }
//}
