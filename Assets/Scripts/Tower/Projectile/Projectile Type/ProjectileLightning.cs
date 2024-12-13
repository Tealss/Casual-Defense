using UnityEngine;

public class ProjectileLightning : IProjectileBehavior
{
    //private static Shader lightningShader;

    public void Execute(Projectile projectile, Transform target)
    {
        int remainingChains = projectile.maxChainHits;
        Transform currentTarget = target;

        //Vector3 firePointPosition = GetFirePointPosition(projectile);

        //LoadLightningShader();

        while (remainingChains > 0 && currentTarget != null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(currentTarget.position, projectile.range);
            currentTarget = null;

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Monster") || hitCollider.CompareTag("Bounty") || hitCollider.CompareTag("Boss"))
                {
                    Monster monster = hitCollider.GetComponent<Monster>();
                    if (monster != null && !projectile.previousTargets.Contains(monster))
                    {
                        float chainDamage = projectile.damage * 0.25f;

                        float randomChance = Random.Range(0f, 100f);
                        bool isCriticalHit = randomChance <= projectile.CriticalChance;
                        float finalDamage = isCriticalHit ? chainDamage * projectile.CriticalDamage : chainDamage;
                        monster.TakeDamage(finalDamage);

                        //CreateLightningEffect(firePointPosition, monster.transform.position);

                        EffectManager.I.SpawnHitEffect(2, monster.transform.position);
                        SoundManager.I.PlaySoundEffect(12);

                        Vector3 spawnPosition = monster.transform.position + new Vector3(0.5f, 1f, 0);
                        string damageText = isCriticalHit ? $"- {(int)finalDamage}!" : $"- {(int)finalDamage}";
                        Color textColor = isCriticalHit ? Color.red : Color.white;

                        FadeOutTextUse.I.SpawnFadeOutText(spawnPosition, damageText, textColor);

                        projectile.previousTargets.Add(monster);
                        currentTarget = monster.transform;
                        break;
                    }
                }
            }

            remainingChains--;
        }
    }

    //private void LoadLightningShader()
    //{
    //    if (lightningShader == null)
    //    {
    //        lightningShader = Resources.Load<Shader>("PathToYourShader"); // Replace with the correct path
    //        if (lightningShader == null)
    //        {
    //            Debug.LogError("Lightning shader could not be loaded. Ensure it's in the Resources folder.");
    //        }
    //    }
    //}

    //private Vector3 GetFirePointPosition(Projectile projectile)
    //{
    //    if (projectile.towerTransform != null)
    //    {
    //        Transform firePoint = projectile.towerTransform.Find("FirePoint");
    //        if (firePoint != null)
    //        {
    //            return firePoint.position;
    //        }
    //    }

    //    return projectile.towerTransform != null
    //        ? projectile.towerTransform.position + new Vector3(0, 0.7f, 0)
    //        : Vector3.zero;
    //}

    //private void CreateLightningEffect(Vector3 start, Vector3 end)
    //{
    //    if (lightningShader == null)
    //    {
    //        Debug.LogError("Lightning shader is not loaded. Call LoadLightningShader first.");
    //        return;
    //    }

    //    GameObject lightning = new GameObject("LightningEffect");
    //    LineRenderer lineRenderer = lightning.AddComponent<LineRenderer>();

    //    lineRenderer.positionCount = 5;
    //    lineRenderer.SetPosition(0, start);

    //    for (int i = 1; i < 4; i++)
    //    {
    //        float t = i / 4.0f;

    //        Vector3 randomOffset = new Vector3(
    //            Random.Range(-0.4f, 0.5f),
    //            Random.Range(-0.4f, 0.5f),
    //            Random.Range(-0.3f, 0.3f)
    //        );

    //        lineRenderer.SetPosition(i, Vector3.Lerp(start, end, t) + randomOffset);
    //    }

    //    lineRenderer.SetPosition(4, end);

    //    lineRenderer.startWidth = 0.070f;
    //    lineRenderer.endWidth = 0.025f;

    //    Material lightningMaterial = new Material(lightningShader);
    //    lineRenderer.material = lightningMaterial;
    //    lineRenderer.startColor = Color.cyan;
    //    lineRenderer.endColor = Color.blue;

    //    CreateGradientLights(start, end);

    //    Object.Destroy(lightning, 0.2f);
    //}

    //private void CreateGradientLights(Vector3 start, Vector3 end)
    //{
    //    int numLights = 5;

    //    for (int i = 0; i < numLights; i++)
    //    {
    //        float lerpFactor = (float)i / (numLights - 1);
    //        Vector3 position = Vector3.Lerp(start, end, lerpFactor);

    //        GameObject lightObj = new GameObject("GradientLight_" + i);
    //        Light light = lightObj.AddComponent<Light>();
    //        light.color = Color.Lerp(Color.cyan, Color.blue, lerpFactor);
    //        light.intensity = Mathf.Lerp(2.2f, 0.8f, lerpFactor);
    //        light.range = Mathf.Lerp(1.2f, 0.8f, lerpFactor);
    //        light.shadows = LightShadows.None;
    //        light.transform.position = position;

    //        Object.Destroy(lightObj, 0.2f);
    //    }
    //}
}
