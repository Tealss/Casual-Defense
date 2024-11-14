using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("몬스터 기본 정보")]
    public float MaxHealth = 100f;
    public float CurrentHealth;

    private MonsterHPSlider hpSlider;

    void Start()
    {
        CurrentHealth = MaxHealth;
        hpSlider = GetComponentInChildren<MonsterHPSlider>();

        if (hpSlider == null)
        {
            Debug.LogError("MonsterHPSlider를 찾을 수 없습니다. 자식 오브젝트에 있는지 확인하세요.");
        }
        else
        {
            hpSlider.Initialize(this);
        }
    }

    void Update()
    {
        if (hpSlider != null)
        {
            hpSlider.UpdateHealth();
        }
    }

    // 체력 감소 메서드
    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth < 0) CurrentHealth = 0; // 체력이 0 미만으로 내려가지 않도록 방지

        // 체력 업데이트
        if (hpSlider != null)
        {
            hpSlider.UpdateHealth();  // 체력 변경 시 슬라이더 업데이트
        }
    }

    // 체력 회복 메서드
    public void Heal(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth; // 체력이 최대 체력을 초과하지 않도록 제한

        // 체력 업데이트
        if (hpSlider != null)
        {
            hpSlider.UpdateHealth();  // 체력 변경 시 슬라이더 업데이트
        }
    }
}