using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("���� ����")]
    public float MaxHealth = 100f;
    public float CurrentHealth;

    private GameObject hpSlider;  // HP �����̴� ����

    void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void SetHpSlider(GameObject slider)
    {
        hpSlider = slider;
    }

    public GameObject GetHpSlider()
    {
        return hpSlider;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
        }
    }
}