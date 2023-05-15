using System;
using UnityEngine;
using PoolSpawner;
using System.Collections;

namespace Bullets
{
    public abstract class Bullet : MonoBehaviour, IPoolable<Bullet>
    {
        public int damage;
        [SerializeField] private int lifeTime;
        [SerializeField] private LayerMask targetLayer;

        private Coroutine returnCoroutine;

        private Action<Bullet, int> returnToPool;
        private int id;


        protected virtual void OnTriggerEnter(Collider _other)
        {
            if (targetLayer == (targetLayer | (1 << _other.gameObject.layer)))
            {
                OnHitTarget(_other.gameObject);
            }
        }

        protected abstract void OnHitTarget(GameObject _target);


        #region pool
        public void Initialize(Action<Bullet, int> _returnAction, int _id)
        {
            this.returnToPool = _returnAction;
            id = _id;
        }

        public void ReturnToPool()
        {
            returnToPool?.Invoke(this, id);
        }

        private IEnumerator ReturnToPoolAfterDelay()
        {
            yield return new WaitForSeconds(lifeTime);
            ReturnToPool();
        }
        #endregion


        #region enable/disable
        private void OnEnable()
        {
            if (returnCoroutine != null)
            {
                StopCoroutine(returnCoroutine);
            }
            returnCoroutine = StartCoroutine(ReturnToPoolAfterDelay());
        }

        private void OnDisable()
        {
            if (returnCoroutine != null)
            {
                StopCoroutine(returnCoroutine);
            }
        }
        #endregion
    }
}

public interface IDamageable
{
    void TakeDamage(int _amount);
}