using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    public float maxHealth;
    public float addHealth;
    public float currentHealth;
    public float addGold;

    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private float speed;
    private float originalSpeed;
    public int bountyIndex;
    private ObjectPool objectPool;
    private GameObject hpSlider;

    private bool isAlive = true;
    private Slider hpSliderComponent;
    private Text hpText;

    private Coroutine slowCoroutine;
    private bool isSlowed = false;
    private float currentSlowAmount = 0f;

    private List<Renderer> renderers = new List<Renderer>();
    private Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();

    private bool isBountyMonster = false;

    private void Start()
    {
        currentHealth = maxHealth;

        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderers.Add(renderer);
            originalColors[renderer] = renderer.material.color;
        }

        isBountyMonster = this.CompareTag("Bounty");
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
        this.originalSpeed = speed;
        this.objectPool = objectPool;
        this.currentWaypointIndex = 0;
        isAlive = true;

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
                GameManager.I.DecreaseLifePoints(1);
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

            int goldMultiplier = GetGoldMultiplierForBounty(bountyIndex);

            int killGold = 50  * goldMultiplier;

            string addGoldText = $"+ {killGold}";
            Color textColor = Color.yellow;

            FadeOutTextUse fadeOutTextSpawner = FindObjectOfType<FadeOutTextUse>();
            if (fadeOutTextSpawner != null)
            {
                fadeOutTextSpawner.SpawnFadeOutText(spawnPosition, addGoldText, textColor);
            }

            GameManager.I.AddGold(killGold);
        }

        UpdateHpUI();
    }

    private int GetGoldMultiplierForBounty(int bountyIndex)
    {
        switch (bountyIndex)
        {
            case 0: return 1;
            case 1: return 10;
            case 2: return 20;
            case 3: return 30;
            case 4: return 40;
            case 5: return 50;
            case 6: return 60;
            case 7: return 70;
            default: return 1;
        }
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
        RestoreOriginalColor();

        if (isSlowed)
        {
            StopCoroutine(slowCoroutine);
            speed = originalSpeed;
            currentSlowAmount = 0f;
            isSlowed = false;
        }

        currentWaypointIndex = 0;
        isAlive = false;

        if (isBountyMonster)
        {
            if (hpSlider != null)
            {
                objectPool.ReturnToPool("HealthBar", hpSlider);
                hpSlider = null;
            }

            Destroy(gameObject);
        }
        else
        {
            if (hpSlider != null)
            {
                objectPool.ReturnToPool("HealthBar", hpSlider);
                hpSlider = null;
            }

            objectPool.ReturnToPool("Monster", gameObject);
        }
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
        if (!isAlive || !gameObject.activeInHierarchy) return;

        if (isSlowed)
        {
            return;
        }

        currentSlowAmount = slowAmount;
        speed -= slowAmount;
        isSlowed = true;

        slowCoroutine = StartCoroutine(SlowCoroutine(duration));
    }
    private IEnumerator SlowCoroutine(float duration)
    {
        ChangeColor(Color.cyan);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (!isAlive || !gameObject.activeInHierarchy) yield break;
            elapsed += Time.deltaTime;
            yield return null;
        }

        RestoreOriginalColor();
        speed = originalSpeed;
        currentSlowAmount = 0f;
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
    public void ResetState()
    {
        maxHealth = currentHealth = maxHealth;
        speed = originalSpeed;
        isAlive = true;

        if (isSlowed && slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
            isSlowed = false;
        }

        RestoreOriginalColor();

        if (hpSliderComponent != null)
        {
            hpSliderComponent.value = 1f;
        }

        if (hpText != null)
        {
            hpText.text = $"{(int)maxHealth}";
        }

        bountyIndex = -1;
    }
}
