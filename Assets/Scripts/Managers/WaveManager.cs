using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [Header("유닛 이동")]
    [SerializeField]
    public GameObject unitPrefab; // 생성할 유닛 프리팹
    public Transform[] waypoints; // 웨이포인트 배열

    [SerializeField]
    public float unitSpeed = 5f;  // 유닛 이동 속도
    public int maxWaveCount = 100; // 최대 웨이브 수
    public int unitCountPerWave = 30; // 웨이브당 생성할 유닛 수
    public float spawnInterval = 1f; // 유닛 생성 간격 (1초)
    public float waveDuration = 60f; // 각 웨이브의 지속 시간 (60초)
    public float spawnDuration = 30f; // 유닛 생성이 이루어지는 시간 (30초)


    private List<GameObject> activeUnits = new List<GameObject>(); // 현재 활성화된 유닛 목록
    private int currentWave = 1; // 현재 웨이브
    private bool isSpawning = false; // 유닛 생성 중인지 확인

    void Start()
    {

        StartCoroutine(WaveRoutine());
    }

    // 웨이브 관리 코루틴
    private IEnumerator WaveRoutine()
    {
        while (currentWave <= maxWaveCount)
        {
            Debug.Log($"Wave {currentWave} 시작!");

            // 유닛 생성 시작 (30초 동안)
            isSpawning = true;
            StartCoroutine(SpawnUnits());

            // 웨이브 60초 동안 지속
            yield return new WaitForSeconds(waveDuration);

            // 웨이브 종료 처리
            Debug.Log($"Wave {currentWave} 종료!");

            // 다음 웨이브로 이동
            currentWave++;
        }

        Debug.Log("모든 웨이브 종료!");
    }

    // 유닛 생성 코루틴 (30초 동안 1초 간격으로 유닛 생성)
    private IEnumerator SpawnUnits()
    {
        int unitsSpawned = 0;
        float spawnEndTime = Time.time + spawnDuration; // 30초 동안만 유닛 생성

        while (unitsSpawned < unitCountPerWave && Time.time < spawnEndTime)
        {
            // 유닛 생성
            GameObject unit = Instantiate(unitPrefab, waypoints[0].position, Quaternion.identity);
            activeUnits.Add(unit);

            // 유닛 이동 초기화
            UnitMovement unitMovement = unit.GetComponent<UnitMovement>();
            unitMovement.Initialize(waypoints, unitSpeed);

            unitsSpawned++;
            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
    }
}
