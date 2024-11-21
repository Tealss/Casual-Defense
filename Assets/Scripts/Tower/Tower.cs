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

    public List<Item> items;

    private LineRenderer rangeIndicator;
    private bool isRangeVisible = false;
    private static Tower currentSelectedTower = null;

    private void Start()
    {
        objectPool = FindObjectOfType<ObjectPool>();
        if (objectPool == null)
        {
            Debug.LogError("ObjectPool을 찾을 수 없음");
            return;
        }

        attackTimer = 0f;

        InitializeStats();

        if (ItemManager.I != null)
            ItemManager.I.OnItemStatsChanged += UpdateTowerStats;

        rangeIndicator = gameObject.AddComponent<LineRenderer>();
        rangeIndicator.positionCount = 0;
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
            towerStats.ResetStats();
            ApplyItemStats();
        }
        else
        {
            Debug.LogError($"타워 스탯 초기화 실패: {gameObject.name}");
        }
    }

    private void OnDestroy()
    {
        // 이벤트 등록 해제
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

        if (Input.GetMouseButtonDown(0)) 
        {
            HandleTowerSelection();
        }
    }

    private void HandleTowerSelection()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.GetComponent<Tower>() != null)
            {
                Tower clickedTower = hit.collider.GetComponent<Tower>();

                if (currentSelectedTower != null && currentSelectedTower != clickedTower)
                {
                    currentSelectedTower.HideAttackRange();
                }


                clickedTower.ShowAttackRangeFor2Seconds();
                currentSelectedTower = clickedTower;

                //Debug.Log($"Selected Tower Stats - {clickedTower.name}:");
                //Debug.Log($"Attack Damage: {clickedTower.towerStats.attackDamage}");
                //Debug.Log($"Attack Speed: {clickedTower.towerStats.attackSpeed}");
                //Debug.Log($"Attack Range: {clickedTower.towerStats.attackRange}");
                //Debug.Log($"Critical Chance: {clickedTower.towerStats.criticalChance}");
                //Debug.Log($"Critical Damage: {clickedTower.towerStats.criticalDamage}");
                //Debug.Log($"Gold Earn Amount: {clickedTower.towerStats.goldEarnAmount}");
                //Debug.Log($"Enemy Slow Amount: {clickedTower.towerStats.enemySlowAmount}");
            }
        }
    }

    private void ShowAttackRangeFor2Seconds()
    {
        if (!isRangeVisible)
        {
            isRangeVisible = true;
            rangeIndicator.positionCount = 100;
            float radius = towerStats.attackRange;

            for (int i = 0; i < rangeIndicator.positionCount; i++)
            {
                float angle = i * Mathf.PI * 2f / rangeIndicator.positionCount;
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                rangeIndicator.SetPosition(i, new Vector3(transform.position.x + x, transform.position.y, transform.position.z + y));
            }

            Invoke("HideAttackRange", 1f);
        }
    }
    private void Attack()
    {
        GameObject target = FindNearestMonster();
        if (target != null)
        {
            GameObject projectile = objectPool.GetProjectileFromPool(towerIndex);
            if (projectile == null)
            {
                Debug.LogError("Projectile 풀을 확인요망");
                return;
            }
            GameObject projectilesFolder = GameObject.Find("ObjectPool");
            if (projectilesFolder == null)
            {
                projectilesFolder = new GameObject("ObjectPool");
            }
            projectile.transform.SetParent(projectilesFolder.transform);

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
                Debug.LogError("Projectile 스크립트가 할당되지 않음");
            }
        }
    }


    private void HideAttackRange()
    {
        isRangeVisible = false;
        rangeIndicator.positionCount = 0;
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
