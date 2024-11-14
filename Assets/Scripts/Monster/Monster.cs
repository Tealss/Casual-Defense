using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("���� ����")]
    public float MaxHealth = 100f;   // ������ �ִ� ü��
    public float CurrentHealth;      // ������ ���� ü��

    private WaveManager waveManager;
    private GameObject hpSlider;    // HP �����̴��� ������ ����

    void Start()
    {
        // �ʱ� ü���� �ִ� ü������ ����
        CurrentHealth = MaxHealth;

        // WaveManager ��������
        waveManager = FindObjectOfType<WaveManager>();

        // HP �����̴� �ʱ�ȭ
        hpSlider = transform.Find("HpSlider")?.gameObject;  // ������ �ڽ����� �����̴��� ���� ��� ã��
    }

    void Update()
    {
        // ���÷� ü���� 0 ������ �� ����
        if (CurrentHealth <= 0)
        {
            // HP �����̴��� �Բ� ����
            if (hpSlider != null)
            {
                Destroy(hpSlider);
            }

            waveManager.RemoveUnit(gameObject); // WaveManager���� ���� ����
        }
    }

    // �������� �޴� �޼���
    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;

        // ü�� �ٴ��� �� 0���� ����
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }
    }
}
