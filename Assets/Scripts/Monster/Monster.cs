using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("���� �⺻ ����")]
    public float MaxHealth = 100f;
    public float CurrentHealth;

    private MonsterHPSlider hpSlider;

    void Start()
    {
        CurrentHealth = MaxHealth;
        hpSlider = GetComponentInChildren<MonsterHPSlider>();

        if (hpSlider == null)
        {
            Debug.LogError("MonsterHPSlider�� ã�� �� �����ϴ�. �ڽ� ������Ʈ�� �ִ��� Ȯ���ϼ���.");
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

    // ü�� ���� �޼���
    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth < 0) CurrentHealth = 0; // ü���� 0 �̸����� �������� �ʵ��� ����

        // ü�� ������Ʈ
        if (hpSlider != null)
        {
            hpSlider.UpdateHealth();  // ü�� ���� �� �����̴� ������Ʈ
        }
    }

    // ü�� ȸ�� �޼���
    public void Heal(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth; // ü���� �ִ� ü���� �ʰ����� �ʵ��� ����

        // ü�� ������Ʈ
        if (hpSlider != null)
        {
            hpSlider.UpdateHealth();  // ü�� ���� �� �����̴� ������Ʈ
        }
    }
}