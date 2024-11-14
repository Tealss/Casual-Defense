using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    [Header("유닛 이동")]
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Transform[] waypoints;

    [Header("HP 슬라이더")]
    [SerializeField] private GameObject hpSliderPrefab;
    [SerializeField] private Transform hpSliderParent;

    [Header("웨이브 세팅")]
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

        // 슬라이더 부모 체크
        if (hpSliderParent == null)
        {
            Debug.LogError("hpSliderParent가 null입니다. UI/Canvas/HpSlider가 할당되지 않았습니다.");
        }

        if (hpSliderPrefab == null)
        {
            Debug.LogError("hpSliderPrefab이 null입니다. 슬라이더 프리팹이 할당되지 않았습니다.");
        }

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

        // HpSlider의 부모 UI 위치 찾기
        Transform hpSliderParent = GameObject.Find("UI/Canvas/HpSlider").transform;

        while (unitsSpawned < unitCountPerWave && Time.time < spawnEndTime)
        {
            // 몬스터 생성
            GameObject unit = Instantiate(unitPrefab, waypoints[0].position, Quaternion.identity);
            activeUnits.Add(unit);

            // HP 슬라이더 생성
            GameObject hpSlider = Instantiate(hpSliderPrefab, hpSliderParent);

            // HP 슬라이더 초기화
            MonsterHPSlider hpSliderScript = hpSlider.GetComponent<MonsterHPSlider>();
            if (hpSliderScript != null)
            {
                hpSliderScript.Initialize(unit); // 여기서 몬스터(GameObject) 전달
            }
            else
            {
                Debug.LogError("MonsterHPSlider 컴포넌트를 찾을 수 없습니다.");
            }

            // 유닛 이동 초기화
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
