using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    [Header("유닛 이동")]
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Transform[] waypoints;

    [Header("HP Slider")]
    [SerializeField] private GameObject hpSliderPrefab; // HP Slider 프리팹

    [SerializeField] private float unitSpeed = 5f;
    [SerializeField] private int maxWaveCount = 100;
    [SerializeField] private int unitCountPerWave = 30;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float waveDuration = 60f;
    [SerializeField] private float spawnDuration = 30f;

    private List<GameObject> activeUnits = new List<GameObject>();
    private List<GameObject> activeSliders = new List<GameObject>(); // 생성된 HP 슬라이더 목록
    private int currentWave = 1;
    private bool isSpawning = false;
    private float currentWaveTimer;

    private GameUiManager gameUIManager;

    void Start()
    {
        gameUIManager = FindObjectOfType<GameUiManager>();
        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        while (currentWave <= maxWaveCount)
        {
            Debug.Log($"Wave {currentWave} 시작!");

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
            Debug.Log($"Wave {currentWave} 종료!");
            currentWave++;
        }

        Debug.Log("모든 웨이브 종료!");
    }

    private IEnumerator SpawnUnits()
    {
        int unitsSpawned = 0;
        float spawnEndTime = Time.time + spawnDuration;

        // UI 하이어라키에서 HPSlider 폴더 찾기
        //Transform hpSliderParent = GameObject.Find("UI/HPSlider").transform;

        while (unitsSpawned < unitCountPerWave && Time.time < spawnEndTime)
        {
            // 유닛 생성
            GameObject unit = Instantiate(unitPrefab, waypoints[0].position, Quaternion.identity);
            activeUnits.Add(unit);

            //// HP Slider 생성 및 초기화
            //GameObject hpSlider = Instantiate(hpSliderPrefab, hpSliderParent);
            //activeSliders.Add(hpSlider);

            //hpSlider.GetComponent<UnitHP>().Initialize(unit);

            // 유닛 이동 초기화
            UnitMovement unitMovement = unit.GetComponent<UnitMovement>();
            unitMovement.Initialize(waypoints, unitSpeed);

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
            Destroy(activeSliders[index]); // HP Slider 제거
            activeUnits.RemoveAt(index);
            activeSliders.RemoveAt(index);
        }

        Destroy(unit); // 유닛 제거
    }
}
