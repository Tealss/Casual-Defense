using System.Collections;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private ObjectPool objectPool;

    private void Awake()
    {
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
            // Ensure we use the correct effect from the pool based on towerTypeIndex
            GameObject hitEffect = objectPool.GetHitEffectFromPool(towerTypeIndex);
            if (hitEffect != null)
            {
                hitEffect.transform.position = position;
                hitEffect.SetActive(true);

                // Return the effect to the pool after a delay
                StartCoroutine(ReturnEffectToPoolAfterDelay(hitEffect, towerTypeIndex, 0.5f));
            }
        }
    }

    private IEnumerator ReturnEffectToPoolAfterDelay(GameObject effect, int towerTypeIndex, float delay)
    {
        if (effect == null || !effect.activeInHierarchy)
        {
            yield break; // Exit if the effect is null or already inactive
        }

        yield return new WaitForSeconds(delay);

        effect.SetActive(false);

        // Return the effect to the correct pool using towerTypeIndex
        if (objectPool != null)
        {
            objectPool.ReturnHitEffectToPool(effect, towerTypeIndex);
        }
    }
}
