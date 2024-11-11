using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [Header("���� �̵�")]
    [SerializeField]
    public GameObject unitPrefab; // ������ ���� ������
    public Transform[] waypoints; // ��������Ʈ �迭

    [SerializeField]
    public float unitSpeed = 5f;  // ���� �̵� �ӵ�
    public int maxWaveCount = 100; // �ִ� ���̺� ��
    public int unitCountPerWave = 30; // ���̺�� ������ ���� ��
    public float spawnInterval = 1f; // ���� ���� ���� (1��)
    public float waveDuration = 60f; // �� ���̺��� ���� �ð� (60��)
    public float spawnDuration = 30f; // ���� ������ �̷������ �ð� (30��)


    private List<GameObject> activeUnits = new List<GameObject>(); // ���� Ȱ��ȭ�� ���� ���
    private int currentWave = 1; // ���� ���̺�
    private bool isSpawning = false; // ���� ���� ������ Ȯ��

    void Start()
    {

        StartCoroutine(WaveRoutine());
    }

    // ���̺� ���� �ڷ�ƾ
    private IEnumerator WaveRoutine()
    {
        while (currentWave <= maxWaveCount)
        {
            Debug.Log($"Wave {currentWave} ����!");

            // ���� ���� ���� (30�� ����)
            isSpawning = true;
            StartCoroutine(SpawnUnits());

            // ���̺� 60�� ���� ����
            yield return new WaitForSeconds(waveDuration);

            // ���̺� ���� ó��
            Debug.Log($"Wave {currentWave} ����!");

            // ���� ���̺�� �̵�
            currentWave++;
        }

        Debug.Log("��� ���̺� ����!");
    }

    // ���� ���� �ڷ�ƾ (30�� ���� 1�� �������� ���� ����)
    private IEnumerator SpawnUnits()
    {
        int unitsSpawned = 0;
        float spawnEndTime = Time.time + spawnDuration; // 30�� ���ȸ� ���� ����

        while (unitsSpawned < unitCountPerWave && Time.time < spawnEndTime)
        {
            // ���� ����
            GameObject unit = Instantiate(unitPrefab, waypoints[0].position, Quaternion.identity);
            activeUnits.Add(unit);

            // ���� �̵� �ʱ�ȭ
            UnitMovement unitMovement = unit.GetComponent<UnitMovement>();
            unitMovement.Initialize(waypoints, unitSpeed);

            unitsSpawned++;
            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
    }
}
