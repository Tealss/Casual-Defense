using UnityEngine;
using UnityEngine.UI;

public class MonsterHPSlider : MonoBehaviour
{
    private Monster monster;
    private Slider slider;
    private Image sliderFillImage;

    private Vector3 offset = new Vector3(0, 2f, 0); 

    void Awake()
    {
        slider = GetComponent<Slider>();
        if (slider == null)
        {
            Debug.LogError("slider x");
        }

        sliderFillImage = slider.fillRect.GetComponent<Image>();
    }

    public void Initialize(GameObject unit)
    {
        monster = unit.GetComponent<Monster>();

        if (monster == null)
        {
            Debug.LogError("Monster x");
            return;
        }

        SetMaxHealth(monster.maxHealth);
        UpdateHealth();
    }

    public void SetMaxHealth(float maxHealth)
    {
        if (slider == null) return;

        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    public void UpdateHealth()
    {
        if (monster == null || slider == null)
        {
            Debug.LogError("slider or monster is null");
            return;
        }

        slider.value = monster.currentHealth;

        UpdateSliderColor();
    }

    void Update()
    {
        if (monster == null || slider == null) return;

        UpdateHealth();

        Collider unitCollider = monster.GetComponent<Collider>();
        if (unitCollider == null)
        {
            Debug.LogError("unit coilder x");
            return;
        }

        Vector3 headPosition = unitCollider.bounds.center + new Vector3(0, unitCollider.bounds.extents.y, 0);

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(headPosition);
        screenPosition += new Vector3(0, offset.y, 0);  
        transform.position = screenPosition;
    }

    private void UpdateSliderColor()
    {
        float healthPercentage = monster.currentHealth / monster.maxHealth;

        if (healthPercentage > 0.5f)
        {
            sliderFillImage.color = Color.Lerp(Color.yellow, Color.green, (healthPercentage - 0.5f) * 2);
        }
        else if (healthPercentage > 0.2f)
        {
            sliderFillImage.color = Color.Lerp(Color.red, Color.yellow, (healthPercentage - 0.2f) * 5);
        }
        else
        {
            sliderFillImage.color = Color.red;
        }
    }
}
