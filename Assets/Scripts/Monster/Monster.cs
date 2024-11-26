using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    public float maxHealth = 0f;
    public float currentHealth;

    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private float speed;
    private GameManager gameManager;
    private ObjectPool objectPool;
    private GameObject hpSlider;

    private bool isAlive = true;
    private Slider hpSliderComponent;
    private Text hpText;

    private Coroutine slowCoroutine;
    private bool isSlowed = false;

    private List<Renderer> renderers = new List<Renderer>();
    private Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();

    private void Start()
    {
        currentHealth = maxHealth;

        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderers.Add(renderer);
            originalColors[renderer] = renderer.material.color;
        }

    }

    public void SetMaxHealth(float value)
    {
        maxHealth = value;
        currentHealth = maxHealth;
    }

    public void Initialize(Transform[] waypoints, float speed, ObjectPool objectPool)
    {
        currentHealth = maxHealth;
        this.waypoints = waypoints;
        this.speed = speed;
        this.objectPool = objectPool;
        this.currentWaypointIndex = 0;
        isAlive = true;

        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (!isAlive || waypoints == null || waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        float step = speed * Time.deltaTime;

        Vector3 moveDirection = targetWaypoint.position - transform.position;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, step);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                if (gameManager != null)
                    gameManager.DecreaseLifePoints(1);

                ReturnToPool();
                return;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isAlive = false;
            ReturnToPool();

            Vector3 spawnPosition = transform.position + new Vector3(0.5f, 1.5f, 0);
            string addGoldText = $"+50";
            Color textColor = Color.yellow;

            FadeOutTextUse fadeOutTextSpawner = FindObjectOfType<FadeOutTextUse>();
            if (fadeOutTextSpawner != null)
            {
                fadeOutTextSpawner.SpawnFadeOutText(spawnPosition, addGoldText, textColor);
            }
            GameManager.I.AddGold(50);
        }

        UpdateHpUI();
    }

    private void UpdateHpUI()
    {
        if (hpSliderComponent != null)
        {
            hpSliderComponent.value = currentHealth;
        }

        if (hpText != null)
        {
            hpText.text = $"{(int)currentHealth}";
        }
    }

    private void ReturnToPool()
    {
        currentWaypointIndex = 0;
        isAlive = false;

        if (hpSlider != null)
        {
            string sliderPoolName = "HealthBar";
            objectPool.ReturnToPool(sliderPoolName, hpSlider);
            hpSlider = null;
        }

        string monsterPoolName = "Monster";
        objectPool.ReturnToPool(monsterPoolName, gameObject);
    }

    public void SetHpSlider(GameObject slider)
    {
        hpSlider = slider;

        hpSliderComponent = hpSlider.GetComponent<Slider>();
        if (hpSliderComponent != null)
        {
            hpSliderComponent.maxValue = maxHealth;
            hpSliderComponent.value = currentHealth;
        }

        hpText = hpSlider.GetComponentInChildren<Text>();
        if (hpText != null)
        {
            hpText.text = $"{(int)currentHealth}";
        }
    }

    public void ApplySlow(float slowAmount, float duration)
    {
        if (!isAlive) return;

        if (isSlowed)
        {
            return;
        }

        speed -= slowAmount;
        isSlowed = true;

        slowCoroutine = StartCoroutine(SlowCoroutine(slowAmount, duration));
    }

    private IEnumerator SlowCoroutine(float slowAmount, float duration)
    {
        ChangeColor(Color.cyan);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            ChangeColor(Color.cyan);
            elapsed += Time.deltaTime;
            yield return null;
        }

        RestoreOriginalColor();
        speed += slowAmount;
        isSlowed = false;
    }

    private void ChangeColor(Color color)
    {
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.material.color = color;
            }
        }
    }

    private void RestoreOriginalColor()
    {
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null && originalColors.ContainsKey(renderer))
            {
                renderer.material.color = originalColors[renderer];
            }
        }
    }

}
