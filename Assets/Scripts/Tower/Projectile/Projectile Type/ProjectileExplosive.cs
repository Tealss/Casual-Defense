using UnityEngine;

public class ProjectileExplosive : IProjectileBehavior
{
    public void Execute(Projectile projectile, Transform target)
    {
        Collider[] hitColliders = Physics.OverlapSphere(target.position, 2f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Monster"))
            {
                Monster monster = hitCollider.GetComponent<Monster>();
                if (monster != null)
                {
                    float explosiveDamage = projectile.damage * 0.3f;
                    monster.TakeDamage(explosiveDamage);

                    EffectManager.I.SpawnHitEffect(1, target.position);

                    Vector3 spawnPosition = monster.transform.position + new Vector3(0.6f, 0.7f, 0);
                    string damageText = $"-{(int)explosiveDamage}";

                    Color textColor = Color.white;

                    FadeOutTextUse.I.SpawnFadeOutText(spawnPosition, damageText, textColor);
                }
            }
        }
        Debug.Log("Explosion damage applied to nearby targets!");
    }
}
