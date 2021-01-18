using System.Collections;
using System.Collections.Generic;
using Interactivity.Components.Gameplay;
using UnityEngine;

namespace Interactivity
{
    public class EnemySpawner : MonoBehaviour
    {
        public bool repeatSpawn;
        public float spawnDelay = 0.25f;
        public bool displaySpawnRadius = true;
        public int spawnRadius = 1;
        public List<EnemySpawnerData> data;
        private Coroutine _currentSpawner;
        
        public void ActivateSpawner()
        {
            if (_currentSpawner != null)
                StopCoroutine(_currentSpawner);
            _currentSpawner = StartCoroutine(SpawnEnemies());
        }

        public void DisableSpawner()
        {
            StopCoroutine(_currentSpawner);
        }

        private IEnumerator SpawnEnemies()
        {
            foreach (var spawnerData in data)
            {
                yield return spawnerData.SpawnEntities(transform, spawnRadius);
            }

            yield return null;
        }


        private void OnDrawGizmos()
        {
            Color displayColor = Color.red;
            displayColor.a = 0.5f;

            Gizmos.color = displayColor;

            Gizmos.DrawSphere(transform.position, spawnRadius);
        }
    }
}