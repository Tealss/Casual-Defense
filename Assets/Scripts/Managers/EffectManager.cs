using System.Collections;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private ObjectPool objectPool;
    public static EffectManager I;

    private void Awake()
    {
        if (I == null)
            I = this;
        else
            Destroy(gameObject);

        objectPool = FindObjectOfType<ObjectPool>();
        if (objectPool == null)
        {
            Debug.LogError("ObjectPool not found!");
        }
    }

    public void SpawnHitEffect(int towerTypeIndex, Vector3 position)
    {
        if (objectPool != null)
        {
            string poolName = $"HitEffect_{towerTypeIndex}";
            GameObject hitEffect = objectPool.GetFromPool(poolName, objectPool.hitEftPrefabs[towerTypeIndex]);
            if (hitEffect != null)
            {
                hitEffect.transform.SetParent(Folder.folder.transform, false);
                hitEffect.transform.position = position;
                hitEffect.SetActive(true);

                StartCoroutine(ReturnEffectToPoolAfterDelay(hitEffect, poolName, 0.5f));
            }
        }
    }

    public void SpawnAttackEffect(int towerTypeIndex, Vector3 position)
    {
        if (objectPool != null)
        {
            string poolName = $"attackEffect_{towerTypeIndex}";
            GameObject attackEffect = objectPool.GetFromPool(poolName, objectPool.attackEftPrefabs[towerTypeIndex]);
            if (attackEffect != null)
            {
                if (towerTypeIndex == 2 || towerTypeIndex == 4 || towerTypeIndex == 5)
                {
                    position.y += 0.5f;
                }

                attackEffect.transform.SetParent(Folder.folder.transform, false);
                attackEffect.transform.position = position;
                attackEffect.SetActive(true);

                StartCoroutine(ReturnEffectToPoolAfterDelay(attackEffect, poolName, 0.5f));
            }
        }
    }

    private IEnumerator ReturnEffectToPoolAfterDelay(GameObject effect, string poolName, float delay)
    {
        if (effect == null || !effect.activeInHierarchy)
        {
            yield break;
        }

        yield return new WaitForSeconds(delay);

        effect.SetActive(false);

        if (objectPool != null)
        {
            objectPool.ReturnToPool(poolName, effect);
        }
    }
}
