using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("몬스터 설정")]
    public float MaxHealth = 100f;   // 몬스터의 최대 체력
    public float CurrentHealth;      // 몬스터의 현재 체력

    private WaveManager waveManager;
    private GameObject hpSlider;    // HP 슬라이더를 관리할 변수

    void Start()
    {
        // 초기 체력을 최대 체력으로 설정
        CurrentHealth = MaxHealth;

        // WaveManager 가져오기
        waveManager = FindObjectOfType<WaveManager>();

        // HP 슬라이더 초기화
        hpSlider = transform.Find("HpSlider")?.gameObject;  // 유닛의 자식으로 슬라이더가 있을 경우 찾기
    }

    void Update()
    {
        // 예시로 체력이 0 이하일 때 제거
        if (CurrentHealth <= 0)
        {
            // HP 슬라이더도 함께 제거
            if (hpSlider != null)
            {
                Destroy(hpSlider);
            }

            waveManager.RemoveUnit(gameObject); // WaveManager에서 유닛 제거
        }
    }

    // 데미지를 받는 메서드
    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;

        // 체력 바닥일 때 0으로 고정
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }
    }
}
