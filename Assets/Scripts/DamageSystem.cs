using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    public int damage = 10;
    public float damageRadius = 2f;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    public void ApplyDamage()
    {
        Vector3 origin = attackPoint ? attackPoint.position : transform.position;
        Collider[] hit = Physics.OverlapSphere(origin, damageRadius, enemyLayers);
        for (int i = 0; i < hit.Length; i++)
        {
            var health = hit[i].GetComponent<EnemyHealth>();
            if (health != null) health.TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 origin = attackPoint ? attackPoint.position : transform.position;
        Gizmos.DrawWireSphere(origin, damageRadius);
    }
}
