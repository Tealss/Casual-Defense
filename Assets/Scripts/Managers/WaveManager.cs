using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    [Header("유닛 이동")]
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
        // GameUIManager 찾기
        gameUIManager = FindObjectOfType<GameUiManager>();

        // 웨이브 시작
        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        while (currentWave <= maxWaveCount)
        {
            Debug.Log($"Wave {currentWave} 시작!");

            // 타이머 초기화
            currentWaveTimer = waveDuration;

            // UI 업데이트
            gameUIManager.UpdateWaveText(currentWave);

            // 유닛 생성 시작
            isSpawning = true;
            StartCoroutine(SpawnUnits());

            // 타이머 1초씩 감소
            while (currentWaveTimer > 0)
            {
                currentWaveTimer -= 1f;

                // UI 타이머 업데이트
                gameUIManager.UpdateTimerText(Mathf.CeilToInt(currentWaveTimer));

                yield return new WaitForSeconds(1f);
            }

            // 웨이브 종료
            Debug.Log($"Wave {currentWave} 종료!");
            currentWave++;
        }

        Debug.Log("모든 웨이브 종료!");
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
