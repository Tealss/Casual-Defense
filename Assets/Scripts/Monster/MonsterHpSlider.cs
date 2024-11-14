using UnityEngine;
using UnityEngine.UI;

public class MonsterHPSlider : MonoBehaviour
{
    private Monster monster; // ���� ��ũ��Ʈ ����
    private Slider slider; // UI �����̴� ����

    void Awake()
    {
        // �����̴� ������Ʈ �Ҵ�
        slider = GetComponent<Slider>();
        if (slider == null)
        {
            Debug.LogError("�����̴� ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    // �ʱ�ȭ �޼���
    public void Initialize(GameObject unit)
    {
        // ���ֿ��� Monster ��ũ��Ʈ�� �����ɴϴ�.
        monster = unit.GetComponent<Monster>();

        if (monster == null)
        {
            Debug.LogError("Monster ��ũ��Ʈ�� ã�� �� �����ϴ�.");
            return;
        }

        SetMaxHealth(monster.MaxHealth);
        UpdateHealth();
    }

    // ü�� �ִ밪 ����
    public void SetMaxHealth(float maxHealth)
    {
        if (slider == null) return;

        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    // ü�� ������Ʈ
    public void UpdateHealth()
    {
        if (monster == null || slider == null)
        {
            Debug.LogError("�����̴��� ���Ͱ� null�Դϴ�.");
            return;
        }

        slider.value = monster.CurrentHealth;
    }

    // �� �����Ӹ��� �����̴� ��ġ�� ü�� ������Ʈ
    void Update()
    {
        if (monster == null || slider == null) return;

        UpdateHealth();

        // HP �����̴� ��ġ ������Ʈ
        Vector3 worldPosition = monster.transform.position + new Vector3(0, 2f, 0);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        transform.position = screenPosition;
    }
}
