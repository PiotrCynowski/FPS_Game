using PoolSpawner;
using System;
using UnityEngine;

namespace Enemies
{
    public abstract class Enemy : MonoBehaviour, IDamageable, IPoolable<Enemy>
    {
        public int MaxHealth;
        private int currentHealth;
        private Renderer rend;
        private MaterialPropertyBlock propertyBlock;

        [HideInInspector] public Vector3 startArenaPos;
        [HideInInspector] public Vector3 targetPosition;

        private Action<Enemy, int> onReturnToPool;
        public static Action<int> onArenaClear;

        [HideInInspector] private int id;
        [HideInInspector] public int arenaId;

        public virtual void OnEnable()
        {
            startArenaPos = transform.position;
            targetPosition = startArenaPos;

            currentHealth = MaxHealth;
            UpdateColor();
        }

        private void Awake()
        {
            rend = GetComponent<Renderer>();
            propertyBlock = new MaterialPropertyBlock();
        }

        private void Start()
        {
            startArenaPos = transform.position;
            targetPosition = startArenaPos;

            UpdateColor();      
        }

        public void TakeDamage(int _damage)
        {
            currentHealth -= _damage;
            UpdateColor();

            if (currentHealth <= 0)
            {
                ReturnToPool();
            }
        }

        #region pool
        public void Initialize(Action<Enemy, int> _returnAction, int _id)
        {
            this.onReturnToPool = _returnAction;
            id = _id;
        }

        public void ReturnToPool()
        {
            onArenaClear?.Invoke(arenaId);
            onReturnToPool?.Invoke(this, id);
        }
        #endregion


        private void UpdateColor()
        {
            float healthPercentage = (float)currentHealth / MaxHealth;

            if (healthPercentage >= 0.5f)
            {
                propertyBlock.SetColor("_Color", Color.Lerp(Color.yellow, Color.green, (healthPercentage - 0.5f) * 2f));
            }
            else
            {
                propertyBlock.SetColor("_Color", Color.Lerp(Color.red, Color.yellow, healthPercentage * 2f));
            }

            rend.SetPropertyBlock(propertyBlock);
        }
    }
}