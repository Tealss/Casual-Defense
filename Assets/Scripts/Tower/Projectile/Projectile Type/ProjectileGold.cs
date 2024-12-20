using UnityEngine;
using static UnityEngine.Rendering.HighDefinition.ScalableSettingLevelParameter;

public class ProjectileGold: IProjectileBehavior
{
    public void Execute(Projectile projectile, Transform target)
    {
        if (target != null && (target.CompareTag("Monster") || target.CompareTag("Bounty") || target.CompareTag("Boss")))
        {
            Monster monster = target.GetComponent<Monster>();
            if (monster != null)
            {
                float basicDamage = projectile.damage * 0.05f;
                float damage = projectile.goldEarn + basicDamage;
                damage *= projectile.level + 1;

                //Debug.Log($"{projectile.level} , {damage}, {projectile.goldEarn}");
                float randomChance = Random.Range(0f, 100f);
                bool isCriticalHit = randomChance <= projectile.CriticalChance;
                float finalDamage = isCriticalHit ? damage * projectile.CriticalDamage : damage;


                monster.TakeDamage(finalDamage);
                GameManager.I.AddGold((int)finalDamage);

                EffectManager.I.SpawnHitEffect(5, target.position);
                SoundManager.I.PlaySoundEffect(5);

                Vector3 spawnPosition = target.position + new Vector3(0.6f, 0.7f, 0);
                string damageText = isCriticalHit ? $"+ {(int)finalDamage}!" : $"+ {(int)finalDamage}";
                Color textColor = isCriticalHit ? Color.cyan : Color.yellow;

                FadeOutTextUse.I.SpawnFadeOutText(spawnPosition, damageText, textColor);

                //Debug.Log($" CP/{projectile.CriticalChance} CD/{projectile.CriticalDamage} R/{projectile.range} G/{projectile.goldEarn} S/{projectile.speed}");
            }
        }
    }
}
