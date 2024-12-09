using UnityEngine;
using System.Collections;

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

    private void Start()
    {
        gameUIManager = FindObjectOfType<GameUiManager>();
        objectPool = FindObjectOfType<ObjectPool>();

        if (hpSliderParent == null)
        {
            Debug.LogError("HP Slider parent not assigned.");
        }

        StartCoroutine(StartWaveRoutineWithDelay());
    }

    private IEnumerator StartWaveRoutineWithDelay()
    {
        yield return new WaitForSeconds(3f);
        SoundManager.I.PlaySoundEffect(11);
        StartCoroutine(WaveRoutine());
    }

    public void SpawnBountyMonster(int index)
    {
        if (index < 0 || index >= objectPool.bountyMonsterPrefabs.Length)
        {
            Debug.LogWarning($"Invalid bounty monster index: {index}");
            return;
        }

        GameObject bountyMonster = Instantiate(objectPool.bountyMonsterPrefabs[index], waypoints[0].position, Quaternion.identity);
        InitializeMonsterWithHpSlider(bountyMonster, index);
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
        gameUIManager.UpdateWaveText(currentWave);
        CalculateWaveHealthMultiplier();
        isSpawning = false;
    }

    private IEnumerator ManageWaveTimer()
    {
        while (currentWaveTimer > 0)
        {
            currentWaveTimer -= Time.deltaTime;
            gameUIManager.UpdateTimerText(Mathf.CeilToInt(currentWaveTimer));
            yield return null;
        }
    }

    private void FinishWave()
    {
        currentWave++;
    }

    private bool IsBossWave(int waveNumber) => waveNumber % 10 == 0;

    private void SpawnBossMonster()
    {
        int bossIndex = (currentWave / 10) - 1;
        if (bossIndex < 0 || bossIndex >= objectPool.bossMonsterPrefabs.Length)
        {
            Debug.LogWarning("Invalid boss index.");
            return;
        }

        GameObject bossMonster = Instantiate(objectPool.bossMonsterPrefabs[bossIndex], waypoints[0].position, Quaternion.identity);
        InitializeMonsterWithHpSlider(bossMonster);
    }

    private void CalculateWaveHealthMultiplier()
    {
        waveHealthMultiplier = Mathf.Pow(1.5f, Mathf.Max(currentWave - 1, 0));
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
    }

    private void SpawnSingleUnit()
    {
        GameObject unit = objectPool.GetFromPool("Monster", objectPool.monsterPrefab);
        if (unit == null)
        {
            Debug.LogError("Unable to get monster from object pool.");
            return;
        }

        unit.transform.SetParent(Folder.folder.transform, false);
        unit.transform.position = waypoints[0].position;

        InitializeMonsterWithHpSlider(unit);
    }

    private void InitializeMonsterWithHpSlider(GameObject monsterObject, int bountyIndex = -1)
    {
        Monster monster = monsterObject.GetComponent<Monster>();
        if (monster != null)
        {
            if (bountyIndex != -1)
                monster.bountyIndex = bountyIndex;

            InitializeMonster(monster);
            AttachHpSliderToMonster(monsterObject);
        }
    }

    private void InitializeMonster(Monster monster)
    {
        float calculatedMaxHealth = monster.maxHealth * waveHealthMultiplier;
        monster.SetMaxHealth(calculatedMaxHealth);
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
}
