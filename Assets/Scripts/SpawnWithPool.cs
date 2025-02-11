using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace PoolSpawner
{
    public class SpawnWithPool<T> where T : Component
    {
        private readonly bool collectionChecks = true;
        private readonly int maxPoolSize = 10;

        private Transform elementsContainer;
        private Transform startPosition;

        private readonly Dictionary<int, ObjectPool<T>> poolObjList = new();

        public void AddPoolForGameObject(GameObject _toSpawn, int _id)
        {
            if (elementsContainer == null)
            {
                elementsContainer = new GameObject().GetComponent<Transform>();
                elementsContainer.name = "PoolElementsContainer";
            }

            ObjectPool<T> pool = new(() =>
            {
                var obj = GameObject.Instantiate(_toSpawn, Vector3.zero, Quaternion.identity, elementsContainer);

                if (obj.GetComponent<IPoolable<T>>() != null)
                {
                    obj.GetComponent<IPoolable<T>>().Initialize(ThisObjReleased, _id);
                    return obj.GetComponent<T>();
                }

                return null;
            },
            OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);

            poolObjList.Add(_id, pool);
        }

        public void Spawn(Transform _pos, int _id)
        {
            startPosition = _pos;
            poolObjList[_id].Get();
        }

        public T GetSpawnObject(Transform _pos, int _id)
        {
            startPosition = _pos;
            return poolObjList[_id].Get();
        }

        private void ThisObjReleased(T _obj, int _id)
        {
            poolObjList[_id].Release(_obj);
        }

        #region poolOperations
        private void OnReturnedToPool(T system)
        {         
            system.gameObject.SetActive(false);
        }

        private void OnTakeFromPool(T system)
        {
            system.transform.SetPositionAndRotation(startPosition.position, startPosition.rotation);
            system.gameObject.SetActive(true);
        }

        private void OnDestroyPoolObject(T system)
        {
            GameObject.Destroy(system.gameObject);
        }
        #endregion  
    }

    public interface IPoolable<T> where T : Component
    {
        void Initialize(System.Action<T, int> _returnAction, int _id);
        void ReturnToPool();
    }
}