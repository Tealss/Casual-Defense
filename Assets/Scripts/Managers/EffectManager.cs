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
            GameObject hitEffect = objectPool.GetHitEffectFromPool(towerTypeIndex);
            if (hitEffect != null)
            {
                hitEffect.transform.SetParent(Folder.folder.transform, false);
                hitEffect.transform.position = position;
                hitEffect.SetActive(true);

                StartCoroutine(ReturnEffectToPoolAfterDelay(hitEffect, towerTypeIndex, 0.5f));
            }
        }
    }

    private IEnumerator ReturnEffectToPoolAfterDelay(GameObject effect, int towerTypeIndex, float delay)
    {
        if (effect == null || !effect.activeInHierarchy)
        {
            yield break;
        }

        yield return new WaitForSeconds(delay);

        effect.SetActive(false);

        if (objectPool != null)
        {
            objectPool.ReturnHitEffectToPool(effect, towerTypeIndex);
        }
    }

}
