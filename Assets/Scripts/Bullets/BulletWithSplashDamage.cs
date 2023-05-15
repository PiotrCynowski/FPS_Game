using UnityEngine;

namespace Bullets
{
    public class BulletWithSplashDamage : Bullet
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float splashRadius = 2f;

        protected override void OnHitTarget(GameObject _target = null)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, splashRadius);
            foreach (Collider hit in hits)
            {
                IDamageable toDamage = hit.GetComponent<IDamageable>();
                if (toDamage != null)
                {
                    toDamage.TakeDamage(damage);
                }
            }
            ReturnToPool();
        }

        private void Update()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }
}