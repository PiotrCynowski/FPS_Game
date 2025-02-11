using UnityEngine;

namespace Bullets
{
    public class BulletStandard : Bullet
    {
        [SerializeField] private float speed = 20f;

        private void Update()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        protected override void OnHitTarget(GameObject _target)
        {
            IDamageable damageable = _target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            ReturnToPool();
        }
    }
}