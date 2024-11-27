using UnityEngine;
using static UnityEngine.Rendering.HighDefinition.ScalableSettingLevelParameter;

public class ProjectileGold: IProjectileBehavior
{
    public void Execute(Projectile projectile, Transform target)
    {
        if (target != null && target.CompareTag("Monster"))
        {
            Monster monster = target.GetComponent<Monster>();
            if (monster != null)
            {
               
                float damage = GameManager.I.gold * 0.05f + projectile.damage * 0.05f;
                damage *= projectile.level + 1;

                Debug.Log($"{projectile.level} , {damage}");
                float randomChance = Random.Range(0f, 100f);
                bool isCriticalHit = randomChance <= projectile.CriticalChance;

                float finalDamage = isCriticalHit ? damage * projectile.CriticalDamage : damage;

                monster.TakeDamage(finalDamage);

                EffectManager.I.SpawnHitEffect(5, target.position);

                Vector3 spawnPosition = target.position + new Vector3(0.6f, 0.7f, 0);
                string damageText = isCriticalHit ? $"- {(int)finalDamage}!" : $"- {(int)finalDamage}";
                Color textColor = isCriticalHit ? Color.red : Color.white;

                FadeOutTextUse.I.SpawnFadeOutText(spawnPosition, damageText, textColor);
            }
        }
    }
}
