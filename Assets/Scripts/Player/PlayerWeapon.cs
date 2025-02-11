using UnityEngine;
using PoolSpawner;
using Bullets;
using System.Collections;

namespace Player
{
    public class PlayerWeapon : MonoBehaviour
    {
        [SerializeField] private Bullet fastBulletPrefab;
        [SerializeField] private Bullet slowBulletPrefab;

        [SerializeField] private Transform gunTransform;

        private SpawnWithPool<Bullet> poolSpawner;

        private bool isPowerupHighDamageActive;
        private float weaponBoostDuration;
        private Coroutine weaponBoost;

        private void OnEnable()
        {
            PlayerInteractions.onWeaponBoost += EnablePowerup;
        }
        private void Start()
        {
            poolSpawner = new SpawnWithPool<Bullet>();
            poolSpawner.AddPoolForGameObject(fastBulletPrefab.gameObject, 1);
            poolSpawner.AddPoolForGameObject(slowBulletPrefab.gameObject, 2);
        }

        private void OnDisable()
        {
            PlayerInteractions.onWeaponBoost -= EnablePowerup;
        }

        public void ShotLeftMouseButton()
        {
            ShootFastBullet();
        }

        public void ShotRightMouseButton()
        {
            ShootSlowBullet();
        }

        private void ShootFastBullet()
        {
            if (isPowerupHighDamageActive)
            {
                poolSpawner.GetSpawnObject(gunTransform, 1).damage *= 2;
                return;
            }

            poolSpawner.Spawn(gunTransform, 1);
        }

        private void ShootSlowBullet()
        {
            if (isPowerupHighDamageActive)
            {
                poolSpawner.GetSpawnObject(gunTransform, 2).damage *= 2;
                return;
            }

            poolSpawner.Spawn(gunTransform, 2);
        }

        #region PowerUp
        private void EnablePowerup(int _addTimeToPuDuration)
        {
            weaponBoostDuration += _addTimeToPuDuration;

            if (weaponBoost == null)
            {
                weaponBoost = StartCoroutine(WeaponBoostRoutine());
            }
        }

        private IEnumerator WeaponBoostRoutine()
        {
            isPowerupHighDamageActive = true;

            while (weaponBoostDuration > 0)
            {
                weaponBoostDuration -= Time.deltaTime;
                yield return null;
            }

            isPowerupHighDamageActive = false;
        }
        #endregion
    }
}