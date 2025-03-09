using System;
using System.Collections.Generic;
using Raft.Scripts;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace Enemies
{
    public class EnemiesManager : MonoBehaviour
    {
        private Queue<Wave> wavesQueue = new();
        
        [SerializeField] private Transform playerTransform;
        [SerializeField] private BuildingsManager buildingsManager;

        [Space]
        [SerializeField] private List<Wave> waves = new();

        [Space]
        [SerializeField] private float minDistanceToSpawn;
        [SerializeField] private float maxDistanceToSpawn;

        [Space]
        public float currentDistanceFromStart;

        private void Awake()
        {
            foreach (var wave in waves)
                wavesQueue.Enqueue(wave);
        }

        private void Update()
        {
            SpawnWave();
        }

        private void SpawnWave()
        {
            if (wavesQueue.Count > 0 && currentDistanceFromStart > wavesQueue.Peek().position)
            {
                foreach (var enemyCount in wavesQueue.Dequeue().enemies)
                {
                    for (int i = 0; i < enemyCount.count; i++)
                    {
                        Quaternion spawnRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

                        Vector3 spawnPosition = playerTransform.position;
                        spawnPosition += spawnRotation * Vector3.forward *
                                         Random.Range(minDistanceToSpawn, maxDistanceToSpawn);
                        spawnPosition.y -= 5f; //Spawn under water
                        Debug.Log(Vector3.Distance(playerTransform.position, spawnPosition));
                        var enemyObject = Instantiate(enemyCount.enemyPrefab, spawnPosition, Quaternion.identity);

                        var enemy = enemyObject.GetComponent<Enemy>();
                        enemy.BuildingsManager = buildingsManager;
                    }
                }
            }
        }
    }
}