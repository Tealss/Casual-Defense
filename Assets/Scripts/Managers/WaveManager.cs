using UnityEngine;
using System.Collections;
using System;

public class WaveManager : MonoBehaviour
{
    public static WaveManager I;

    [Header("Wave Set")]
    [SerializeField] private float unitSpeed = 5f;
    [SerializeField] private int maxWaveCount = 100;
    [SerializeField] private int unitCountPerWave = 30;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float waveDuration = 50f;
    [SerializeField] private float spawnDuration = 30f;

    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Transform hpSliderParent;

    public int currentWave = 1;
    private bool isSpawning = false;
    private float currentWaveTimer;
    private float waveHealthMultiplier = 1f;

    private GameUiManager gameUIManager;
    private ObjectPool objectPool;

    private void Awake()
    {
        if (I == null)
            I = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        gameUIManager = FindObjectOfType<GameUiManager>();
        objectPool = FindObjectOfType<ObjectPool>();

        if (hpSliderParent == null)
        {
            Debug.LogError("HP Slider parent check");
        }

        //StartCoroutine(WaveRoutine());
    }

    public void SpawnBountyMonster(int index)
    {
        if (index >= 0 && index < objectPool.bountyMonsterPrefabs.Length)
        {
            GameObject bountyMonster = Instantiate(objectPool.bountyMonsterPrefabs[index], transform.position, Quaternion.identity);
            bountyMonster.transform.position = waypoints[0].position;

            Monster monster = bountyMonster.GetComponent<Monster>();
            if (monster != null)
            {
                monster.bountyIndex = index;
                InitializeMonster(monster);
                AttachHpSliderToMonster(bountyMonster);
            }
        }
        else
        {
            //Debug.LogWarning("Invalid bounty monster index.");
        }
    }

    private IEnumerator WaveRoutine()
    {
        while (currentWave <= maxWaveCount)
        {
            currentWaveTimer = waveDuration;
            gameUIManager.UpdateWaveText(currentWave);

            CalculateWaveHealthMultiplier();

            if (!isSpawning)
            {
                isSpawning = true;

                if (IsBossWave(currentWave))
                {
                    SpawnBossMonster();
                }
                else
                {
                    StartCoroutine(SpawnUnits());
                }
            }

            while (currentWaveTimer > 0)
            {
                currentWaveTimer -= Time.deltaTime;
                gameUIManager.UpdateTimerText(Mathf.CeilToInt(currentWaveTimer));
                yield return null;
            }

            isSpawning = false;
            currentWave++;
        }

        Debug.Log("All wave END!");
    }

    private bool IsBossWave(int waveNumber)
    {
        return waveNumber % 10 == 0;
    }

    private void SpawnBossMonster()
    {
        int bossIndex = (currentWave / 10) - 1;
        if (bossIndex >= 0 && bossIndex < objectPool.bossMonsterPrefabs.Length)
        {
            GameObject bossMonster = Instantiate(objectPool.bossMonsterPrefabs[bossIndex], transform.position, Quaternion.identity);
            bossMonster.transform.position = waypoints[0].position;

            Monster monster = bossMonster.GetComponent<Monster>();
            if (monster != null)
            {
                InitializeMonster(monster);
                AttachHpSliderToMonster(bossMonster);
            }
        }
        else
        {
            Debug.LogWarning("Invalid boss index.");
        }
    }

    private void CalculateWaveHealthMultiplier()
    {
        if (waveHealthMultiplier == 1f)
        {
            waveHealthMultiplier = Mathf.Pow(1.5f, Mathf.Max(currentWave - 1, 0));
            //Debug.Log($"Wave {currentWave} - Health Multiplier: {waveHealthMultiplier}");
        }
    }

    private IEnumerator SpawnUnits()
    {
        int unitsSpawned = 0;
        float spawnEndTime = Time.time + spawnDuration;

        while (unitsSpawned < unitCountPerWave && Time.time < spawnEndTime)
        {
            SpawnSingleUnit();
            unitsSpawned++;
            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
    }

    private void SpawnSingleUnit()
    {
        GameObject unit = objectPool.GetFromPool("Monster", objectPool.monsterPrefab);
        if (unit == null)
        {
            Debug.LogError("Can't get the monster from object pool");
            return;
        }

        unit.transform.SetParent(Folder.folder.transform, false);
        unit.transform.position = waypoints[0].position;

        Monster monster = unit.GetComponent<Monster>();
        if (monster != null)
        {
            InitializeMonster(monster);
            AttachHpSliderToMonster(unit);
        }
    }

    private void InitializeMonster(Monster monster)
    {
        float calculatedMaxHealth = monster.maxHealth * waveHealthMultiplier;
        monster.SetMaxHealth(calculatedMaxHealth);
        monster.Initialize(waypoints, unitSpeed, objectPool);

        //Debug.Log($"Wave {currentWave} - Initial Health: {monster.maxHealth}, Multiplier: {waveHealthMultiplier}, Calculated Max Health: {calculatedMaxHealth}");
    }

    private void AttachHpSliderToMonster(GameObject unit)
    {
        GameObject hpSlider = objectPool.GetFromPool("HealthBar", objectPool.healthBarPrefab);
        if (hpSlider == null)
        {
            Debug.LogError("Can't get the healthBar from object pool.");
            return;
        }

        hpSlider.transform.SetParent(hpSliderParent, false);

        Vector3 unitPosition = unit.transform.position;
        hpSlider.transform.position = new Vector3(unitPosition.x, unitPosition.y + 2f, unitPosition.z);

        MonsterHPSlider hpSliderScript = hpSlider.GetComponent<MonsterHPSlider>();
        if (hpSliderScript != null)
        {
            hpSliderScript.Initialize(unit);
        }
        else
        {
            Debug.LogError("Cant' find MonsterHPSlider.");
        }

        Monster monster = unit.GetComponent<Monster>();
        if (monster != null)
        {
            monster.SetHpSlider(hpSlider);
        }
    }
}
