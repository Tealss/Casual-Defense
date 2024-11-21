using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("몬스터 설정")]
    public float MaxHealth = 100f;
    public float CurrentHealth;

    private GameObject hpSlider;
    private bool isAlive = true; 

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
        if (!isAlive) return;

        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            isAlive = false;  
            Die();  
        }
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    private void Die()
    {
        if (hpSlider != null)
        {
            hpSlider.SetActive(false);
        }
        gameObject.SetActive(false);
    }
}
