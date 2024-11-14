using UnityEngine;
using UnityEngine.UI;

public class MonsterHPSlider : MonoBehaviour
{
    private Slider slider;
    private Monster monster;

    void Start() // Start�� ����
    {
        // �����̴� ������Ʈ�� �ʱ�ȭ
        slider = GetComponentInChildren<Slider>();
        if (slider == null)
        {
            Debug.LogError("�����̴��� �ڽ� ������Ʈ�� �����ϴ�!");
        }
    }

    public void Initialize(Monster monster)
    {
        if (monster == null)
        {
            Debug.LogError("Monster ��ü�� null�Դϴ�!");
            return;
        }

        this.monster = monster;

        // �ʱ�ȭ �� ü�� ����
        SetMaxHealth(monster.MaxHealth);
        UpdateHealth();
    }

    public void SetMaxHealth(float maxHealth)
    {
        if (slider == null)
        {
            Debug.LogError("�����̴��� �ʱ�ȭ���� �ʾҽ��ϴ�.");
            return;
        }

        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    public void UpdateHealth()
    {
        if (slider == null || monster == null)
        {
            Debug.LogError("�����̴��� ���Ͱ� null�Դϴ�.");
            return;
        }

        slider.value = monster.CurrentHealth;
    }
}
