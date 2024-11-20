using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Tower : MonoBehaviour
{
    public TowerStats towerStats;
    public int level = 1;
    public int towerIndex;

    private float attackTimer;
    private ObjectPool objectPool;
    public string towerType;

    public List<Item> items; // ������ ����Ʈ (Ÿ���� ���� �����۵�)

    private LineRenderer rangeIndicator; // ���� ������ ǥ���� LineRenderer
    private bool isRangeVisible = false; // ���� ǥ�� ����
    private static Tower currentSelectedTower = null; // ���� ���õ� Ÿ��

    private void Start()
    {
        objectPool = FindObjectOfType<ObjectPool>();
        if (objectPool == null)
        {
            Debug.LogError("ObjectPool�� ã�� �� ����");
            return;
        }

        attackTimer = 0f;

        InitializeStats();

        if (ItemManager.I != null)
            ItemManager.I.OnItemStatsChanged += UpdateTowerStats;

        // LineRenderer ����
        rangeIndicator = gameObject.AddComponent<LineRenderer>();
        rangeIndicator.positionCount = 0; // ó������ ǥ������ ����
        rangeIndicator.startWidth = 0.1f;
        rangeIndicator.endWidth = 0.1f;
        rangeIndicator.material = new Material(Shader.Find("Sprites/Default"));
        rangeIndicator.startColor = Color.green;
        rangeIndicator.endColor = Color.green;
    }

    public void InitializeStats()
    {
        if (towerStats != null)
        {
            towerStats.ResetStats(); // �⺻������ �ʱ�ȭ
            ApplyItemStats();       // ������ ȿ�� ����
        }
        else
        {
            Debug.LogError($"Ÿ�� ���� �ʱ�ȭ ����: {gameObject.name}");
        }
    }

    private void OnDestroy()
    {
        // �̺�Ʈ ��� ����
        if (ItemManager.I != null)
            ItemManager.I.OnItemStatsChanged -= UpdateTowerStats;
    }

    private void UpdateTowerStats()
    {
        InitializeStats();
    }

    private void ApplyItemStats()
    {
        if (ItemManager.I == null) return;

        for (int slotIndex = 0; slotIndex < ItemManager.I.itemTypesInSlots.Length; slotIndex++)
        {
            if (!ItemManager.I.slotOccupied[slotIndex]) continue;

            int itemType = ItemManager.I.itemTypesInSlots[slotIndex];
            int level = ItemManager.I.currentLevels[slotIndex];
            int grade = ItemManager.I.itemGrades[slotIndex];

            float effect = ItemManager.I.GetItemTypeEffect(itemType, level, grade);

            // ������ Ÿ�Կ� ���� ������ Ÿ�� ���ȿ� ����
            switch (itemType)
            {
                case 0: towerStats.attackDamage += effect; break;
                case 1: towerStats.attackSpeed += effect; break;
                case 2: towerStats.attackRange += effect; break;
                case 3: towerStats.criticalChance += effect; break;
                case 4: towerStats.criticalDamage += effect; break;
                case 5: towerStats.goldEarnAmount += effect; break;
                case 6: towerStats.enemySlowAmount += effect; break;
            }
        }
    }

    public void RemoveItemStats(int itemType, int itemLevel, int itemGrade)
    {
        float effect = ItemManager.I.GetItemTypeEffect(itemType, itemLevel, itemGrade);

        switch (itemType)
        {
            case 0: towerStats.attackDamage -= effect; break;
            case 1: towerStats.attackSpeed -= effect; break;
            case 2: towerStats.attackRange -= effect; break;
            case 3: towerStats.criticalChance -= effect; break;
            case 4: towerStats.criticalDamage -= effect; break;
            case 5: towerStats.goldEarnAmount -= effect; break;
            case 6: towerStats.enemySlowAmount -= effect; break;
        }
    }

    private void Update()
    {
        if (towerStats == null || objectPool == null) return;

        attackTimer += Time.deltaTime;
        if (attackTimer >= 1f / towerStats.attackSpeed)
        {
            attackTimer = 0f;
            Attack();
        }

        // ���콺 ��Ŭ�� �� Ÿ���� Ŭ���ߴ��� Ȯ��
        if (Input.GetMouseButtonDown(0)) // ��Ŭ�� (0�� ���� Ŭ��)
        {
            HandleTowerSelection();
        }
    }

    private void HandleTowerSelection()
    {
        // ���콺 ��ġ�� �������� ���̸� ���, Ÿ���� Ŭ���� ��쿡�� ������ ǥ��
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Ŭ���� ������Ʈ�� Ÿ�����
            if (hit.collider != null && hit.collider.GetComponent<Tower>() != null)
            {
                Tower clickedTower = hit.collider.GetComponent<Tower>();

                // ���� ���õ� Ÿ���� �ְ�, �� Ÿ���� ���� Ŭ���� Ÿ���� �ٸ��� ���� �����
                if (currentSelectedTower != null && currentSelectedTower != clickedTower)
                {
                    currentSelectedTower.HideAttackRange();
                }

                // Ÿ�� ���� �����ֱ�
                clickedTower.ShowAttackRangeFor2Seconds();

                // ���� ���õ� Ÿ���� ����
                currentSelectedTower = clickedTower;
            }
        }
    }

    private void ShowAttackRangeFor2Seconds()
    {
        if (!isRangeVisible)
        {
            isRangeVisible = true;
            rangeIndicator.positionCount = 100; // ���� �׸��� ���� ����Ʈ ��
            float radius = towerStats.attackRange;

            // �� ������� ������ �׸���
            for (int i = 0; i < rangeIndicator.positionCount; i++)
            {
                float angle = i * Mathf.PI * 2f / rangeIndicator.positionCount;
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                rangeIndicator.SetPosition(i, new Vector3(transform.position.x + x, transform.position.y, transform.position.z + y));
            }

            // 2�� �Ŀ� ������ ����
            Invoke("HideAttackRange", 2f);
        }
    }

    private void HideAttackRange()
    {
        isRangeVisible = false;
        rangeIndicator.positionCount = 0;
    }

    private void Attack()
    {
        GameObject target = FindNearestMonster();
        if (target != null)
        {
            GameObject projectile = objectPool.GetProjectileFromPool(towerIndex);
            if (projectile == null)
            {
                Debug.LogError("Projectile Ǯ�� Ȯ�ο��");
                return;
            }

            projectile.transform.position = transform.position;
            Projectile projectileScript = projectile.GetComponent<Projectile>();

            if (projectileScript != null)
            {
                SoundManager.I.PlaySoundEffect(5);
                projectileScript.SetTarget(target.transform);
                projectileScript.speed = towerStats.projectileSpeed;
                projectileScript.SetTowerStats(towerStats);
            }
            else
            {
                Debug.LogError("Projectile ��ũ��Ʈ�� �Ҵ���� ����");
            }
        }
    }

    private GameObject FindNearestMonster()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        GameObject nearestMonster = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject monster in monsters)
        {
            float distance = Vector3.Distance(transform.position, monster.transform.position);

            if (distance < minDistance && distance <= towerStats.attackRange)
            {
                minDistance = distance;
                nearestMonster = monster;
            }
        }

        return nearestMonster;
    }
}
