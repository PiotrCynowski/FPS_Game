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

        [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
        private List<Transform> emptyAreaSpawn = new List<Transform>();

        private SpawnWithPool<Enemy> poolSpawner;
        private System.Random rnd;

        private int numberOfEnemyTypes;
        private int nextSpawnerIndex;
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

            emptyAreaSpawn.AddRange(spawnPoints);

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
            int nextSpawnPoints;
            int limitSpawnBySpawnerNumbers = emptyAreaSpawn.Count;

            for (int i = 0; i < maxAllowedEnemiesAtOnce; i++)
            {
                if(i >= limitSpawnBySpawnerNumbers)
                {
                    break;
                }

                nextSpawnPoints = rnd.Next(0, emptyAreaSpawn.Count);

                poolSpawner.GetSpawnObject(emptyAreaSpawn[nextSpawnPoints], rnd.Next(0, numberOfEnemyTypes))
                     .arenaId = spawnPoints.IndexOf(emptyAreaSpawn[nextSpawnerIndex]);

                emptyAreaSpawn.Remove(emptyAreaSpawn[nextSpawnPoints]);

                numEnemies++;
            }

            StartCoroutine(SpawnEnemies());
        }

        private IEnumerator SpawnEnemies()
        {
            while (true)
            {
                if (numEnemies < maxAllowedEnemiesAtOnce && emptyAreaSpawn.Count !=0)
                {
                    SpawnEnemyInEmptyArea();  
                }
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        private void SpawnEnemyInEmptyArea()
        {
            nextSpawnerIndex = rnd.Next(0, emptyAreaSpawn.Count);

            poolSpawner.GetSpawnObject(emptyAreaSpawn[nextSpawnerIndex], rnd.Next(0, numberOfEnemyTypes))
                .arenaId = spawnPoints.IndexOf(emptyAreaSpawn[nextSpawnerIndex]);

            emptyAreaSpawn.Remove(emptyAreaSpawn[nextSpawnerIndex]);

            numEnemies++;
        }

        private void DecrementNumEnemies(int _arenaOfThatEnemy)
        {
            emptyAreaSpawn.Add(spawnPoints[_arenaOfThatEnemy]);

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
        }
#endif
    }
}