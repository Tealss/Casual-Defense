using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Tower : MonoBehaviour
{
    public TowerStats towerStats;
    public int level;
    public int towerIndex;
    public string towerType;
    public List<Item> items;

    private float attackTimer;
    private ObjectPool objectPool;
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
        }
    }

    public void ApplyMergeBonus()
    {
        towerStats.attackDamage *= 2.1f;
        towerStats.level += 1;
        switch (towerType)
        {
            case "Tower_0":
                break;
            case "Tower_1":
                break;
            case "Tower_2":
                Debug.Log("T3");
                break;
            case "Tower_3)":
                towerStats.enemySlowAmount += 0.1f;
                break;
            case "Tower_4":
                break;
            case "Tower_5":
                break;
            case "Tower_6":
                break;
            default:
                break;
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

    private int GetTowerTypeIndexFromString(string towerType)
    {
        switch (towerType)
        {
            case "Tower_0": return 0;
            case "Tower_1": return 1;
            case "Tower_2": return 2;
            case "Tower_3": return 3;
            case "Tower_4": return 4;
            case "Tower_5": return 5;
            case "Tower_6": return 6;
            default:
                return -1;
        }
    }

    private void Attack()
    {
        GameObject target = FindNearestMonster();
        if (target != null)
        {
            this.towerIndex = GetTowerTypeIndexFromString(towerType);
            Fire(target.transform);
        }
    }

    public void Fire(Transform target)
    {
        //Debug.Log($"Tower Type: {towerType}, Projectile Type Index: {towerIndex}");

        GameObject projectile = objectPool.GetFromPool($"Projectile_{towerIndex}", objectPool.projectilePrefabs[towerIndex]);
        if (projectile == null)
        {
            Debug.LogError("Check the projectile pool");
            return;
        }

        int projectileTypeIndex = towerIndex;

        //projectile.transform.SetParent(Folder.folder.transform, false);
        projectile.transform.position = transform.position;

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize();
            projectileScript.SetTarget(target);
            projectileScript.goldEarn = towerStats.goldEarnAmount;
            projectileScript.speed = towerStats.projectileSpeed;
            projectileScript.SetTowerTransform(transform, projectileTypeIndex);
            projectileScript.SetTowerStats(towerStats);

            switch (towerType)
            {
                case "Tower_0":
                    projectileScript.SetBehavior(new ProjectileBasic());
                    break;
                case "Tower_1":
                    projectileScript.SetBehavior(new ProjectileExplosive());
                    break;
                case "Tower_2":
                    projectileScript.SetBehavior(new ProjectileLightning());
                    break;
                case "Tower_3":
                    projectileScript.SetBehavior(new ProjectileIce());
                    break;
                case "Tower_4":
                    projectileScript.SetBehavior(new ProjectileRandom());
                    break;
                case "Tower_5":
                    projectileScript.SetBehavior(new ProjectileGold());
                    break;
                case "Tower_6":
                    projectileScript.SetBehavior(new ProjectileBoss());
                    break;
                default:
                    projectileScript.SetBehavior(new ProjectileBasic());
                    break;
            }
        }
        else
        {
            Debug.LogError("Projectile 스크립트가 할당되지 않음");
        }
    }

    private void HideAttackRange()
    {
        isRangeVisible = false;
        rangeIndicator.positionCount = 0;
    }

    private GameObject FindNearestMonster()
    {
        GameObject[] monsters = CombineArrays(
            GameObject.FindGameObjectsWithTag("Monster"),
            GameObject.FindGameObjectsWithTag("Bounty"),
            GameObject.FindGameObjectsWithTag("Boss")
        );

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

    private GameObject[] CombineArrays(GameObject[] array1, GameObject[] array2, GameObject[] array3)
    {
        List<GameObject> combinedList = new List<GameObject>();
        combinedList.AddRange(array1);
        combinedList.AddRange(array2);
        combinedList.AddRange(array3);
        return combinedList.ToArray();
    }
}
