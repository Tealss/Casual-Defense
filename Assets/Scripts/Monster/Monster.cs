using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("몬스터 설정")]
    public float MaxHealth = 100f;
    public float CurrentHealth;

    private GameObject hpSlider;  // HP 슬라이더 참조

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