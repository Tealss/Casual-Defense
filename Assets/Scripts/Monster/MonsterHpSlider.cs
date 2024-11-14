using UnityEngine;
using UnityEngine.UI;

public class MonsterHPSlider : MonoBehaviour
{
    private Monster monster; // 몬스터 스크립트 참조
    private Slider slider; // UI 슬라이더 참조

    void Awake()
    {
        // 슬라이더 컴포넌트 할당
        slider = GetComponent<Slider>();
        if (slider == null)
        {
            Debug.LogError("슬라이더 컴포넌트를 찾을 수 없습니다.");
        }
    }

    // 초기화 메서드
    public void Initialize(GameObject unit)
    {
        // 유닛에서 Monster 스크립트를 가져옵니다.
        monster = unit.GetComponent<Monster>();

        if (monster == null)
        {
            Debug.LogError("Monster 스크립트를 찾을 수 없습니다.");
            return;
        }

        SetMaxHealth(monster.MaxHealth);
        UpdateHealth();
    }

    // 체력 최대값 설정
    public void SetMaxHealth(float maxHealth)
    {
        if (slider == null) return;

        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    // 체력 업데이트
    public void UpdateHealth()
    {
        if (monster == null || slider == null)
        {
            Debug.LogError("슬라이더나 몬스터가 null입니다.");
            return;
        }

        slider.value = monster.CurrentHealth;
    }

    // 매 프레임마다 슬라이더 위치와 체력 업데이트
    void Update()
    {
        if (monster == null || slider == null) return;

        UpdateHealth();

        // HP 슬라이더 위치 업데이트
        Vector3 worldPosition = monster.transform.position + new Vector3(0, 2f, 0);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        transform.position = screenPosition;
    }
}
