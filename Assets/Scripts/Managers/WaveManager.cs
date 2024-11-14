using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    [Header("���� �̵�")]
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Transform[] waypoints;

    [Header("HP �����̴�")]
    [SerializeField] private GameObject hpSliderPrefab;
    [SerializeField] private Transform hpSliderParent;

    [Header("���̺� ����")]
    [SerializeField] private float unitSpeed = 5f;
    [SerializeField] private int maxWaveCount = 100;
    [SerializeField] private int unitCountPerWave = 30;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float waveDuration = 60f;
    [SerializeField] private float spawnDuration = 30f;

    private List<GameObject> activeUnits = new List<GameObject>();
    private List<GameObject> activeSliders = new List<GameObject>();
    private int currentWave = 1;
    private bool isSpawning = false;
    private float currentWaveTimer;

    private GameUiManager gameUIManager;

    void Start()
    {
        gameUIManager = FindObjectOfType<GameUiManager>();

        // �����̴� �θ� üũ
        if (hpSliderParent == null)
        {
            Debug.LogError("hpSliderParent�� null�Դϴ�. UI/Canvas/HpSlider�� �Ҵ���� �ʾҽ��ϴ�.");
        }

        if (hpSliderPrefab == null)
        {
            Debug.LogError("hpSliderPrefab�� null�Դϴ�. �����̴� �������� �Ҵ���� �ʾҽ��ϴ�.");
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

        // HpSlider�� �θ� UI ��ġ ã��
        Transform hpSliderParent = GameObject.Find("UI/Canvas/HpSlider").transform;

        while (unitsSpawned < unitCountPerWave && Time.time < spawnEndTime)
        {
            // ���� ����
            GameObject unit = Instantiate(unitPrefab, waypoints[0].position, Quaternion.identity);
            activeUnits.Add(unit);

            // HP �����̴� ����
            GameObject hpSlider = Instantiate(hpSliderPrefab, hpSliderParent);

            // HP �����̴� �ʱ�ȭ
            MonsterHPSlider hpSliderScript = hpSlider.GetComponent<MonsterHPSlider>();
            if (hpSliderScript != null)
            {
                hpSliderScript.Initialize(unit); // ���⼭ ����(GameObject) ����
            }
            else
            {
                Debug.LogError("MonsterHPSlider ������Ʈ�� ã�� �� �����ϴ�.");
            }

            // ���� �̵� �ʱ�ȭ
            UnitMovement unitMovement = unit.GetComponent<UnitMovement>();
            if (unitMovement != null)
            {
                unitMovement.Initialize(waypoints, unitSpeed);
            }

            unitsSpawned++;
            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
    }

    public void RemoveUnit(GameObject unit)
    {
        int index = activeUnits.IndexOf(unit);

        if (index != -1)
        {
            Destroy(activeSliders[index]); 
            activeUnits.RemoveAt(index);
            activeSliders.RemoveAt(index);
        }

        Destroy(unit);
    }

}
