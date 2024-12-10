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

    private Animator animator;
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
            Debug.LogError("can't find the ObjectPool");
            return;
        }

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator is null.");
        }

        if (towerStats != null)
        {
            towerStats = Instantiate(towerStats);
        }
        else
        {
            Debug.LogError("TowerStats is null.");
        }

        towerStats.InitializeStats();

        if (ItemManager.I != null)
            ItemManager.I.OnItemStatsChanged += UpdateTowerStats;

        rangeIndicator = gameObject.AddComponent<LineRenderer>();
        rangeIndicator.positionCount = 0;
        rangeIndicator.startWidth = 0.1f;
        rangeIndicator.endWidth = 0.1f;
        rangeIndicator.material = new Material(Shader.Find("Sprites/Default"));
        rangeIndicator.startColor = Color.red;
        rangeIndicator.endColor = Color.green;

        attackTimer = 0f;
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

        GameObject nearestMonster = FindNearestMonster();
        if (nearestMonster != null)
        {
            float distanceToMonster = Vector3.Distance(transform.position, nearestMonster.transform.position);
            if (distanceToMonster <= towerStats.attackRange)
            {
                LookAtTarget(nearestMonster);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            HandleTowerSelection();
        }
    }

    public void InitializeStats()
    {
        if (towerStats != null)
        {
            towerStats.InitializeStats();
            ApplyItemStats();
        }
        else
        {
            Debug.LogError($"tower stats: {gameObject.name}");
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

            towerStats.InitializeBaseStats();
            towerStats.AddItemBonus(itemType, effect);
            towerStats.ApplyItemBonuses();

        }
    }

    public void RemoveItemStats(int itemType, int itemLevel, int itemGrade)
    {
        float effect = ItemManager.I.GetItemTypeEffect(itemType, itemLevel, itemGrade);

        towerStats.InitializeBaseStats();
        towerStats.RemoveItemBonus(itemType, effect);
        towerStats.ApplyItemBonuses();
    }

    public void ApplyMergeBonus(int newLevel)
    {
        level = newLevel;
        towerStats.level = newLevel;

        towerStats.attackDamage = towerStats.baseAttackDamage * Mathf.Pow(2.1f, newLevel - 1);
        towerStats.attackSpeed = towerStats.baseAttackSpeed + (newLevel - 1) * 0.05f;
        towerStats.attackRange = towerStats.baseAttackRange;
        towerStats.criticalChance = towerStats.baseCriticalChance;
        towerStats.criticalDamage = towerStats.baseCriticalDamage;
        towerStats.goldEarnAmount = towerStats.baseGoldEarnAmount;
        towerStats.enemySlowAmount = towerStats.baseEnemySlowAmount + (newLevel - 1) * 0.1f;


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
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            Invoke(nameof(ResetToIdleAnimation), 0.3f / towerStats.attackSpeed);
            this.towerIndex = GetTowerTypeIndexFromString(towerType);

            Invoke(nameof(DelayedFire), 0.15f);
        }
    }

    private void DelayedFire()
    {
        GameObject target = FindNearestMonster();
        if (target != null)
        {
            Fire(target.transform);
        }
    }


    public void Fire(Transform target)
    {
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
            //projectileScript.level = towerStats.level;
            //projectileScript.slowAmount = towerStats.enemySlowAmount;
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
            Debug.LogError("Projectile script is null");
        }
    }
    private void ResetToIdleAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Idle");
        }
    }

    private void HideAttackRange()
    {
        isRangeVisible = false;
        rangeIndicator.positionCount = 0;
    }

    private void LookAtTarget(GameObject target)
    {
        if (target == null) return;

        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }
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
