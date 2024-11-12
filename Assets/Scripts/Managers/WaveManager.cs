using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    [Header("���� �̵�")]
    [SerializeField]
    public GameObject unitPrefab;
    public Transform[] waypoints;

    [SerializeField]
    public float unitSpeed = 5f;
    public int maxWaveCount = 100;
    public int unitCountPerWave = 30;
    public float spawnInterval = 1f;
    public float waveDuration = 60f;
    public float spawnDuration = 30f;

    private List<GameObject> activeUnits = new List<GameObject>();
    private int currentWave = 1;
    private bool isSpawning = false;
    private float currentWaveTimer;

    private GameUiManager gameUIManager;

    void Start()
    {
        // GameUIManager ã��
        gameUIManager = FindObjectOfType<GameUiManager>();

        // ���̺� ����
        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        while (currentWave <= maxWaveCount)
        {
            Debug.Log($"Wave {currentWave} ����!");

            // Ÿ�̸� �ʱ�ȭ
            currentWaveTimer = waveDuration;

            // UI ������Ʈ
            gameUIManager.UpdateWaveText(currentWave);

            // ���� ���� ����
            isSpawning = true;
            StartCoroutine(SpawnUnits());

            // Ÿ�̸� 1�ʾ� ����
            while (currentWaveTimer > 0)
            {
                currentWaveTimer -= 1f;

                // UI Ÿ�̸� ������Ʈ
                gameUIManager.UpdateTimerText(Mathf.CeilToInt(currentWaveTimer));

                yield return new WaitForSeconds(1f);
            }

            // ���̺� ����
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
            GameObject unit = Instantiate(unitPrefab, waypoints[0].position, Quaternion.identity);
            activeUnits.Add(unit);

            UnitMovement unitMovement = unit.GetComponent<UnitMovement>();
            unitMovement.Initialize(waypoints, unitSpeed);

            unitsSpawned++;
            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
    }
}
