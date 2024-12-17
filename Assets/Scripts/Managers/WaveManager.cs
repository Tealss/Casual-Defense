using UnityEngine;
using System.Collections;
using System;

public class WaveManager : MonoBehaviour
{
    public static WaveManager I;

    [Header("Wave Settings")]

    [SerializeField] private int maxWaveCount = 100;
    [SerializeField] private int unitCountPerWave = 30;

    [SerializeField] private float unitSpeed = 5f;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float waveDuration = 50f;
    [SerializeField] private float spawnDuration = 30f;
    [SerializeField] private float initialMonsterHealth = 50f;
    [SerializeField] private float healthIncreasePerWavePercentage = 30f;

    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Transform hpSliderParent;

    public int currentWave = 1;
    private bool isSpawning = false;
    private float currentWaveMonsterHealth;
    private float currentWaveTimer;

    private Monster monster;
    private ObjectPool objectPool;

    [Header("Custom Health Settings")]
    private int[] bountyHp = { 3000, 15000, 50000, 200000, 500000, 2000000, 6000000 };
    private int[] bossHp = { 5000, 20000, 50000, 100000, 150000, 300000, 500000, 1000000, 3000000, 10000000 };

    private void Awake()
    {
        if (I == null)
            I = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        objectPool = FindObjectOfType<ObjectPool>();

        if (hpSliderParent == null)
        {
            Debug.LogError("HP Slider parent not assigned.");
        }

        StartCoroutine(StartWaveRoutineWithDelay());
    }

    private IEnumerator StartWaveRoutineWithDelay()
    {
        yield return new WaitForSeconds(3.5f);
        SoundManager.I.PlaySoundEffect(11);
        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        while (currentWave <= maxWaveCount)
        {
            SetupWave();

            if (IsBossWave(currentWave))
                SpawnBossMonster();
            else if (!isSpawning)
                StartCoroutine(SpawnUnits());

            yield return ManageWaveTimer();

            FinishWave();
        }

        Debug.Log("All waves complete!");
    }

    private void SetupWave()
    {
        currentWaveTimer = waveDuration;
        GameUiManager.I.UpdateWaveText(currentWave);
        isSpawning = false;

        currentWaveMonsterHealth = initialMonsterHealth * Mathf.Pow(1 + healthIncreasePerWavePercentage / 100f, currentWave - 1);
    }

    private IEnumerator ManageWaveTimer()
    {
        while (currentWaveTimer > 0)
        {
            currentWaveTimer -= Time.deltaTime;
            GameUiManager.I.UpdateTimerText(Mathf.CeilToInt(currentWaveTimer));
            yield return null;
        }
    }

    private void FinishWave()
    {
        currentWave++;
    }

    private bool IsBossWave(int waveNumber) => waveNumber % 10 == 0;

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
    }

    private void SpawnSingleUnit()
    {
        int prefabIndex = (currentWave % 10 == 7) ? 1 : 0;

        GameObject unit = objectPool.GetFromPool($"Monster_{prefabIndex}", objectPool.monsterPrefabs[prefabIndex]);
        if (unit == null)
        {
            Debug.LogError($"Unable to get Monster_{prefabIndex} from object pool.");
            return;
        }
        Monster monster = unit.GetComponent<Monster>();
        if (monster != null)
        {
            monster.monsterIndex = prefabIndex;
        }
        unit.transform.SetParent(Folder.folder.transform, false);
        unit.transform.position = waypoints[0].position;

        InitializeMonsterWithHpSlider(unit);
    }

    private void InitializeMonsterWithHpSlider(GameObject monsterObject, int customHealth = -1)
    {
        Monster monster = monsterObject.GetComponent<Monster>();
        if (monster != null)
        {
            InitializeMonster(monster, customHealth);
            AttachHpSliderToMonster(monsterObject);
        }
        else
        {
            Debug.LogError("Monster component missing from monster object.");
        }
    }

    private void InitializeMonster(Monster monster, int customHealth = -1)
    {
        if (customHealth > 0)
            monster.SetMaxHealth(customHealth);
        else
            monster.SetMaxHealth(currentWaveMonsterHealth);

        monster.Initialize(waypoints, unitSpeed, objectPool);
    }

    private void AttachHpSliderToMonster(GameObject monsterObject)
    {
        GameObject hpSlider = objectPool.GetFromPool("HealthBar", objectPool.healthBarPrefab);
        if (hpSlider == null)
        {
            Debug.LogError("Unable to get health bar from object pool.");
            return;
        }

        hpSlider.transform.SetParent(hpSliderParent, false);
        hpSlider.transform.position = monsterObject.transform.position + Vector3.up * 2f;

        if (hpSlider.TryGetComponent(out MonsterHPSlider hpSliderScript))
            hpSliderScript.Initialize(monsterObject);

        if (monsterObject.TryGetComponent(out Monster monster))
            monster.SetHpSlider(hpSlider);
    }

    private void SpawnBossMonster()
    {
        int bossIndex = (currentWave / 10) - 1;
        if (bossIndex < 0 || bossIndex >= objectPool.bossMonsterPrefabs.Length)
        {
            Debug.LogWarning("Invalid boss index.");
            return;
        }

        GameObject bossMonster = Instantiate(objectPool.bossMonsterPrefabs[bossIndex], waypoints[0].position, Quaternion.identity);
        int bossHealth = (bossIndex < bossHp.Length) ? bossHp[bossIndex] : 100000; // 기본값 설정
        InitializeMonsterWithHpSlider(bossMonster, bossHealth);

        Monster monster = bossMonster.GetComponent<Monster>();
        if (monster != null)
        {
            monster.bossIndex = bossIndex;
        }
    }

    public void SpawnBountyMonster(int index)
    {
        if (index < 0 || index >= objectPool.bountyMonsterPrefabs.Length)
        {
            Debug.LogWarning($"Invalid bounty monster index: {index}");
            return;
        }

        GameObject bountyMonster = Instantiate(objectPool.bountyMonsterPrefabs[index], waypoints[0].position, Quaternion.identity);
        int bountyHealth = (index < bountyHp.Length) ? bountyHp[index] : 1000; // 기본값 설정
        InitializeMonsterWithHpSlider(bountyMonster, bountyHealth);

        Monster monster = bountyMonster.GetComponent<Monster>();
        if (monster != null)
        {
            monster.bountyIndex = index;
        }

    }


}
