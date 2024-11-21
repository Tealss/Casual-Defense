using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
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

    private int currentWave = 1;
    private bool isSpawning = false;
    private float currentWaveTimer;

    private GameUiManager gameUIManager;
    private ObjectPool objectPool;

    void Start()
    {
        gameUIManager = FindObjectOfType<GameUiManager>();
        objectPool = FindObjectOfType<ObjectPool>();

        if (hpSliderParent == null)
        {
            Debug.LogError("hpSliderParent�� null�Դϴ�. UI/Canvas/HpSlider�� �Ҵ���� �ʾҽ��ϴ�.");
        }

        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        while (currentWave <= maxWaveCount)
        {
            Debug.Log($"Wave {currentWave} ����!");

            currentWaveTimer = waveDuration;
            gameUIManager.UpdateWaveText(currentWave);

            if (!isSpawning)
            {
                isSpawning = true;
                StartCoroutine(SpawnUnits());
            }

            while (currentWaveTimer > 0)
            {
                currentWaveTimer -= 1f;
                gameUIManager.UpdateTimerText(Mathf.CeilToInt(currentWaveTimer));
                yield return new WaitForSeconds(1f);
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

        GameObject unitsFolder = GameObject.Find("ObjectPool");
        if (unitsFolder == null)
        {
            unitsFolder = new GameObject("ObjectPool"); // ���� ������ ������ ����
        }

        while (unitsSpawned < unitCountPerWave && Time.time < spawnEndTime)
        {
            GameObject unit = objectPool.GetFromPool(objectPool.unitPool, objectPool.unitPrefab);
            unit.transform.SetParent(unitsFolder.transform, false);
            unit.transform.position = waypoints[0].position;

            GameObject hpSlider = objectPool.GetFromPool(objectPool.hpSliderPool, objectPool.hpSliderPrefab);
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

            UnitMovement unitMovement = unit.GetComponent<UnitMovement>();
            if (unitMovement != null)
            {
                unitMovement.Initialize(waypoints, unitSpeed, objectPool);
                unitMovement.SetHpSlider(hpSlider);
            }

            unitsSpawned++;
            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
    }


    public void RemoveUnit(GameObject unit)
    {
        if (unit != null)
        {
            objectPool.ReturnToPool(unit, objectPool.unitPool);
        }
    }
}
