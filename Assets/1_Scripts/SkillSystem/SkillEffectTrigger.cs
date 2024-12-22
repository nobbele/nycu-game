using UnityEngine;

public class SkillEffectTrigger : MonoBehaviour
{
    private int damage;

    public void Initialize(int skillDamage)
    {
        damage = skillDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        var layer = LayerMask.LayerToName(other.gameObject.layer);
        if (layer == "Player") {
            return;
        }

        // Enemy
        var enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            onEnemyDamage(enemy);
            return;
        }

        // Boss
        var boss = other.GetComponent<Boss>();
        if (boss != null)
        {
            onBossDamage(boss);
            return;
        }
    }

    private void onEnemyDamage(Enemy enemy)
    {
        enemy.OnDamage(this.gameObject, damage);
    }

    private void onBossDamage(Boss boss)
    {
        boss.OnDamage(this.gameObject, damage);
    }
}
