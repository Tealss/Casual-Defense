using System.Collections;
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
        }
    }

    public void ApplyMergeBonus()
    {
        towerStats.attackDamage *= 2.1f;
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

            projectile.transform.position = transform.position;

            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                SoundManager.I.PlaySoundEffect(5);
                projectileScript.SetTarget(target.transform);
                projectileScript.speed = towerStats.projectileSpeed;
                projectileScript.SetTowerTransform(transform, towerType);
                projectileScript.SetTowerStats(towerStats);

                // 타워에서 이펙트 관리
                SpawnHitEffect(target.transform.position);
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

    private void SpawnHitEffect(Vector3 position)
    {
        if (objectPool != null)
        {
            GameObject hitEffect = objectPool.GetHitEffectFromPool(towerIndex);  // 타워 인덱스를 통해 효과 가져오기
            if (hitEffect != null)
            {
                hitEffect.transform.position = position;
                hitEffect.SetActive(true);

                // 0.3초 후 이펙트를 비활성화하고 풀로 반환
                StartCoroutine(DeactivateEffectAfterDelay(hitEffect, 0.3f));
            }
        }
    }

    private IEnumerator DeactivateEffectAfterDelay(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        effect.SetActive(false);
        objectPool.ReturnHitEffectToPool(effect, towerIndex);
    }
}
