using System.Collections;
using UnityEngine;
using PoolSpawner;
using System.Collections.Generic;
using System;

namespace Enemies {
    public class EnemySpawner : MonoBehaviour
    {
        [Range(1, 8)]
        [SerializeField] private int maxAllowedEnemiesAtOnce;

        [SerializeField] private float spawnInterval = 2.0f;

        [SerializeField] private Enemy[] toSpawn;

        [SerializeField] private List<Transform> spawnPoints = new();
        private Queue<int> emptySpawnPointsIndex = new();

        private SpawnWithPool<Enemy> poolSpawner;
        private System.Random rnd;

        private int numberOfEnemyTypes;
        private int numEnemies = 0;

        private void OnEnable()
        {
            Enemy.onArenaClear += DecrementNumEnemies;
        }

        private void Start()
        {
            poolSpawner = new SpawnWithPool<Enemy>();
            rnd = new System.Random();

            numberOfEnemyTypes = toSpawn.Length;

            for (int i = 0; i < spawnPoints.Count; i++)
            {
                emptySpawnPointsIndex.Enqueue(i);
            }

            for (int i = 0; i < numberOfEnemyTypes; i++)
            {
                poolSpawner.AddPoolForGameObject(toSpawn[i].gameObject, i);
            }

            StartSpawn();
        }

        private void OnDisable()
        {
            Enemy.onArenaClear -= DecrementNumEnemies;
        }

        private void StartSpawn()
        {
            for (int i = 0; i < maxAllowedEnemiesAtOnce; i++)
            {
                if(i >= spawnPoints.Count)
                {
                    break;
                }

                SpawnEnemyInEmptyArea();
            }
        }

        private IEnumerator SpawnEnemies()
        {
            yield return new WaitForSeconds(spawnInterval);
            if (numEnemies < maxAllowedEnemiesAtOnce && emptySpawnPointsIndex.Count != 0)
            {
                SpawnEnemyInEmptyArea();
            }
        }

        private void SpawnEnemyInEmptyArea()
        {
            if (emptySpawnPointsIndex.Count == 0)
                return;

            int nextSpawnerIndex = emptySpawnPointsIndex.Dequeue();

            poolSpawner.GetSpawnObject(spawnPoints[nextSpawnerIndex], rnd.Next(0, numberOfEnemyTypes))
                .arenaId = spawnPoints.IndexOf(spawnPoints[nextSpawnerIndex]);

            numEnemies++;
        }

        private void DecrementNumEnemies(int _arenaOfThatEnemy)
        {
            emptySpawnPointsIndex.Enqueue(_arenaOfThatEnemy);
            StartCoroutine(SpawnEnemies());
            numEnemies--;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(spawnPoints[i].position, 1);
            }

            foreach (int index in emptySpawnPointsIndex)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(spawnPoints[index].position, Vector3.one);
            }
        }
#endif
    }
}