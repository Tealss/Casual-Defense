using UnityEngine;

public class Tower : MonoBehaviour
{
    public TowerStats towerStats;
    public ItemStats itemStats;

    public int level;
    public int towerIndex;
    public string towerType;
    public Transform firePoint;

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
            towerStats.InitializeBaseStats();
        }
        else
        {
            Debug.LogError("TowerStats is null.");
        }
        if (itemStats != null)
        {
            itemStats.InitializeBaseStats();
        }
        else
        {
            Debug.LogError("itemStats is null.");
        }

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
                    towerStats.InitializeBaseStats();
        GameObject nearestMonster = FindNearestMonster();
        if (nearestMonster != null)
        {
            float distanceToMonster = Vector3.Distance(transform.position, nearestMonster.transform.position);
            if (distanceToMonster <= towerStats.attackRange + itemStats.itemAttackRange)
            {
                LookAtTarget(nearestMonster);

                attackTimer += Time.deltaTime;
                if (attackTimer >= 1f / towerStats.attackSpeed)
                {
                    attackTimer = 0f;
                    Attack();
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            HandleTowerSelection();
        }
    }

    public void itemEffect() 
    { 

    }

    public void InitializeStats()
    {
        if (towerStats != null)
        {
            towerStats.InitializeBaseStats();
        }
        else
        {
            Debug.LogError($"tower stats: {gameObject.name}");
        }
    }
    private void UpdateTowerStats()
    {
        InitializeStats();
    }

    public void ApplyMergeBonus(int newLevel)
    {
        level = newLevel;
        towerStats.level = newLevel;

        towerStats.attackDamage = towerStats.baseAttackDamage * Mathf.Pow(2.1f, newLevel - 1);
        towerStats.attackSpeed = towerStats.baseAttackSpeed + (newLevel - 1) * 0.05f;
        towerStats.attackRange = towerStats.baseAttackRange + (newLevel - 1) * 0.05f;
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
            float radius = towerStats.attackRange + itemStats.itemAttackRange;

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

            Invoke(nameof(ResetToIdleAnimation), 0.3f / towerStats.attackSpeed + itemStats.itemAttackSpeed);
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
        projectile.transform.position = firePoint.position;

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize();
            projectileScript.SetTarget(target);
            projectileScript.goldEarn = towerStats.goldEarnAmount;
            projectileScript.projectileSpeed = towerStats.projectileSpeed;
            projectileScript.SetTowerTransform(transform, projectileTypeIndex);
            projectileScript.SetTowerStats(towerStats,itemStats);

            //Debug.Log($"{towerStats.itemAttackDamageBonus}");

            switch (towerType)
            {
                case "Tower_0":
                    projectileScript.SetBehavior(new ProjectileBasic());
                    break;
                case "Tower_1":
                    projectileScript.SetBehavior(new ProjectileExplosive());
                    EffectManager.I.SpawnAttackEffect(1, transform.position);
                    break;
                case "Tower_2":
                    projectileScript.SetBehavior(new ProjectileLightning());
                    EffectManager.I.SpawnAttackEffect(2, transform.position);
                    break;
                case "Tower_3":
                    projectileScript.SetBehavior(new ProjectileIce());
                    EffectManager.I.SpawnAttackEffect(3, transform.position);
                    break;
                case "Tower_4":
                    projectileScript.SetBehavior(new ProjectileRandom());
                    EffectManager.I.SpawnAttackEffect(4, transform.position);
                    break;
                case "Tower_5":
                    projectileScript.SetBehavior(new ProjectileGold());
                    EffectManager.I.SpawnAttackEffect(5, transform.position);
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

    private GameObject FindNearestMonster()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        GameObject[] bountyMonsters = GameObject.FindGameObjectsWithTag("Bounty");
        GameObject[] bossMonsters = GameObject.FindGameObjectsWithTag("Boss");

        GameObject[] allUnits = new GameObject[monsters.Length + bountyMonsters.Length + bossMonsters.Length];
        monsters.CopyTo(allUnits, 0);
        bountyMonsters.CopyTo(allUnits, monsters.Length);
        bossMonsters.CopyTo(allUnits, monsters.Length + bountyMonsters.Length);

        GameObject nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject unit in allUnits)
        {
            float distance = Vector3.Distance(transform.position, unit.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = unit;
            }
        }

        return nearest;
    }

    private void LookAtTarget(GameObject target)
    {
        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }
}
