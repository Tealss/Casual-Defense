using UnityEngine;
using UnityEngine.UI;

public class MonsterHPSlider : MonoBehaviour
{
    private Slider slider;
    private Monster monster;

    void Start() // Start로 변경
    {
        // 슬라이더 컴포넌트를 초기화
        slider = GetComponentInChildren<Slider>();
        if (slider == null)
        {
            Debug.LogError("슬라이더가 자식 오브젝트에 없습니다!");
        }
    }

    public void Initialize(Monster monster)
    {
        if (monster == null)
        {
            Debug.LogError("Monster 객체가 null입니다!");
            return;
        }

        this.monster = monster;

        // 초기화 후 체력 설정
        SetMaxHealth(monster.MaxHealth);
        UpdateHealth();
    }

    public void SetMaxHealth(float maxHealth)
    {
        if (slider == null)
        {
            Debug.LogError("슬라이더가 초기화되지 않았습니다.");
            return;
        }

        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    public void UpdateHealth()
    {
        if (slider == null || monster == null)
        {
            Debug.LogError("슬라이더나 몬스터가 null입니다.");
            return;
        }

        slider.value = monster.CurrentHealth;
    }
}
