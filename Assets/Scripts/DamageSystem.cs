using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    public int damage;
    public float damageRadius = 2f;

    public void ApplyDamage()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, damageRadius);

        foreach (var enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<EnemyHealth>().TakeDamage(damage);
            }
        }
    }
}
