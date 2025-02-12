using System.Collections;
using UnityEngine;

namespace Bullets
{
    public class BulletWithSplashDamage : Bullet
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float splashRadius = 2f;
        [SerializeField] private Renderer meshRenderer;
        [SerializeField] private ParticleSystem splashEffect;
        private bool isMoving;

        private void OnEnable()
        {
            isMoving = true;
            meshRenderer.enabled = true;
        }

        private void Start()
        {
            var main = splashEffect.main; 
            main.startSize = splashRadius;
        }

        private void Update()
        {
            if (isMoving)
            {
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            }
        }

        protected override void OnHitTarget(GameObject _target = null)
        {
            StartCoroutine(OnHitWithDelay());
        }

        private IEnumerator OnHitWithDelay()
        {
            isMoving = false;
            meshRenderer.enabled = false;

            splashEffect.gameObject.SetActive(true);
            splashEffect.Play();

            Collider[] hits = Physics.OverlapSphere(transform.position, splashRadius);
            foreach (Collider hit in hits)
            {
                IDamageable toDamage = hit.GetComponent<IDamageable>();
                toDamage?.TakeDamage(damage);
            }

            yield return new WaitForSeconds(1);
            ReturnToPool();
        }
    }
}