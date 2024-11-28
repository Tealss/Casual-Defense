using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public static WaveManager I;

    [Header("���� �̵�")]
    [SerializeField] private Transform[] waypoints;
    [Header("HP �����̴�")]
    [SerializeField] private Transform hpSliderParent;

    [Header("���̺� ����")]
    [SerializeField] private float unitSpeed = 5f;
    [SerializeField] private int maxWaveCount = 100;
    [SerializeField] private int unitCountPerWave = 30;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float waveDuration = 60f;
    [SerializeField] private float spawnDuration = 30f;

    public int currentWave = 1;
    private bool isSpawning = false;
    private float currentWaveTimer;

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
            Debug.LogError("HP �����̴� �θ� �������� �ʾҽ��ϴ�. UI/Canvas/HpSlider�� Ȯ���ϼ���.");
        }

        StartCoroutine(WaveRoutine());
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
            Debug.LogWarning("Invalid bounty monster index.");
        }
    }

    private IEnumerator WaveRoutine()
    {
        while (currentWave <= maxWaveCount)
        {

            //Debug.Log($"Wave {currentWave} ����!");

            currentWaveTimer = waveDuration;
            gameUIManager.UpdateWaveText(currentWave);

            if (!isSpawning)
            {
                isSpawning = true;
                StartCoroutine(SpawnUnits());
            }

            while (currentWaveTimer > 0)
            {
                currentWaveTimer -= Time.deltaTime;
                gameUIManager.UpdateTimerText(Mathf.CeilToInt(currentWaveTimer));
                yield return null;
            }

            isSpawning = false;
            Debug.Log($"Wave {currentWave} ����!");
            currentWave++;
        }

        Debug.Log("��� ���̺� ����!");
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
            Debug.LogError("������Ʈ Ǯ���� ������ ������ �� �����ϴ�.");
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
        float calculatedMaxHealth = CalculateMonsterMaxHealthForWave(monster);
        monster.SetMaxHealth(calculatedMaxHealth);
        monster.Initialize(waypoints, unitSpeed, objectPool);
    }

    private void AttachHpSliderToMonster(GameObject unit)
    {
        GameObject hpSlider = objectPool.GetFromPool("HealthBar", objectPool.healthBarPrefab);
        if (hpSlider == null)
        {
            Debug.LogError("������Ʈ Ǯ���� HP �����̴��� ������ �� �����ϴ�.");
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
            Debug.LogError("MonsterHPSlider ������Ʈ�� ã�� �� �����ϴ�.");
        }

        Monster monster = unit.GetComponent<Monster>();
        if (monster != null)
        {
            monster.SetHpSlider(hpSlider);
        }
    }

    private float CalculateMonsterMaxHealthForWave(Monster monster)
    {
        float healthMultiplier = Mathf.Pow(1.5f, currentWave - 1);
        return monster.maxHealth * healthMultiplier;
    }
}
